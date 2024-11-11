using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyPlanManagerController
    {
        [HttpGet, Route(RouteCourse)]
        public async Task<ActionResult<GetCourseResult>> GetCourse([FromQuery] GetCourseRequest request)
        {
            var plan = await _studyPlanRepository.GetAsync(request.Id);

            var resultList = new List<StudyPlanCourse>();

            var list = await _studyPlanCourseRepository.GetListAsync(plan.Id);
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (!string.IsNullOrEmpty(request.KeyWords) && item.CourseName.Contains(request.KeyWords))
                    {
                        var totalUser = await _studyPlanUserRepository.GetCountAsync(plan.Id, "");
                        var overUser = await _studyCourseUserRepository.GetOverCountAsync(plan.Id, item.CourseId, true);
                        var studyUser = await _studyCourseUserRepository.GetOverCountAsync(plan.Id, item.CourseId, null);

                        item.Set("TotalUser", totalUser);
                        item.Set("OverUser", overUser);
                        item.Set("StudyUser", studyUser);

                        resultList.Add(item);
                    }

                }
            }
            return new GetCourseResult
            {
                List = resultList,
            };
        }
        [HttpPost, Route(RouteCourseExport)]
        public async Task<ActionResult<StringResult>> UserExport([FromBody] GetCourseRequest request)
        {
            var plan = await _studyPlanRepository.GetAsync(request.Id);

            var fileName = $"{plan.PlanName}-课程列表.xlsx";
            var filePath = _pathManager.GetDownloadFilesPath(fileName);

            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var head = new List<string>
            {
                "序号",
                "课程",
                "类型",
                "课时",
                "学时",
                "学分",
                "培训人数",
                "学习人数",
                "完成人数",
                "完成率",
            };
            var rows = new List<List<string>>();

            var list = await _studyPlanCourseRepository.GetListAsync(plan.Id);
            if (list != null && list.Count > 0)
            {
                var index = 1;

                foreach (var item in list)
                {
                    if (!string.IsNullOrEmpty(request.KeyWords) && item.CourseName.Contains(request.KeyWords))
                    {
                        var type = "必修课";
                        if (item.IsSelectCourse)
                        {
                            type = "选修课";
                            if (item.OffLine)
                            {
                                type = type + "-线下课";
                            }
                        }
                        else
                        {
                            if (item.OffLine)
                            {
                                type = type + "-线下课";
                            }
                        }

                        var totalUser = await _studyPlanUserRepository.GetCountAsync(plan.Id, "");
                        var overUser = await _studyCourseUserRepository.GetOverCountAsync(plan.Id, item.CourseId, true);
                        var studyUser = await _studyCourseUserRepository.GetOverCountAsync(plan.Id, item.CourseId, null);

                        rows.Add([
                            index.ToString(),
                            item.CourseName,
                            type,
                             item.StudyHour.ToString(),
                             TranslateUtils.ToMinuteAndSecond(item.Duration,true),
                             item.Credit.ToString(),
                             totalUser.ToString(),
                             studyUser.ToString(),
                             overUser.ToString(),
                              TranslateUtils.ToPercent(overUser,studyUser)+"%"
                       ]);

                        index++;
                    }
                }
            }

            ExcelUtils.Write(filePath, head, rows);

            var downloadUrl = _pathManager.GetDownloadFilesUrl(fileName);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
