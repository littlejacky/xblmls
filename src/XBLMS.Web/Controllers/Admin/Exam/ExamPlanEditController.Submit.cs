using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamPlanEditController
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
            var plan = request.Item;

            if (plan.TmRandomType != ExamPaperTmRandomType.RandomExaming)
            {
                plan.Moni = false;
            }

            if (plan.Id > 0)
            {
                var oldPlan = await _examPlanRepository.GetAsync(plan.Id);

                if (request.SubmitType == SubmitType.Submit)
                {
                    plan.SubmitType = request.SubmitType;
                    plan.ConfigList = request.ConfigList;
                    //await _examManager.ClearRandom(plan.Id, request.IsClear);

                    //await SetRandomConfigs(request.ConfigList, plan);

                    //await _examManager.PaperRandomSet(plan, auth);
                    //await _examManager.Arrange(plan, auth);

                    await _authManager.AddAdminLogAsync("重新发布考试计划", plan.Title);
                    await _authManager.AddStatLogAsync(StatType.ExamPlanUpdate, "重新发布考试计划", plan.Id, plan.Title);
                }
                else
                {
                    await _authManager.AddAdminLogAsync("修改考试计划", plan.Title);
                    await _authManager.AddStatLogAsync(StatType.ExamPlanUpdate, "修改考试计划", plan.Id, plan.Title);
                }

                await _examPlanRepository.UpdateAsync(plan);

                //if (request.IsUpdateDateTime)
                //{
                //    await _examPaperUserRepository.UpdateExamDateTimeAsync(plan.Id, plan.ExamBeginDateTime.Value, plan.ExamEndDateTime.Value);
                //}
                //if (request.IsUpdateExamTimes)
                //{
                //    await _examPaperUserRepository.UpdateExamTimesAsync(plan.Id, plan.ExamTimes);
                //}
                //if (oldPlan.Title != plan.Title)
                //{
                //    await _examPaperUserRepository.UpdateKeyWordsAsync(plan.Id, plan.Title);
                //    await _examPaperStartRepository.UpdateKeyWordsAsync(plan.Id, plan.Title);
                //}
                //if (oldPlan.Moni != plan.Moni)
                //{
                //    await _examPaperUserRepository.UpdateMoniAsync(plan.Id, plan.Moni);
                //}
            }
            else
            {
                plan.SubmitType = request.SubmitType;
                plan.CompanyId = auth.CompanyId;
                plan.CreatorId = auth.AdminId;
                plan.DepartmentId = auth.DepartmentId;

                var paperId = await _examPlanRepository.InsertAsync(plan);


                plan = await _examPlanRepository.GetAsync(paperId);
                plan.ConfigList = request.ConfigList;
                //await SetRandomConfigs(request.ConfigList, plan);

                if (request.SubmitType == SubmitType.Submit)
                {
                    //await _examManager.PaperRandomSet(plan, auth);
                    //await _examManager.Arrange(plan, auth);

                    await _authManager.AddAdminLogAsync("发布考试计划", plan.Title);
                    await _authManager.AddStatLogAsync(StatType.ExamPlanAdd, "发布考试计划", plan.Id, plan.Title);
                    await _authManager.AddStatCount(StatType.ExamPlanAdd);
                }
                else
                {
                    await _authManager.AddAdminLogAsync("新增考试计划", $"{plan.Title}");
                    await _authManager.AddStatLogAsync(StatType.ExamPlanAdd, "新增考试计划", plan.Id, plan.Title);
                    await _authManager.AddStatCount(StatType.ExamPlanAdd);
                }
                await _examPlanRepository.UpdateAsync(plan);
            }

            return new BoolResult
            {
                Value = true
            };
        }

    }


}
