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

            start.IsSubmit = true;
            start.EndDateTime = DateTime.Now;

            var sumScore = await _examPlanAnswerRepository.ScoreSumAsync(start.Id);
            var objectiveSocre = await _examPlanAnswerRepository.ObjectiveScoreSumAsync(start.Id);
            var subjectiveScore = await _examPlanAnswerRepository.SubjectiveScoreSumAsync(start.Id);

            start.ObjectiveScore = objectiveSocre;
            start.SubjectiveScore = subjectiveScore;
            start.Score = sumScore;

            await _examPlanPracticeRepository.UpdateAsync(start);

            var record = await _examPlanRecordRepository.GetAsync(start.PlanRecordId);
            if(record.RequirePass && sumScore < record.PassScore)
            {
                await _examManager.RecreateImmediatelyTrainingTasks(record, start.UserId);
            }
        }
    }
}



