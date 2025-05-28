using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamPlanRecordManagerController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] IdRequest request)
        {
            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Manage))
            {
                return this.NoAuth();
            }

            var auth = await _authManager.GetAuthorityAuth();

            var practice = await _examPlanRecordRepository.GetAsync(request.Id);
            var maxScore = await _examPlanPracticeRepository.GetMaxScoreAsync(request.Id);
            var minScore = await _examPlanPracticeRepository.GetMinScoreAsync(request.Id);

            var sumScore = await _examPlanPracticeRepository.SumScoreAsync(request.Id);
            //var sumScoreDistinct = await _examPaperStartRepository.SumScoreDistinctAsync(request.Id);

            var scoreCount = await _examPlanPracticeRepository.CountAsync(request.Id);
            var scoreCountDistinct = await _examPlanPracticeRepository.CountDistinctAsync(request.Id);


            var userTotal = await _examPlanPracticeRepository.CountUserAsync(request.Id);

            var passTotal = await _examPlanPracticeRepository.CountByPassAsync(request.Id, practice.PassScore);
            var passTotalDistinct = await _examPlanPracticeRepository.CountByPassDistinctAsync(request.Id, practice.PassScore);

            return new GetResult
            {
                Title = practice.Title,
                TotalScore = practice.TotalScore,
                PassScore = practice.PassScore,
                TotalUser = userTotal,
                MaxScore = maxScore,
                MinScore = minScore,
                TotalPass = passTotal,
                TotalPassDistinct = passTotalDistinct,
                TotalUserScore = sumScore,
                TotalExamTimes = scoreCount,
                TotalExamTimesDistinct = scoreCountDistinct
            };
        }
    }
}
