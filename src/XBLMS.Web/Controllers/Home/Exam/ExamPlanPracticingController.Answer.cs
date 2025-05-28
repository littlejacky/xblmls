using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Home.Exam
{
    public partial class ExamPlanPracticingController
    {
        [HttpPost, Route(RouteAnswer)]
        public async Task<ActionResult<GetSubmitAnswerResult>> Answer([FromBody] GetSubmitAnswerRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var tm = await _examTmRepository.GetAsync(request.Id);
            var tx = await _examTxRepository.GetAsync(tm.TxId);
            var examTmType = (tx.ExamTxBase == Enums.ExamTxBase.Tiankongti || tx.ExamTxBase == Enums.ExamTxBase.Jiandati) ? Enums.ExamTmType.Subjective : Enums.ExamTmType.Objective;

            var result = new GetSubmitAnswerResult
            {
                IsRight = false,
                Answer = tm.Answer,
                Jiexi = tm.Jiexi
            };

            if (examTmType == ExamTmType.Objective)
            {
                if (StringUtils.Equals(tm.Answer, request.Answer))
                {
                    result.IsRight = true;
                    result.Answer = string.Empty;
                    result.Jiexi = string.Empty;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tm.Answer))
                {
                    var answerList = ListUtils.GetStringList(tm.Answer);
                    var allTrue = true;
                    foreach (var answer in answerList)
                    {
                        if (!StringUtils.Contains(request.Answer, answer))
                        {
                            allTrue = false;
                        }
                    }
                    if (!allTrue)
                    {
                        answerList = ListUtils.GetStringList(tm.Answer, ";");
                        foreach (var answer in answerList)
                        {
                            if (!StringUtils.Contains(request.Answer, answer))
                            {
                                allTrue = false;
                            }
                        }
                    }
                    if (!allTrue)
                    {
                        answerList = ListUtils.GetStringList(tm.Answer, "，");
                        foreach (var answer in answerList)
                        {
                            if (!StringUtils.Contains(request.Answer, answer))
                            {
                                allTrue = false;
                            }
                        }
                    }
                    if (!allTrue)
                    {
                        answerList = ListUtils.GetStringList(tm.Answer, "；");
                        foreach (var answer in answerList)
                        {
                            if (!StringUtils.Contains(request.Answer, answer))
                            {
                                allTrue = false;
                            }
                        }
                    }
                    if (allTrue)
                    {
                        result.IsRight = true;
                        result.Answer = string.Empty;
                        result.Jiexi = string.Empty;
                    }
                }
            }

            if (!result.IsRight)
            {
                var wrong = await _examPracticeWrongRepository.GetAsync(user.Id);
                if (wrong != null)
                {
                    if (!wrong.TmIds.Contains(request.Id))
                    {
                        wrong.TmIds.Add(request.Id);
                        await _examPracticeWrongRepository.UpdateAsync(wrong);
                    }
                }
                else
                {
                    await _examPracticeWrongRepository.InsertAsync(new ExamPracticeWrong
                    {
                        UserId = user.Id,
                        TmIds = new List<int> { request.Id }
                    });
                }
            }

            //if (StringUtils.Equals(tm.Answer, request.Answer))
            //{
            //    result.IsRight = true;
            //    result.Answer = string.Empty;
            //    result.Jiexi = string.Empty;
            //}
            //else
            //{
            //    var wrong = await _examPracticeWrongRepository.GetAsync(user.Id);
            //    if (wrong != null)
            //    {
            //        if (!wrong.TmIds.Contains(request.Id))
            //        {
            //            wrong.TmIds.Add(request.Id);
            //            await _examPracticeWrongRepository.UpdateAsync(wrong);
            //        }
            //    }
            //    else
            //    {
            //        await _examPracticeWrongRepository.InsertAsync(new ExamPracticeWrong
            //        {
            //            UserId = user.Id,
            //            TmIds = new List<int> { request.Id }
            //        });
            //    }
            //}

            var record = await _examPlanAnswerRepository.GetAsync(user.Id, tm.Id, request.PracticeId);
            if (record == null)
            {
                await _examPlanAnswerRepository.InsertAsync(new ExamPlanAnswer
                {
                    UserId = user.Id,
                    PracticeId = request.PracticeId,
                    TmId = request.Id,
                    IsRight = result.IsRight,
                    Score = result.IsRight ? tm.Score : 0,
                    ExamTmType = examTmType
                });

                await _examPlanPracticeRepository.IncrementAnswerCountAsync(request.PracticeId);
                if (result.IsRight)
                {
                    await _examPlanPracticeRepository.IncrementRightCountAsync(request.PracticeId);
                }
            }
            else
            {
                if (!record.IsRight && result.IsRight)
                {
                    record.IsRight = result.IsRight;
                    record.Score = record.IsRight ? tm.Score : 0;
                    await _examPlanAnswerRepository.UpdateAsync(record);
                    await _examPlanPracticeRepository.IncrementRightCountAsync(request.PracticeId);
                }
                else if (record.IsRight && !result.IsRight)
                {
                    record.IsRight = result.IsRight;
                    record.Score = record.IsRight ? tm.Score : 0;
                    await _examPlanAnswerRepository.UpdateAsync(record);
                    await _examPlanPracticeRepository.DecrementRightCountAsync(request.PracticeId);
                }
            }

            return result;
        }

    }
}



