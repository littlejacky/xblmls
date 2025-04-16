using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Home
{
    public partial class RegisterController
    {

        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            var config = await _configRepository.GetAsync();
            var ipAddress = PageUtils.GetIpAddress(Request);

            if (!config.IsUserCaptchaDisabled)
            {
                var captcha = TranslateUtils.JsonDeserialize<CaptchaUtils.Captcha>(_settingsManager.Decrypt(request.Token));
                if (captcha == null || string.IsNullOrEmpty(captcha.Value) || captcha.ExpireAt < DateTime.Now)
                {
                    return this.Error("验证码已超时，请点击刷新验证码！");
                }
                if (!StringUtils.EqualsIgnoreCase(captcha.Value, request.Value) || CaptchaUtils.IsAlreadyUsed(captcha, _cacheManager))
                {
                    return this.Error("验证码不正确，请重新输入！");
                }
            }

            // 验证手机号是否已存在
            if (await _userRepository.IsMobileExistsAsync(request.Mobile))
            {
                return this.Error("手机号已被注册，请更换手机号");
            }
            // 验证邮箱是否已存在
            else if (await _userRepository.IsEmailExistsAsync(request.Email))
            {
                return this.Error("邮箱已被注册，请更换邮箱");
            }
            // 验证工号是否已存在
            else if (await _userRepository.IsEmployeeIdExistsAsync(request.EmployeeId))
            {
                return this.Error("工号已被注册，请更换工号");
            }

            var user = new User
            {
                UserName = request.UserName,
                DisplayName = request.DisplayName,
                EmployeeId = request.EmployeeId,
                Mobile = request.Mobile,
                Email = request.Email
            };

            var (newUser, errorMessage) = await _userRepository.InsertAsync(user, request.Password, true, ipAddress);
            if (newUser == null)
            {
                return this.Error(errorMessage);
            }

            var registerLog = "用户注册";
            if (request.AppRegister) { registerLog = registerLog + "-移动端"; }
            await _logRepository.AddUserLogAsync(newUser, ipAddress, registerLog);

            return new SubmitResult
            {
                User = newUser
            };
        }
    }
}
