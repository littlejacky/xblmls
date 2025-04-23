using System;
using System.Collections.Generic;
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
                            continue;
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
                            continue;
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
                            continue;
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
                            continue;
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

            return true;
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
