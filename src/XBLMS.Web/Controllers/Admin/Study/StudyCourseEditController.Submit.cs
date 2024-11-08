using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseEditController
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
            var course = request.Item;
            course.TreePath = await _studyCourseTreeRepository.GetPathAsync(course.TreeId);


            if (course.Id > 0)
            {
                await _studyCourseRepository.UpdateAsync(course);
                var courseWareIds = new List<int>();

                if (request.WareList != null && request.WareList.Count > 0)
                {
                    foreach (var ware in request.WareList)
                    {
                        if (ware.Id > 0)
                        {
                            await _studyCourseWareRepository.UpdateAsync(ware);
                        }
                        else
                        {
                            var file = await _studyCourseFilesRepository.GetAsync(ware.CourseFileId);
                            ware.Id = await _studyCourseWareRepository.InsertAsync(new StudyCourseWare
                            {
                                CourseId = course.Id,
                                CourseFileId = file.Id,
                                FileName = ware.FileName,
                                Duration = file.Duration,
                                Url = file.Url,
                                Taxis = ware.Taxis,
                                CompanyId = auth.CompanyId,
                                DepartmentId = auth.DepartmentId,
                                CreatorId = auth.AdminId
                            });
                        }
                        courseWareIds.Add(ware.Id);
                    }
                }

                await _studyCourseWareRepository.DeleteByNotIdsAsync(courseWareIds, course.Id);
                await _studyCourseUserRepository.UpdateByCourseAsync(course);
                await _authManager.AddAdminLogAsync("修改课程", $"{course.Name}");
            }
            else
            {
                course.CompanyId = auth.CompanyId;
                course.DepartmentId = auth.DepartmentId;
                course.CreatorId = auth.AdminId;
                course.TotaEvaluationlUser = 0;
                course.TotalEvaluation = 0;
                course.TotalUser = 0;

                var courseId = await _studyCourseRepository.InsertAsync(course);
                if (request.WareList != null && request.WareList.Count > 0)
                {
                    foreach (var ware in request.WareList)
                    {
                        var file = await _studyCourseFilesRepository.GetAsync(ware.CourseFileId);
                        await _studyCourseWareRepository.InsertAsync(new StudyCourseWare
                        {
                            CourseId = courseId,
                            CourseFileId = file.Id,
                            FileName = ware.FileName,
                            Duration = file.Duration,
                            Url = file.Url,
                            Taxis = ware.Taxis,
                            CompanyId = auth.CompanyId,
                            DepartmentId = auth.DepartmentId,
                            CreatorId = auth.AdminId
                        });
                    }
                }

                await _authManager.AddAdminLogAsync("新增课程", $"{course.Name}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }


}
