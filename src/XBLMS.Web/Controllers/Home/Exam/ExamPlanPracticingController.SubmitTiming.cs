using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using XBLMS.Dto;

namespace XBLMS.Web.Controllers.Home.Exam
{
    public partial class ExamPlanPracticingController
    {
        [HttpPost, Route(RouteSubmitTiming)]
        public async void SubmitTiming([FromBody] IdRequest request)
        {
            await _examPlanPracticeRepository.IncrementAsync(request.Id);
        }
        [HttpPost, Route(RouteSubmit)]
        public async void Submit([FromBody] IdRequest request)
        {
            await _examPlanPracticeRepository.UpdateEndDateTimeAsync(request.Id, DateTime.Now);
        }
    }
}



