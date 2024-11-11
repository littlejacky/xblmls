using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyPlanManagerController
    {
        [HttpGet, Route(RouteCourse)]
        public async Task<ActionResult<GetCourseResult>> GetCourse([FromQuery] GetCourseRequest request)
        {
            return new GetCourseResult
            {

            };
        }

    }
}
