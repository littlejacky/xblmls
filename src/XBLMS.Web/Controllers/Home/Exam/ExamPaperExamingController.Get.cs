﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Home.Exam
{
    public partial class ExamPaperExamingController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> GetList([FromQuery] GetRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var paper = await _examPaperRepository.GetAsync(request.Id);

            if (paper == null) { return this.Error("未找到试卷"); }

            var randomId = 0;
            var startId = 0;
            var isNewExam = true;

            int useTimeSecond = 0;
            var noSubmitStart = await _examPaperStartRepository.GetNoSubmitAsync(paper.Id, user.Id, request.PlanId, request.CourseId);
            if (noSubmitStart != null)
            {
                randomId = noSubmitStart.ExamPaperRandomId;
                startId = noSubmitStart.Id;
                isNewExam = false;
                useTimeSecond = noSubmitStart.ExamTimeSeconds;

                var timeSpan = DateTime.Now - noSubmitStart.BeginDateTime.Value;
                var useTotalSecond = timeSpan.TotalSeconds + useTimeSecond;
                if (useTotalSecond >= paper.TimingMinute * 60)
                {
                    useTotalSecond = paper.TimingMinute * 60;
                    useTimeSecond = -1;
                }
                else
                {
                    useTimeSecond = (int)useTotalSecond;
                }

                noSubmitStart.ExamTimeSeconds = (int)useTotalSecond;
                await _examPaperStartRepository.UpdateAsync(noSubmitStart);
            }
            else
            {
                if (paper.TmRandomType == ExamPaperTmRandomType.RandomExaming)
                {

                    var auth = await _authManager.GetAuthorityAuth(paper.CreatorId);

                    randomId = await _examManager.SetExamPaperRantomByRandomNowAndExaming(paper, auth, user.Id);
                    // randomId = await _examPaperRandomRepository.GetOneIdByPaperAsync(paper.Id);
                }
                else
                {
                    randomId = await _examPaperRandomRepository.GetOneIdByPaperAsync(paper.Id);
                }


                startId = await _examPaperStartRepository.InsertAsync(new ExamPaperStart
                {
                    PlanId = request.PlanId,
                    CourseId = request.CourseId,
                    UserId = user.Id,
                    ExamPaperId = paper.Id,
                    ExamPaperRandomId = randomId,
                    BeginDateTime = DateTime.Now,
                    KeyWords = paper.Title,
                    KeyWordsAdmin = await _organManager.GetUserKeyWords(user.Id)
                });
            }


            if (randomId == 0) { return this.Error("试卷发布有问题，找不到任何题目，请联系管理员！"); }


            var configs = await _examPaperRandomConfigRepository.GetListAsync(paper.Id);
            if (configs == null || configs.Count == 0) { return this.Error("试卷发布有问题：找不到任何题目！"); }


            var paperTmTotal = 0;

            var tmIndex = 1;
            foreach (var config in configs)
            {
                var tx = await _examTxRepository.GetAsync(config.TxId);
                ExamTmType tmType = ExamTmType.Objective;
                if (tx.ExamTxBase == ExamTxBase.Tiankongti || tx.ExamTxBase == ExamTxBase.Jiandati)
                {
                    tmType = ExamTmType.Subjective;
                }

                var tms = await _examPaperRandomTmRepository.GetListAsync(randomId, config.TxId);
                if (tms != null && tms.Count > 0)
                {
                    paperTmTotal += tms.Count;
                    if (paper.IsExamingTmRandomView)
                    {
                        tms = tms.OrderBy(t => StringUtils.Guid()).ToList();
                    }
                    foreach (var item in tms)
                    {
                        if (isNewExam)
                        {
                            var examAnswer = new ExamPaperAnswer
                            {
                                UserId = user.Id,
                                ExamPaperId = paper.Id,
                                ExamStartId = startId,
                                RandomTmId = item.Id,
                                ExamTmType = tmType
                            };
                            examAnswer.Set("OptionsValues", new List<string>());
                            await _examPaperAnswerRepository.InsertAsync(examAnswer);
                        }

                        await _examManager.GetTmInfoByPaperUser(item, paper, startId);
                        item.Set("TmIndex", tmIndex);
                        tmIndex++;
                    }
                    config.Set("TmList", tms);
                }
            }

            paper.Set("TmTotal", paperTmTotal);
            paper.Set("StartId", startId);

            paper.Set("UserDisplayName", user.DisplayName);
            paper.Set("UserAvatar", user.AvatarUrl);

            paper.Set("UseTimeSecond", useTimeSecond);


            return new GetResult
            {
                Watermark = await _authManager.GetWatermark(),
                Item = paper,
                TxList = configs
            };
        }
    }
}
