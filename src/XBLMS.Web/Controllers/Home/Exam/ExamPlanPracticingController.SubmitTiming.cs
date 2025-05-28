using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;

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
            //await _examPlanPracticeRepository.UpdateEndDateTimeAsync(request.Id, DateTime.Now);

            var start = await _examPlanPracticeRepository.GetAsync(request.Id);
            var paper = await _databaseManager.ExamPaperRepository.GetAsync(start.PlanRecordId);

            start.EndDateTime = DateTime.Now;

            var sumScore = await _examPlanAnswerRepository.ScoreSumAsync(startId);
            var objectiveSocre = await _examPlanAnswerRepository.ObjectiveScoreSumAsync(startId);
            var subjectiveScore = await _examPlanAnswerRepository.SubjectiveScoreSumAsync(startId);

            start.ObjectiveScore = objectiveSocre;
            start.SubjectiveScore = subjectiveScore;
            start.Score = sumScore;

            await _examPlanPracticeRepository.UpdateAsync(start);

        }
    }
}



