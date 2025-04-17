using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System;
using System.Threading.Tasks;
using XBLMS.Configuration;
using XBLMS.Core.Utils;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Route(Constants.ApiHomePrefix)]
    public partial class RegisterController : ControllerBase
    {
        private const string Route = "register";
        private const string RouteCaptcha = "register/captcha";

        private readonly IUserRepository _userRepository;
        private readonly IConfigRepository _configRepository;
        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;
        private readonly ILogRepository _logRepository;

        public RegisterController(IUserRepository userRepository, IConfigRepository configRepository,
            ISettingsManager settingsManager, ICacheManager cacheManager, ILogRepository logRepository)
        {
            _userRepository = userRepository;
            _configRepository = configRepository;
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
            _logRepository = logRepository;
        }

        public class GetResult
        {
            public string Version { get; set; }
            public bool IsUserCaptchaDisabled { get; set; }
        }

        public class SubmitRequest
        {
            public string UserName { get; set; }
            public string DisplayName { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Token { get; set; }
            public string Value { get; set; }
            public bool AppRegister { get; set; }
        }

        public class SubmitResult
        {
            public User User { get; set; }
        }
    }
}
