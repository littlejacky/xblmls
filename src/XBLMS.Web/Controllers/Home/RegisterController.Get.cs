using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace XBLMS.Web.Controllers.Home
{
    public partial class RegisterController 
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                Version = _settingsManager.Version,
                IsUserCaptchaDisabled = config.IsUserCaptchaDisabled
            };
        }
    }
}
