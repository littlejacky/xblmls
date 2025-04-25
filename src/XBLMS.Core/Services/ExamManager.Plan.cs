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
                if (plan.PlanType == PlanType.Practice)
                {
                    await SetPlanPractice(plan, today);
                }
                else if (plan.PlanType == PlanType.Paper)
                {
                    await SetPlanPaper(plan, today);
                }
            }

            return true;
        }

        private async Task SetPlanPractice(ExamPlan plan, DateTime today)
        {
            ExamPractice practice = null;
            practice.PlanId = plan.Id;
            practice.Title += $"({today:YYYYMMdd})";

            if (plan.FrequencyType == Enums.FrequencyType.Daily ||
                (plan.FrequencyType == Enums.FrequencyType.Weekly && today.Day == 1) ||
                (plan.FrequencyType == Enums.FrequencyType.Monthly && today.DayOfWeek != DayOfWeek.Saturday && today.DayOfWeek != DayOfWeek.Sunday) ||
                (plan.FrequencyType == Enums.FrequencyType.PerWeekday && today.DayOfWeek != DayOfWeek.Saturday && today.DayOfWeek != DayOfWeek.Sunday) ||
                (plan.FrequencyType == Enums.FrequencyType.Weekly && today.DayOfWeek == DayOfWeek.Monday) ||
                (plan.FrequencyType == Enums.FrequencyType.Yearly && today.Day == 1 && today.Month == 1)
                )
            {
                practice = new ExamPractice();

                practice.PracticeType = PracticeType.Random;
                practice.CompanyId = plan.CompanyId;
                practice.CreatorId = plan.CreatorId;
                practice.DepartmentId = plan.DepartmentId;

                var auth = await _authManager.GetAuthorityAuth();

                var userIds = await Arrange(plan, auth);
                foreach (var userId in userIds)
                {
                    var tms = await SetExamPaperRantomByRandomNowAndExaming(plan, auth, userId);

                    practice.TmIds = tms.Select(s => s.Id).ToList();
                    practice.TmCount = tms.Count;
                    practice.Zsds = tms.Select(s => s.Zhishidian).Distinct().ToList();
                    practice.KeyWords = await _organManager.GetUserKeyWords(userId);

                    await _examPracticeRepository.InsertAsync(practice);
                }
            }
        }

        private async Task SetPlanPaper(ExamPlan plan, DateTime today)
        {
            ExamPaper paper = plan;
            paper.PlanId = plan.Id;
            paper.Title += $"({today:YYYYMMdd})";

            switch (plan.FrequencyType)
            {
                case Enums.FrequencyType.Daily:
                    paper = new ExamPaper();
                    paper.ExamBeginDateTime = today.Date;
                    paper.ExamEndDateTime = today.Date.AddDays(1).AddSeconds(-1);
                    break;

                case Enums.FrequencyType.Monthly:
                    if (today.Day == 1) // 每月第一天
                    {
                        paper = new ExamPaper();
                        paper.ExamBeginDateTime = today.Date;
                        paper.ExamEndDateTime = today.Date.AddMonths(1).AddSeconds(-1);
                    }
                    else
                    {
                        return;
                    }
                    break;

                case Enums.FrequencyType.PerWeekday:
                    if (today.DayOfWeek != DayOfWeek.Saturday && today.DayOfWeek != DayOfWeek.Sunday) // 非周末
                    {
                        paper = new ExamPaper();
                        paper.ExamBeginDateTime = today.Date;
                        paper.ExamEndDateTime = today.Date.AddDays(1).AddSeconds(-1);
                    }
                    else
                    {
                        return;
                    }
                    break;

                case Enums.FrequencyType.Weekly:
                    if (today.DayOfWeek == DayOfWeek.Monday) // 每周一
                    {
                        paper = new ExamPaper();
                        paper.ExamBeginDateTime = today.Date;
                        paper.ExamEndDateTime = today.Date.AddDays(7).AddSeconds(-1);
                    }
                    else
                    {
                        return;
                    }
                    break;

                case Enums.FrequencyType.Yearly:
                    if (today.Day == 1 && today.Month == 1) // 每年的第一天
                    {
                        paper = new ExamPaper();
                        paper.ExamBeginDateTime = today.Date;
                        paper.ExamEndDateTime = today.Date.AddYears(1).AddSeconds(-1);
                    }
                    else
                    {
                        return;
                    }
                    break;
            }

            paper.SubmitType = SubmitType.Submit;
            paper.CompanyId = plan.CompanyId;
            paper.CreatorId = plan.CreatorId;
            paper.DepartmentId = plan.DepartmentId;

            var paperId = await _examPaperRepository.InsertAsync(paper);

            var auth = await _authManager.GetAuthorityAuth();

            paper = await _examPaperRepository.GetAsync(paperId);
            await SetRandomConfigs(plan.ConfigList, paper);

            await PaperRandomSet(paper, auth);
            await Arrange(paper, auth);

            await _authManager.AddAdminLogAsync("按计划发布考试", paper.Title);
            await _authManager.AddStatLogAsync(StatType.ExamAdd, "按计划发布考试", paper.Id, paper.Title);
            await _authManager.AddStatCount(StatType.ExamAdd);

            await _examPaperRepository.UpdateAsync(paper);
        }

        private async Task SetRandomConfigs(List<ExamPaperRandomConfig> randomConfigs, ExamPaper paper)
        {
            await _examPaperRandomConfigRepository.DeleteByPaperAsync(paper.Id);

            if (paper.TmRandomType != ExamPaperTmRandomType.RandomNone)
            {
                var txIds = new List<int>();
                if (randomConfigs != null && randomConfigs.Count > 0)
                {
                    foreach (var randomConfig in randomConfigs)
                    {
                        if (randomConfig.Nandu1TmCount > 0 || randomConfig.Nandu2TmCount > 0 || randomConfig.Nandu3TmCount > 0 || randomConfig.Nandu4TmCount > 0 || randomConfig.Nandu5TmCount > 0)
                        {
                            randomConfig.ExamPaperId = paper.Id;
                            txIds.Add(randomConfig.TxId);
                            await _examPaperRandomConfigRepository.InsertAsync(randomConfig);
                        }

                    }
                }
                paper.TxIds = txIds;
            }
        }
    }
}
