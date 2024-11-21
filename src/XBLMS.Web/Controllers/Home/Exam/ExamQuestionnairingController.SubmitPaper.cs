using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Home.Exam
{
    public partial class ExamQuestionnairingController
    {
        [HttpPost, Route(RouteSubmitPaper)]
        public async Task<ActionResult<BoolResult>> SubmitPaper([FromBody] GetSubmitRequest request)
        {
            var paper = await _examQuestionnaireRepository.GetAsync(request.Id);
            if (paper == null)
            {
                return this.Error("无效的问卷调查");
            }

            var user = new User();
            user.Id = 0;

            if (!paper.Published)
            {
                user = await _authManager.GetUserAsync();

                var paperUser = await _examQuestionnaireUserRepository.GetAsync(paper.Id, user.Id, request.PlanId, request.CourseId);
                if (paperUser == null)
                {
                    await _examQuestionnaireUserRepository.InsertAsync(new ExamQuestionnaireUser
                    {
                        PlanId = request.PlanId,
                        CourseId = request.CourseId,
                        ExamPaperId = paper.Id,
                        UserId = user.Id,
                        KeyWords = paper.Title,
                        KeyWordsAdmin = await _organManager.GetUserKeyWords(user.Id),
                        Locked = paper.Locked,
                        ExamBeginDateTime = paper.ExamBeginDateTime,
                        ExamEndDateTime = paper.ExamEndDateTime,
                        CompanyId = user.CompanyId,
                        DepartmentId = user.DepartmentId,
                        CreatorId = user.CreatorId,
                        SubmitType = SubmitType.Submit
                    });
                }
                else
                {
                    paperUser.SubmitType = SubmitType.Submit;
                    await _examQuestionnaireUserRepository.UpdateAsync(paperUser);
                }
            }


            if (request.TmList != null && request.TmList.Count > 0)
            {
                foreach (var item in request.TmList)
                {
                    await _examQuestionnaireAnswerRepository.InsertAsync(new ExamQuestionnaireAnswer
                    {
                        PlanId = request.PlanId,
                        CourseId = request.CourseId,
                        TmId = item.Id,
                        Answer = item.Get("Answer").ToString(),
                        ExamPaperId = request.Id,
                        UserId = user.Id
                    });
                }
            }


            await _examQuestionnaireRepository.IncrementAsync(request.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
