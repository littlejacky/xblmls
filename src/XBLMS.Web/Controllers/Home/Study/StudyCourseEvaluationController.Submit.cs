using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Home.Study
{
    public partial class StudyCourseEvaluationController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] GetSubmitRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            if (request.List != null && request.List.Count > 0)
            {
                var totalStar = 0;
                var starItemCount = 0;
                foreach (var item in request.List)
                {
                    if (!item.TextContent)
                    {
                        starItemCount++;
                    }
                    var starValue = TranslateUtils.ToInt(item.Get("StarValue").ToString());
                    totalStar += starValue;
                    await _studyCourseEvaluationItemUserRepository.InsertAsync(new StudyCourseEvaluationItemUser
                    {
                        PlanId = request.PlanId,
                        CourseId = request.CourseId,
                        EvaluationId = request.EId,
                        EvaluationItemId = item.Id,
                        UserId = user.Id,
                        StarValue = starValue,
                        TextContent = item.Get("StrValue").ToString(),
                        KeyWordsAdmin = await _organManager.GetUserKeyWords(user.Id),
                        CompanyId = user.CompanyId,
                        DepartmentId = user.DepartmentId,
                        CreatorId = user.Id,
                    });
                }
                await _studyCourseEvaluationUserRepository.InsertAsync(new StudyCourseEvaluationUser
                {
                    PlanId = request.PlanId,
                    CourseId = request.CourseId,
                    EvaluationId = request.EId,
                    UserId = user.Id,
                    KeyWordsAdmin = await _organManager.GetUserKeyWords(user.Id),
                    CompanyId = user.CompanyId,
                    DepartmentId = user.DepartmentId,
                    CreatorId = user.Id,
                    TotalStarValue = totalStar

                });
    

                var courseUserInfo = await _studyCourseUserRepository.GetAsync(user.Id, request.PlanId, request.CourseId);
                courseUserInfo.TotalEvaluation = totalStar;
                courseUserInfo.AvgEvaluation = totalStar / starItemCount;
                await _studyCourseUserRepository.UpdateAsync(courseUserInfo);

                var courseInfo = await _studyCourseRepository.GetAsync(request.CourseId);
                courseInfo.TotaEvaluationlUser++;
                courseInfo.TotalEvaluation += totalStar;
                courseInfo.TotalAvgEvaluation += courseUserInfo.AvgEvaluation;
                await _studyCourseRepository.UpdateAsync(courseInfo);
            }

            return new BoolResult()
            {
                Value = true
            };
        }

    }
}
