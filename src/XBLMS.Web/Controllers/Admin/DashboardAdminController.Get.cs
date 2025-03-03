using Datory;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin
{
    public partial class DashboardAdminController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetLogResult>> GetLog([FromQuery] GetLogRequest request)
        {
            var auth = await _authManager.GetAuthorityAuth();

            var (total, list) = await _statLogRepository.GetListAsync(auth, null, null, request.PageIndex, request.PageSize);
            if (total > 0)
            {
                foreach (var item in list)
                {
                    var color = "success";
                    if (item.StatType.GetValue().Contains("Update"))
                    {
                        color = "warning";
                    }
                    if (item.StatType.GetValue().Contains("Delete"))
                    {
                        color = "danger";
                    }
                    if (item.StatType.GetValue().Contains("Export"))
                    {
                        color = "primary";
                        var entity = TranslateUtils.JsonDeserialize<StringResult>(item.LastEntity);
                        item.Set("Url", entity.Value);
                    }
                    item.Set("Color", color);

                    var adminName = "我";
                    if (item.AdminId != auth.AdminId)
                    {
                        var otherAdmin = await _administratorRepository.GetByUserIdAsync(item.AdminId);
                        if (otherAdmin != null)
                        {
                            adminName = otherAdmin.DisplayName;
                        }
                    }

                    item.Set("AdminName", adminName);

                    item.Set("Title", item.StatTypeStr);

                    item.Set("Date", DateUtils.ParseThisMoment(item.CreatedDate.Value, DateTime.Now));

                    var name = item.ObjectName;


                    var isView = false;
                    var isEdit = false;

                    var isPlan = false;
                    if (item.StatType == StatType.StudyPlanAdd || item.StatType == StatType.StudyPlanUpdate)
                    {
                        isPlan = true;
                        if (await _databaseManager.StudyPlanRepository.ExistsAsync(item.ObjectId))
                        {
                            isView = true;
                            isEdit = true;
                        }
                        else
                        {
                            name = $"{name}(已删除)";
                        }
                    }
                    item.Set("IsPlan", isPlan);

                    var isCourse = false;
                    var isFace = false;
                    if (item.StatType == StatType.StudyCourseAdd || item.StatType == StatType.StudyCourseUpdate)
                    {
                        isCourse = true;
                        if (await _databaseManager.StudyCourseRepository.ExistsAsync(item.ObjectId))
                        {
                            var course = await _databaseManager.StudyCourseRepository.GetAsync(item.ObjectId);
                            isFace = course.OffLine;
                            isView = true;
                            isEdit = true;
                        }
                        else
                        {
                            name = $"{name}(已删除)";
                        }
                    }
                    item.Set("IsFace", isFace);
                    item.Set("IsCourse", isCourse);

                    var isFile = false;
                    if (item.StatType == StatType.StudyFileAdd || item.StatType == StatType.StudyFileUpdate)
                    {
                        isFile = true;
                        if (await _databaseManager.StudyCourseFilesRepository.ExistsAsync(item.ObjectId))
                        {
                            isView = true;
                        }
                        else
                        {
                            name = $"{name}(已删除)";
                        }
                    }
                    item.Set("IsFile", isFile);

                    var isTm = false;
                    if (item.StatType == StatType.ExamTmAdd || item.StatType == StatType.ExamTmUpdate)
                    {
                        isTm = true;
                        if (await _databaseManager.ExamTmRepository.ExistsAsync(item.ObjectId))
                        {
                            isView = true;
                            isEdit = true;
                        }
                        else
                        {
                            name = $"{name}(已删除)";
                        }
                    }
                    item.Set("IsTm", isTm);

                    var isDeleteTm = false;
                    if (item.StatType == StatType.ExamTmDelete)
                    {
                        isDeleteTm = true;
                        isView = true;

                    }
                    item.Set("IsDeleteTm", isDeleteTm);

                    var isUser = false;
                    if (item.StatType == StatType.UserAdd || item.StatType == StatType.UserUpdate)
                    {
                        isUser = true;
                        if (await _databaseManager.UserRepository.ExistsAsync(item.ObjectId))
                        {
                            isView = true;
                            isEdit = true;
                        }
                        else
                        {
                            name = $"{name}(已删除)";
                        }
                    }
                    item.Set("IsUser", isUser);

                    var isAdmin = false;
                    if (item.StatType == StatType.AdminAdd || item.StatType == StatType.AdminUpdate)
                    {
                        isAdmin = true;
                        if (await _databaseManager.AdministratorRepository.ExistsAsync(item.ObjectId))
                        {
                            isView = true;
                            isEdit = true;
                        }
                        else
                        {
                            name = $"{name}(已删除)";
                        }
                    }
                    item.Set("IsAdmin", isAdmin);

                    var isExam = false;
                    if (item.StatType == StatType.ExamAdd || item.StatType == StatType.ExamUpdate)
                    {
                        isExam = true;
                        var examPaper = await _databaseManager.ExamPaperRepository.GetAsync(item.ObjectId);
                        if (examPaper != null)
                        {
                            isEdit = true;
                            if (examPaper.TmRandomType != ExamPaperTmRandomType.RandomExaming && examPaper.SubmitType == SubmitType.Submit)
                            {
                                isView = true;
                            }
                        }
                        else
                        {
                            name = $"{name}(已删除)";
                        }
                    }
                    item.Set("IsExam", isExam);

                    var isExamQ = false;
                    if (item.StatType == StatType.ExamQAdd || item.StatType == StatType.ExamQUpdate)
                    {
                        isExamQ = true;
                        if (await _databaseManager.ExamQuestionnaireRepository.ExistsAsync(item.ObjectId))
                        {
                            isView = true;
                            isEdit = true;
                        }
                        else
                        {
                            name = $"{name}(已删除)";
                        }
                    }
                    item.Set("IsExamQ", isExamQ);


                    var isExamCer = false;
                    if (item.StatType == StatType.ExamCerAdd || item.StatType == StatType.ExamCerUpdate)
                    {
                        isExamCer = true;
                        if (await _databaseManager.ExamCerRepository.ExistsAsync(item.ObjectId))
                        {
                            isView = true;
                            isEdit = true;
                        }
                        else
                        {
                            name = $"{name}(已删除)";
                        }
                    }
                    item.Set("IsExamCer", isExamCer);

                    item.Set("Name", name);
                    item.Set("IsView", isView);
                    item.Set("IsEdit", isEdit);
                }
            }
            return new GetLogResult
            {
                Total = total,
                List = list
            };
        }


        [HttpGet, Route(RouteGetData)]
        public async Task<ActionResult<GetDataResult>> GetData()
        {
            var auth = await _authManager.GetAuthorityAuth();


            var (c1, c2, c3, c4, c5) = await _databaseManager.OrganCompanyRepository.GetDataCount(auth);

            var (admin1, admin2, admin3, admin4, admin5) = await _databaseManager.AdministratorRepository.GetDataCount(auth);
            admin3 = await _statRepository.SumAsync(StatType.AdminDelete, auth);
            admin2 = admin1 + admin3;

            var (user1, user2, user3, user4, user5) = await _databaseManager.UserRepository.GetDataCount(auth);
            user3 = await _statRepository.SumAsync(StatType.UserDelete, auth);
            user2 = user1 + user3;

            var (e1, e2, e3, e4, e5) = await _databaseManager.ExamPaperRepository.GetDataCount(auth);
            e3 = await _statRepository.SumAsync(StatType.ExamDelete, auth);
            e2 = e1 + e3;

            var (q1, q2, q3, q4, q5) = await _databaseManager.ExamQuestionnaireRepository.GetDataCount(auth);
            q3 = await _statRepository.SumAsync(StatType.ExamQDelete, auth);
            q2 = q1 + q3;


            var (cer1, cer2, cer3, cer4, cer5) = await _databaseManager.ExamCerRepository.GetDataCount(auth);
            cer3 = await _statRepository.SumAsync(StatType.ExamCerDelete, auth);
            cer2 = cer1 + cer3;

            var (t1, t2, t3, t4, t5) = await _databaseManager.ExamTmRepository.GetDataCount(auth);
            t3 = await _statRepository.SumAsync(StatType.ExamTmDelete, auth);
            t2 = t1 + t3;

            var (plan1, plan2, plan3, plan4, plan5) = await _databaseManager.StudyPlanRepository.GetDataCount(auth);
            plan3 = await _statRepository.SumAsync(StatType.StudyPlanDelete, auth);
            plan2 = plan1 + plan3;

            var (course1, course2, course3, course4, course5) = await _databaseManager.StudyCourseRepository.GetDataCount(auth);
            course3 = await _statRepository.SumAsync(StatType.StudyCourseDelete, auth);
            course2 = course1 + course3;

            var (file1, file2, file3, file4, file5) = await _databaseManager.StudyCourseFilesRepository.GetDataCount(auth);
            file3 = await _statRepository.SumAsync(StatType.StudyFileDelete, auth);
            file2 = file1 + file3;

            var dataList = new List<GetDataInfo>();
            dataList.Add(new GetDataInfo
            {
                Name = "全部",
                Data = [plan1, course1, file1, e1, t1, cer1, q1, c1, admin1, user1]
            });
            dataList.Add(new GetDataInfo
            {
                Name = "新增",
                Data = [plan2, course2, file2, e2, t2, cer2, q2, c2, admin2, user2]
            });
            dataList.Add(new GetDataInfo
            {
                Name = "删除",
                Data = [plan3, course3, file3, e3, t3, cer3, q3, c3, admin3, user3]
            });
            dataList.Add(new GetDataInfo
            {
                Name = "停用",
                Data = [plan4, course4, file4, e4, t4, cer4, q4, c4, admin4, user4]
            });
            dataList.Add(new GetDataInfo
            {
                Name = "启用",
                Data = [plan5, course5, file5, e5, t5, cer5, q5, c5, admin5, user5]
            });

            return new GetDataResult
            {
                DataList = dataList,
                DataTitleList = new List<string> { "培训计划", "课程", "课件", "试卷", "题目", "证书", "问卷调查", "组织", "管理员账号", "用户账号" }
            };
        }
    }
}
