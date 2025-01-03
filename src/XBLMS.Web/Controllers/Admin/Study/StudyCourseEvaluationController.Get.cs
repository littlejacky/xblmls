using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using XBLMS.Utils;
namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseEvaluationController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasPermissionsAsync())
            {
                return this.NoAuth();
            }

            var auth = await _authManager.GetAuthorityAuth();

            var (total, list) = await _studyCourseEvaluationRepository.GetListAsync(auth, request.Keyword, request.PageIndex, request.PageSize);
            if (total > 0)
            {
                foreach (var item in list)
                {
                    var useCount = await _studyCourseRepository.GetPaperUseCount(item.Id) + await _studyPlanCourseRepository.GetPaperUseCount(item.Id);
                    item.Set("UseCount", useCount);
                }
            }
            return new GetResult
            {
                Total = total,
                List = list,
            };
        }

    }
}
