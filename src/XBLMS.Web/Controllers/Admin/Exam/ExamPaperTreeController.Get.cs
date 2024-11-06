using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamPaperTreeController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAuthorityAuth();

            var trees = await _examManager.GetExamPaperTreeCascadesAsync(auth, true);

            return new GetResult
            {
                Items = trees,
            };
        }
    }
}
