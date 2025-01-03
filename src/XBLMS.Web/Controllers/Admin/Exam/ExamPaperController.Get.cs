using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Utils;
namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamPaperController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> GetManage([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasPermissionsAsync())
            {
                return this.NoAuth();
            }

            var auth = await _authManager.GetAuthorityAuth();

            var treeIds = new List<int>();
            if (request.TreeId > 0)
            {
                if (request.TreeIsChildren)
                {
                    treeIds = await _examPaperTreeRepository.GetIdsAsync(request.TreeId);
                }
                else
                {
                    treeIds.Add(request.TreeId);
                }
            }
            var (total, list) = await _examPaperRepository.GetListAsync(auth, treeIds, request.Keyword, request.PageIndex, request.PageSize);
            if (total > 0)
            {
                foreach (var item in list)
                {
                    var markCount = await _examPaperStartRepository.CountByMarkAsync(item.Id);
                    item.Set("MarkCount", markCount);

                    var useCount = await _studyCourseRepository.GetPaperUseCount(item.Id) + await _studyPlanCourseRepository.GetPaperUseCount(item.Id);
                    item.Set("UseCount", useCount);
                }
            }
            return new GetResult
            {
                Total = total,
                Items = list,
            };
        }

    }
}
