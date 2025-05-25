using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Dto;

namespace XBLMS.Web.Controllers.Home.Exam
{
    public partial class ExamPlanPracticeResultController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] IdRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var practice = await _examPlanPracticeRepository.GetAsync(request.Id);
            return new GetResult
            {
                Total = practice.TmCount,
                AnswerTotal = practice.AnswerCount,
                RightTotal = practice.RightCount,
                WrongTotal = practice.AnswerCount - practice.RightCount
            };
        }
    }
}
