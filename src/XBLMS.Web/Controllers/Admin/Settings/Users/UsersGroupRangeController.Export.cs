using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersGroupRangeController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] GetRequest request)
        {
            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Export))
            {
                return this.NoAuth();
            }

            var auth = await _authManager.GetAuthorityAuth();


            return new StringResult
            {
                Value = ""
            };
        }
    }
}
