using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Dto;

namespace XBLMS.Web.Controllers.Home.Exam
{
    public partial class ExamPlanPracticeLogController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            await _examPlanPracticeRepository.DeleteAsync(user.Id);

            return new BoolResult
            {
               Value= true,
            };
        }
    }
}
