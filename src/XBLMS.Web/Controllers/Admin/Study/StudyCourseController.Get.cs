using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using XBLMS.Enums;
using XBLMS.Utils;
namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasPermissionsAsync())
            {
                return this.NoAuth();
            }

            var auth = await _authManager.GetAuthorityAuth();

            var (total, list) = await _studyCourseRepository.GetListAsync(auth, request.Keyword, request.Type, request.TreeId, request.TreeIsChildren, request.PageIndex, request.PageSize);

            return new GetResult
            {
                Total = total,
                List = list,
            };
        }

    }
}
