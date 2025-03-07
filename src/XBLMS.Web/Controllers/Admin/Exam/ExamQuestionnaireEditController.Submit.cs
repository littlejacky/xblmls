﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamQuestionnaireEditController
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
            var paper = request.Item;


            if (paper.Id > 0)
            {

                if (request.SubmitType == SubmitType.Submit)
                {
                    paper.SubmitType = request.SubmitType;

                    await _examManager.ClearQuestionnaire(paper.Id);

                    await _examManager.ArrangeQuestionnaire(auth, paper);

                    await _examManager.SetQuestionnairTm(request.TmList, paper.Id);

                    await _authManager.AddAdminLogAsync("重新发布调查问卷",paper.Title);
                    await _authManager.AddStatLogAsync(StatType.ExamQUpdate, "重新发布调查问卷", paper.Id, paper.Title);

                }
                else
                {
                    await _authManager.AddAdminLogAsync("修改调查问卷", paper.Title);
                    await _authManager.AddStatLogAsync(StatType.ExamQUpdate, "修改调查问卷", paper.Id, paper.Title);
                }



                if (!paper.Published)
                {
                    await _questionnaireUserRepository.UpdateKeyWordsAsync(paper.Id, paper.Title);
                    await _questionnaireUserRepository.UpdateExamDateTimeAsync(paper.Id, paper.ExamBeginDateTime.Value, paper.ExamEndDateTime.Value);
                }
          

                await _questionnaireRepository.UpdateAsync(paper);

            }
            else
            {
                paper.SubmitType = request.SubmitType;
                paper.CompanyId = auth.CompanyId;
                paper.CreatorId = auth.AdminId;
                paper.DepartmentId = auth.DepartmentId;

                var paperId = await _questionnaireRepository.InsertAsync(paper);
                paper.Id = paperId;

                await _examManager.SetQuestionnairTm(request.TmList, paperId);


                if (request.SubmitType == SubmitType.Submit)
                {
                    await _examManager.ArrangeQuestionnaire(auth, paper);

                    await _authManager.AddAdminLogAsync("发布问卷调查",paper.Title);
                    await _authManager.AddStatLogAsync(StatType.ExamQAdd, "发布问卷调查", paper.Id, paper.Title);
                    await _authManager.AddStatCount(StatType.ExamQAdd);
                }
                else
                {
                    await _authManager.AddAdminLogAsync("新增问卷调查", paper.Title);
                    await _authManager.AddStatLogAsync(StatType.ExamQAdd, "新增问卷调查", paper.Id, paper.Title);
                    await _authManager.AddStatCount(StatType.ExamQAdd);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
