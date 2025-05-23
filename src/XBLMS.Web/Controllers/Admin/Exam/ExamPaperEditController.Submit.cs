﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamPaperEditController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] GetSubmitRequest request)
        {
            if (request.Item.Id > 0)
            {
                if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Update))
                {
                    return this.NoAuth();
                }
            }
            else
            {
                if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Add))
                {
                    return this.NoAuth();
                }
            }


            var auth = await _authManager.GetAuthorityAuth();
            var paper = request.Item;

            if (paper.TmRandomType != ExamPaperTmRandomType.RandomExaming)
            {
                paper.Moni = false;
            }

            if (paper.Id > 0)
            {
                var oldPaper = await _examPaperRepository.GetAsync(paper.Id);

                if (request.SubmitType == SubmitType.Submit)
                {
                    paper.SubmitType = request.SubmitType;

                    await _examManager.ClearRandom(paper.Id, request.IsClear);

                    await SetRandomConfigs(request.ConfigList, paper);
                    await SetTmGroupProportions(request.TmGroupProportions, paper);

                    await _examManager.PaperRandomSet(paper, auth);
                    await _examManager.Arrange(paper, auth);

                    await _authManager.AddAdminLogAsync("重新发布试卷", paper.Title);
                    await _authManager.AddStatLogAsync(StatType.ExamUpdate, "重新发布试卷", paper.Id, paper.Title);
                }
                else
                {
                    await _authManager.AddAdminLogAsync("修改试卷", paper.Title);
                    await _authManager.AddStatLogAsync(StatType.ExamUpdate, "修改试卷", paper.Id, paper.Title);
                }

                await _examPaperRepository.UpdateAsync(paper);

                if (request.IsUpdateDateTime)
                {
                    await _examPaperUserRepository.UpdateExamDateTimeAsync(paper.Id, paper.ExamBeginDateTime.Value, paper.ExamEndDateTime.Value);
                }
                if (request.IsUpdateExamTimes)
                {
                    await _examPaperUserRepository.UpdateExamTimesAsync(paper.Id, paper.ExamTimes);
                }
                if (oldPaper.Title != paper.Title)
                {
                    await _examPaperUserRepository.UpdateKeyWordsAsync(paper.Id, paper.Title);
                    await _examPaperStartRepository.UpdateKeyWordsAsync(paper.Id, paper.Title);
                }
                if (oldPaper.Moni != paper.Moni)
                {
                    await _examPaperUserRepository.UpdateMoniAsync(paper.Id, paper.Moni);
                }
            }
            else
            {
                paper.SubmitType = request.SubmitType;
                paper.CompanyId = auth.CompanyId;
                paper.CreatorId = auth.AdminId;
                paper.DepartmentId = auth.DepartmentId;

                var paperId = await _examPaperRepository.InsertAsync(paper);


                paper = await _examPaperRepository.GetAsync(paperId);
                await SetRandomConfigs(request.ConfigList, paper);
                await SetTmGroupProportions(request.TmGroupProportions, paper);

                if (request.SubmitType == SubmitType.Submit)
                {
                    await _examManager.PaperRandomSet(paper, auth);
                    await _examManager.Arrange(paper, auth);

                    await _authManager.AddAdminLogAsync("发布考试", paper.Title);
                    await _authManager.AddStatLogAsync(StatType.ExamAdd, "发布考试", paper.Id, paper.Title);
                    await _authManager.AddStatCount(StatType.ExamAdd);
                }
                else
                {
                    await _authManager.AddAdminLogAsync("新增考试", $"{paper.Title}");
                    await _authManager.AddStatLogAsync(StatType.ExamAdd, "新增考试", paper.Id, paper.Title);
                    await _authManager.AddStatCount(StatType.ExamAdd);
                }
                await _examPaperRepository.UpdateAsync(paper);
            }

            return new BoolResult
            {
                Value = true
            };
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

        private async Task SetTmGroupProportions(List<ExamTmGroupProportion> tmGroupProportions, ExamPaper paper)
        {
            await _examTmGroupProportionRepository.DeleteByPaperAsync(paper.Id);

            if (paper.TmRandomType != ExamPaperTmRandomType.RandomNone)
            {
                if (tmGroupProportions != null && tmGroupProportions.Count > 0)
                {
                    foreach (var tmGroupProportion in tmGroupProportions)
                    {
                        tmGroupProportion.ExamPaperId = paper.Id;
                        await _examTmGroupProportionRepository.InsertAsync(tmGroupProportion);
                    }
                }
            }
        }
    }


}
