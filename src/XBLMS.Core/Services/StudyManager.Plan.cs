using Datory;
using System;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Enums;
using XBLMS.Models;

namespace XBLMS.Core.Services
{
    public partial class StudyManager
    {
        public async Task User_GetPlanInfo(StudyPlanUser planUser)
        {
            var studyPlan = await _studyPlanRepository.GetAsync(planUser.PlanId);
            studyPlan.Set("PlanBeginDateTimeStr", studyPlan.PlanBeginDateTime.Value.ToString(DateUtils.FormatStringDateOnlyCN));
            studyPlan.Set("PlanEndDateTimeStr", studyPlan.PlanEndDateTime.Value.ToString(DateUtils.FormatStringDateOnlyCN));

            var planCourseTotal = await _studyPlanCourseRepository.CountAsync(studyPlan.Id, false);
            var planSelectCourseTotal = await _studyPlanCourseRepository.CountAsync(studyPlan.Id, false);

            studyPlan.Set("CourseTotal", planCourseTotal);
            studyPlan.Set("SelectCourseTotal", planSelectCourseTotal);


            var overCourseTotal = 0;
            var overSelectCourseTotal = 0;

            var planUserCourseList = await _studyCourseUserRepository.GetListAsync(studyPlan.Id, planUser.UserId);
            if (planUserCourseList != null && planUserCourseList.Count > 0)
            {
                foreach (var planUserCourse in planUserCourseList)
                {
                    if (planUserCourse.State == StudyStatType.Yiwancheng || planUserCourse.State == StudyStatType.Yidabiao)
                    {
                        var planCourse = await _studyPlanCourseRepository.GetAsync(studyPlan.Id, planUserCourse.CourseId);
                        if (planCourse.IsSelectCourse)
                        {
                            overSelectCourseTotal++;
                        }
                        else
                        {
                            overCourseTotal++;
                        }
                    }

                }
            }
            if (overCourseTotal == planCourseTotal)
            {
                planUser.State = StudyStatType.Yiwancheng;
            }
            if (planUser.TotalCredit >= studyPlan.PlanCredit)
            {
                planUser.State = StudyStatType.Yidabiao;
            }

            planUser.Set("OverCourseTotal", overCourseTotal);
            planUser.Set("OverSelectCourseTotal", overSelectCourseTotal);

            var isStudy = true;
            if (studyPlan.PlanBeginDateTime.Value > DateTime.Now || studyPlan.PlanEndDateTime.Value < DateTime.Now)
            {
                isStudy = false;
            }
            planUser.Set("IsStudy", isStudy);
            planUser.Set("Plan", studyPlan);
        }
    }
}
