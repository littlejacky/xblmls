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
                plan.ConfigList = request.ConfigList;

                await _authManager.AddAdminLogAsync("修改计划", plan.Title);
                await _authManager.AddStatLogAsync(StatType.ExamPlanUpdate, "修改计划", plan.Id, plan.Title);

                await _examPlanRepository.UpdateAsync(plan);
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

                await _authManager.AddAdminLogAsync("新增计划", $"{plan.Title}");
                await _authManager.AddStatLogAsync(StatType.ExamPlanAdd, "新增计划", plan.Id, plan.Title);
                await _authManager.AddStatCount(StatType.ExamPlanAdd);

                await _examPlanRepository.UpdateAsync(plan);
            }

            if (request.SubmitType == SubmitType.Submit)
            {
                await _examManager.CreateImmediatelyTrainingTasks(plan);
            }

            return new BoolResult
            {
                Value = true
            };
        }

    }


}
