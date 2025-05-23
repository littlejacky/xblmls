﻿using Microsoft.AspNetCore.Mvc;
using System;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Home
{
    public partial class RegisterController
    {
        [HttpPost, Route(RouteCaptcha)]
        public StringResult CaptchaNew()
        {
            var captcha = new CaptchaUtils.Captcha
            {
                Value = CaptchaUtils.GetCode(),
                ExpireAt = DateTime.Now.AddMinutes(10)
            };
            var json = TranslateUtils.JsonSerialize(captcha);

            return new StringResult
            {
                Value = _settingsManager.Encrypt(json)
            };
        }
    }
}
