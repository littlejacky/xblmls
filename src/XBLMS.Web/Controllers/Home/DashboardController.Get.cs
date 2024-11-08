using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Configuration;
using XBLMS.Core.Utils;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

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

            var paperIds = await _examPaperUserRepository.GetPaperIdsByUser(user.Id, "");
            var (paperTotal, paperList) = await _examPaperRepository.GetListByUserAsync(paperIds, "", 1, 1);
            var (moniPaperTotal, moniPaperList) = await _examPaperRepository.GetListByUserAsync(paperIds, "", 1, 1, true);

            if (paperTotal > 0)
            {
                resultPaper = paperList[0];
                await _examManager.GetPaperInfo(resultPaper, user);
            }
            else
            {
                resultPaper = null;
            }
            if (moniPaperTotal > 0)
            {
                resultMoni = moniPaperList[0];
                await _examManager.GetPaperInfo(resultMoni, user);
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


            var (courseTotal, courseList) = await _studyCourseRepository.User_GetPublicListAsync("", "", "", 1, 3);
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

                CourseList = courseList,
                StudyPlan = planUser,

                StudyPlanTotalCredit = planTotalCredit,
                StudyPlanTotalOverCredit = planTotalOverCredit,
                TotalCourse = totalCourse,
                TotalOverCourse = totalOverCourse,

                TotalDuration = totalDuration,

                TopCer = topCer
            };
        }
    }
}
