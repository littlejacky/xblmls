using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyPlanManagerController
    {
        [HttpGet, Route(RouteUser)]
        public async Task<ActionResult<GetUserResult>> Submit([FromQuery] GetUserRequest request)
        {
            return new GetUserResult
            {

            };
        }
    }


}
