using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Services;

namespace XBLMS.Core.Services
{
    public partial class ExamManager
    {
        public async Task<bool> CreateDailyTrainingTasks()
        {
            var today = DateTime.Today;
            // 获取所有活动的培训计划
            var activePlans = await _examPlanRepository.GetActivePlansAsync(today);

            foreach (var plan in activePlans)
            {
                if (plan.FrequencyType == Enums.FrequencyType.Daily ||
                    (plan.FrequencyType == Enums.FrequencyType.Weekly && today.Day == 1) ||
                    (plan.FrequencyType == Enums.FrequencyType.Monthly && today.DayOfWeek != DayOfWeek.Saturday && today.DayOfWeek != DayOfWeek.Sunday) ||
                    (plan.FrequencyType == Enums.FrequencyType.PerWeekday && today.DayOfWeek != DayOfWeek.Saturday && today.DayOfWeek != DayOfWeek.Sunday) ||
                    (plan.FrequencyType == Enums.FrequencyType.Weekly && today.DayOfWeek == DayOfWeek.Monday)
                    )
                {
                    await SetPlanPractice(await NewPlanRecord(plan), today);
                }
            }

            return true;
        }

        public async Task<bool> CreateImmediatelyTrainingTasks(ExamPlan plan)
        {
            var today = DateTime.Today;

            await SetPlanPractice(await NewPlanRecord(plan), today, true);

            return true;
        }

        public async Task<ExamPlanRecord> NewPlanRecord(ExamPlan plan)
        {
            var record = new ExamPlanRecord(plan);
            await _examPlanRecordRepository.InsertAsync(record);
            return record;
        }

        private async Task SetPlanPractice(ExamPlanRecord plan, DateTime today, bool isImmediately = false)
        {
            ExamPlanPractice practice = new ExamPlanPractice();
            practice.PlanRecordId = plan.Id;
            practice.Title = $"{plan.Title}({today:yyyyMMdd})";

            practice.PracticeType = PracticeType.Random;
            practice.CompanyId = plan.CompanyId;
            practice.CreatorId = plan.CreatorId;
            practice.DepartmentId = plan.DepartmentId;

            var auth = await _authManager.GetAuthorityAuth();

            var userIds = await Arrange(plan, auth);
            foreach (var userId in userIds)
            {
                var tms = await SetExamPaperRantomByRandomNowAndExaming(plan, auth, userId);

                practice.UserId = userId;
                practice.TmIds = tms.Select(s => s.Id).ToList();
                practice.TmCount = tms.Count;
                practice.Zsds = tms.Select(s => s.Zhishidian).Distinct().ToList();
                practice.KeyWords = await _organManager.GetUserKeyWords(userId);

                await _examPlanPracticeRepository.InsertAsync(practice);
            }

            await _examPlanRepository.IncrementExecutedCountAsync(plan.Id);
        }

        public async Task MarkUnfinishPlanPractices()
        {
            var practices = await _examPlanPracticeRepository.ListUnfinishAsync();
            var rids = practices.Select(s => s.PlanRecordId).Distinct().ToList();
            var records = await _examPlanRecordRepository.GetIdsAsync(rids, null);
            foreach (var practice in practices)
            {
                var record = records.Find(f => f.Id == practice.PlanRecordId);
                if (record == null || !practice.BeginDateTime.HasValue) continue;

                var timeSpan = DateTime.Now - practice.BeginDateTime.Value;
                var useTotalSecond = timeSpan.TotalSeconds + practice.ExamTimeSeconds;
                if (useTotalSecond >= record.TimingMinute * 60)
                {
                    useTotalSecond = record.TimingMinute * 60;
                    practice.EndDateTime = practice.BeginDateTime.Value.AddMinutes(record.TimingMinute);
                    practice.ExamTimeSeconds = (int)useTotalSecond;

                    await _examPlanPracticeRepository.UpdateAsync(practice);
                }
            }
        }
    }
}
