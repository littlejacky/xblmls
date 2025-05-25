using Datory;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Home.Exam
{
    public partial class ExamPlanPracticingController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] IdRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var item = await _examPlanPracticeRepository.GetAsync(request.Id);
            if (item != null)
            {
                var record = await _examPlanRecordRepository.GetAsync(item.PlanRecordId);
                int useTimeSecond = item.ExamTimeSeconds;
                int answeredCount = 0;
                if (!item.BeginDateTime.HasValue)
                {
                    item.BeginDateTime = DateTime.Now;
                    item.TmIds = item.TmIds.OrderBy(tm => { return StringUtils.Guid(); }).ToList();
                    await _examPlanPracticeRepository.UpdateAsync(item);
                }
                else
                {
                    var timeSpan = DateTime.Now - item.BeginDateTime.Value;
                    var useTotalSecond = timeSpan.TotalSeconds + useTimeSecond;
                    if (useTotalSecond >= record.TimingMinute * 60)
                    {
                        useTotalSecond = record.TimingMinute * 60;
                        useTimeSecond = -1;
                    }
                    else
                    {
                        useTimeSecond = (int)useTotalSecond;
                    }
                    item.ExamTimeSeconds = useTimeSecond;
                    await _examPlanPracticeRepository.UpdateAsync(item);

                    answeredCount = await _examPlanAnswerRepository.CountByPracticeId(item.Id);
                }

                return new GetResult
                {
                    Title = item.PracticeType.GetDisplayName(),
                    TmIds = item.TmIds,
                    Total = item.TmCount,
                    Watermark = await _authManager.GetWatermark(),
                    OpenExist = record.OpenExist,
                    IsTiming = record.IsTiming,
                    TimingMinute = record.TimingMinute,
                    UseTimeSecond = useTimeSecond,
                    TmIndex = answeredCount
                };

            }
            return this.Error("练习错误，请重试");
        }

        [HttpGet, Route(RouteTm)]
        public async Task<ActionResult<ItemResult<ExamTm>>> GetTm([FromQuery] IdRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var tm = await _examTmRepository.GetAsync(request.Id);
            if (tm != null)
            {
                await _examManager.GetTmInfoByPracticing(tm);

                bool isCollection = false;
                var collection = await _examPracticeCollectRepository.GetAsync(user.Id);
                if (collection != null && collection.TmIds.Contains(tm.Id))
                {
                    isCollection = true;
                }

                tm.Set("IsCollection", isCollection);

                bool isWrong = false;
                var wrong = await _examPracticeWrongRepository.GetAsync(user.Id);
                if (wrong != null && wrong.TmIds.Contains(tm.Id))
                {
                    isWrong = true;
                }
                tm.Set("IsWrong", isWrong);


                return new ItemResult<ExamTm>
                {
                    Item = tm
                };
            }
            return this.Error("题目加载错误，请继续");
        }
    }
}



