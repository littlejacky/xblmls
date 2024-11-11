using Datory;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyPlanManagerController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] IdRequest request)
        {
            var auth = await _authManager.GetAuthorityAuth();

            var plan = await _studyPlanRepository.GetAsync(request.Id);
            var courseCount = await _studyPlanCourseRepository.CountAsync(plan.Id, false);
            var courseSelectCount = await _studyPlanCourseRepository.CountAsync(plan.Id, true);
            var courseTotal = courseCount + courseSelectCount;

            var userTotal = await _studyPlanUserRepository.GetCountAsync(plan.Id, "");
            var userPassTotal = await _studyPlanUserRepository.GetCountAsync(plan.Id, StudyStatType.Yiwancheng.GetValue());
            var userPass1Total= await _studyPlanUserRepository.GetCountAsync(plan.Id, StudyStatType.Yidabiao.GetValue());
            var totalCredit = await _studyPlanUserRepository.GetTotalCreditAsync(plan.Id);
            var avgCredit = TranslateUtils.ToAvg((double)totalCredit, (double)userTotal);

            var courseCreditTotal = await _studyPlanCourseRepository.GetTotalCreditAsync(plan.Id, false);
            var courseSelectCreditTotal = await _studyPlanCourseRepository.GetTotalCreditAsync(plan.Id, true);

            var overCourseUser = await _studyCourseUserRepository.GetOverCountAsync(plan.Id, false);
            var overSelectCourseUser = await _studyCourseUserRepository.GetOverCountAsync(plan.Id, true);

            var overCourseCreditTotal = await _studyCourseUserRepository.GetOverTotalCreditAsync(plan.Id, false);
            var overSelectCourseCreditTotal = await _studyCourseUserRepository.GetOverTotalCreditAsync(plan.Id, true);

            plan.Set("AvgCredit", avgCredit);
            plan.Set("TotalUser", userTotal);
            plan.Set("TotalPassUser", userPassTotal);
            plan.Set("TotalPass1User", userPass1Total);
            plan.Set("TotalCourse", courseTotal);
            plan.Set("CountCourse", courseCount);
            plan.Set("CountSelectCourse", courseSelectCount);

            plan.Set("TotalOverCourseUser", overCourseUser);
            plan.Set("AvgCourseCredit", TranslateUtils.ToAvg((double)overCourseCreditTotal, (double)userTotal));
            plan.Set("TotalOverSelectCourseUser", overSelectCourseUser);
            plan.Set("AvgOverSelectCredit", TranslateUtils.ToAvg((double)overSelectCourseCreditTotal, (double)userTotal));

            plan.Set("TotalCreditCourse", courseCreditTotal);
            plan.Set("TotalCreditSelectCourse", courseSelectCreditTotal);

            return new GetResult
            {
                Item = plan,
            };

        }

    }
}
