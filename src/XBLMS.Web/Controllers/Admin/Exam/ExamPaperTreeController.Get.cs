using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamPaperTreeController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get(bool isPlan = false)
        {
            var auth = await _authManager.GetAuthorityAuth();

            var trees = await _examManager.GetExamPaperTreeCascadesAsync(auth, true, isPlan);

            return new GetResult
            {
                Items = trees,
            };
        }
    }
}
