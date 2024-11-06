using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyPlanEditController
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

            if (plan.Id > 0)
            {
                await _studyPlanRepository.UpdateAsync(plan);
                var planCourseIds = new List<int>();

                if (request.CourseList != null && request.CourseList.Count > 0)
                {
                    foreach (var item in request.CourseList)
                    {

                        if (item.Id > 0)
                        {
                            await _studyPlanCourseRepository.UpdateAsync(item);
                        }
                        else
                        {
                            item.CompanyId = auth.CompanyId;
                            item.DepartmentId = auth.DepartmentId;
                            item.CreatorId = auth.AdminId;
                            item.PlanId = plan.Id;

                            item.Id = await _studyPlanCourseRepository.InsertAsync(item);
                        }
                        planCourseIds.Add(item.Id);
                    }
                }

                if (request.CourseSelectList != null && request.CourseSelectList.Count > 0)
                {
                    foreach (var item in request.CourseSelectList)
                    {
                        if (item.Id > 0)
                        {
                            await _studyPlanCourseRepository.UpdateAsync(item);
                        }
                        else
                        {
                            item.CompanyId = auth.CompanyId;
                            item.DepartmentId = auth.DepartmentId;
                            item.CreatorId = auth.AdminId;
                            item.PlanId = plan.Id;

                            item.Id = await _studyPlanCourseRepository.InsertAsync(item);
                        }
                        planCourseIds.Add(item.Id);
                    }

                }
                await _studyPlanCourseRepository.DeleteByNotIdsAsync(planCourseIds, plan.Id);
                await _authManager.AddAdminLogAsync("修改培训计划", $"计划名称：{plan.PlanName}");
            }
            else
            {
                plan.CompanyId = auth.CompanyId;
                plan.DepartmentId = auth.DepartmentId;
                plan.CreatorId = auth.AdminId;

                var planId = await _studyPlanRepository.InsertAsync(plan);
                plan.Id = planId;

                if (request.CourseList != null && request.CourseList.Count > 0)
                {
                    foreach (var item in request.CourseList)
                    {
                        item.CompanyId = auth.CompanyId;
                        item.DepartmentId = auth.DepartmentId;
                        item.CreatorId = auth.AdminId;
                        item.PlanId = planId;

                        await _studyPlanCourseRepository.InsertAsync(item);
                    }
                }


                if (request.CourseSelectList != null && request.CourseSelectList.Count > 0)
                {
                    foreach (var item in request.CourseSelectList)
                    {
                        item.CompanyId = auth.CompanyId;
                        item.DepartmentId = auth.DepartmentId;
                        item.CreatorId = auth.AdminId;
                        item.PlanId = planId;

                        await _studyPlanCourseRepository.InsertAsync(item);
                    }
                }

                await _authManager.AddAdminLogAsync("新增培训计划", $"计划名称:{plan.PlanName}");
            }
            if (plan.SubmitType == SubmitType.Submit)
            {
                await _studyManager.PlanArrangeUser(plan, auth);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }


}
