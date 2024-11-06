using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Web.Controllers.Admin.Common
{
    public partial class SelectAuthOrganController
    {
        [HttpPost, Route(RouteSet)]
        public async Task<ActionResult<BoolResult>> Set([FromBody] IdRequest request)
        {
            var admin = await _authManager.GetAdminAsync();

            admin.AuthCurManageOrganId = request.Id;

            await _administratorRepository.UpdateAsync(admin);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
