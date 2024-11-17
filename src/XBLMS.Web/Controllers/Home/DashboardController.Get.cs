using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Models;

namespace XBLMS.Web.Controllers.Home
{
    public partial class DashboardController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var user = await _authManager.GetUserAsync();

            if (user == null)
            {
                return Unauthorized();
            }

            user = await _organManager.GetUser(user.Id);

            var (allPassPercent, allTotal, moniPassPercent, moniTotal, passPercent, total) = await _examManager.AnalysisMorePass(user.Id);
            var (answerTmTotal, answerPercent, allTmTotal, allPercent, collectTmTotal, collectPercent, wrongTmTotal, wrongPercent) = await _examManager.AnalysisPractice(user.Id, user.CompanyId);

            var resultPaper = new ExamPaper();
            var resultMoni = new ExamPaper();

            var (paperTotal, paperList) = await _examPaperUserRepository.GetListAsync(user.Id, false, "", "", 1, 1);
            var (moniPaperTotal, moniPaperList) = await _examPaperUserRepository.GetListAsync(user.Id, true, "", "", 1, 1);

            if (paperTotal > 0)
            {
                var paperUser = paperList[0];
                var paper = await _examPaperRepository.GetAsync(paperUser.ExamPaperId);
                await _examManager.GetPaperInfo(paper, user, paperUser.PlanId, paperUser.CourseId);
                resultPaper = paper;
            }
            else
            {
                resultPaper = null;
            }
            if (moniPaperTotal > 0)
            {
                var paperUser = moniPaperList[0];
                var paper = await _examPaperRepository.GetAsync(paperUser.ExamPaperId);
                await _examManager.GetPaperInfo(paper, user, paperUser.PlanId, paperUser.CourseId);
                resultMoni = paper;
            }
            else
            {
                resultMoni = null;
            }



            var taskPaperTotal = 0;

            var taskPaperIds = await _examPaperUserRepository.GetPaperIdsByUser(user.Id);
            if (taskPaperIds != null && taskPaperIds.Count > 0)
            {
                foreach (var paperId in taskPaperIds)
                {
                    var paper = await _examPaperRepository.GetAsync(paperId);
                    var myExamTimes = await _examPaperStartRepository.CountAsync(paperId, user.Id);
                    if (myExamTimes <= 0 && (paper.ExamBeginDateTime.Value < DateTime.Now && paper.ExamEndDateTime.Value > DateTime.Now))
                    {
                        if (paper.Moni)
                        {

                        }
                        else
                        {
                            taskPaperTotal++;
                        }
                    }
                }

            }

            var qPaperTotal = 0;
            var qPaperIds = await _examQuestionnaireUserRepository.GetPaperIdsAsync(user.Id);
            if (qPaperIds != null && qPaperIds.Count > 0)
            {
                foreach (var qPaperId in qPaperIds)
                {
                    var paper = await _examQuestionnaireRepository.GetAsync(qPaperId);
                    if (paper != null)
                    {
                        if ((paper.ExamBeginDateTime.Value < DateTime.Now && paper.ExamEndDateTime.Value > DateTime.Now))
                        {
                            qPaperTotal++;
                        }
                    }

                }
            }


            var (courseTotal, courseList) = await _studyCourseRepository.User_GetPublicListAsync(user.CompanyId, "", "", "", 1, 3);
            if (courseTotal > 0)
            {
                foreach (var course in courseList)
                {
                    var courseUser = await _studyCourseUserRepository.GetAsync(user.Id, 0, course.Id);
                    await _studyManager.User_GetCourseInfoByCourseList(0, course, courseUser);
                }
            }

            var (planTotal, planList) = await _studyPlanUserRepository.GetListAsync(0, "", "", user.Id, 1, 1);
            var planUser = new StudyPlanUser();
            if (planTotal > 0)
            {
                planUser = planList[0];
                await _studyManager.User_GetPlanInfo(planUser);
            }

            var (planTotalCredit, planTotalOverCredit) = await _studyPlanUserRepository.GetCreditAsync(user.Id);
            var (totalCourse, totalOverCourse) = await _studyCourseUserRepository.GetTotalAsync(user.Id);
            var totalDuration = await _studyCourseUserRepository.GetTotalDurationAsync(user.Id);


            var topCer = new ExamCerUser();
            var (cerTotal, cerList) = await _examCerUserRepository.GetListAsync(user.Id, 1, 1);
            if (total > 0)
            {
                foreach (var item in cerList)
                {
                    var cerInfo = await _examCerRepository.GetAsync(item.CerId);
                    if (cerInfo != null)
                    {
                        item.Set("CerName", cerInfo.Name);
                        item.Set("CerOrganName", cerInfo.OrganName);
                    }
                    else
                    {
                        item.Set("CerName", "证书异常");
                    }
                    item.Set("AwartDate", item.CerDateTime.Value.ToString(DateUtils.FormatStringDateOnlyCN));
                    var paper = await _examPaperRepository.GetAsync(item.ExamPaperId);
                    if (paper != null)
                    {
                        item.Set("PaperName", paper.Title);
                    }
                    else
                    {
                        item.Set("PaperName", "试卷异常");
                    }
                    var start = await _examPaperStartRepository.GetAsync(item.ExamStartId);
                    if (start != null)
                    {
                        item.Set("PaperScore", start.Score);
                    }
                    else
                    {
                        item.Set("PaperScore", "成绩异常");
                    }
                    topCer = item;
                }
            }

            var planTask = await _studyPlanUserRepository.GetTaskCountAsync(user.Id);
            var (planCount, planOverCount) = await _studyPlanUserRepository.GetCountAsync(user.Id);

            var courseTask = await _studyCourseUserRepository.GetTaskCountAsync(user.Id);

            var dateStr = $"{DateTime.Now.ToString(DateUtils.FormatStringDateOnlyCN)} {DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-CN"))}";

            return new GetResult
            {
                User = user,
                AllPercent = allPassPercent,
                ExamTotal = total,
                ExamPercent = allPassPercent,
                ExamMoniPercent = moniPassPercent,
                ExamMoniTotal = moniTotal,

                ExamPaper = resultPaper,
                ExamMoni = resultMoni,

                PracticeAnswerTmTotal = answerTmTotal,
                PracticeAnswerPercent = answerPercent,
                PracticeAllTmTotal = allTmTotal,
                PracticeAllPercent = allPercent,
                PracticeCollectTmTotal = collectTmTotal,
                PracticeCollectPercent = collectPercent,
                PracticeWrongTmTotal = wrongTmTotal,
                PracticeWrongPercent = wrongPercent,

                TaskPaperTotal = taskPaperTotal,
                TaskQTotal = qPaperTotal,
                TaskPlanTotal = planTask,
                TaskCourseTotal = courseTask,

                CourseList = courseList,
                StudyPlan = planUser,

                StudyPlanTotalCredit = planTotalCredit,
                StudyPlanTotalOverCredit = planTotalOverCredit,
                TotalCourse = totalCourse,
                TotalOverCourse = totalOverCourse,

                TotalDuration = totalDuration,

                TopCer = topCer,

                PlanCount = planCount,
                PlanOverCount = planOverCount,

                DateStr = dateStr,
            };
        }
    }
}
