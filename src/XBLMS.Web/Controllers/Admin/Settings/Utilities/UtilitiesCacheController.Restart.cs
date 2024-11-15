using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Dto;
using XBLMS.Core.Utils;
using XBLMS.Enums;
using XBLMS.Utils;
using XBLMS.Configuration;

namespace XBLMS.Web.Controllers.Admin.Settings.Utilities
{
    public partial class UtilitiesCacheController
    {
        [HttpPost, Route(RouteRestart)]
        public async Task<ActionResult<BoolResult>> Restart()
        {
            if (_settingsManager.IsSafeMode)
            {
                return this.Error(Constants.ErrorSafe);
            }

            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.SystemRestart))
            {
                return this.NoAuth();
            }
            if (!await _authManager.HasPermissionsAsync())
            {
                return this.NoAuth();
            }
            var admin = await _authManager.GetAdminAsync();

            _hostApplicationLifetime.StopApplication();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
