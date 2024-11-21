using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamQuestionnaireController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> GetManage([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAuthorityAuth();

            var (total, list) = await _examQuestionnaireRepository.GetListAsync(auth, request.Keyword, request.PageIndex, request.PageSize);

            return new GetResult
            {
                Total = total,
                Items = list,
            };
        }

    }
}
