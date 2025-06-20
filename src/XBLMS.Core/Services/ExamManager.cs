﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Core.Repositories;
using XBLMS.Core.Utils;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;

namespace XBLMS.Core.Services
{
    public partial class ExamManager : IExamManager
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IUserRepository _userRepository;
        private readonly IExamTmRepository _examTmRepository;
        private readonly IExamTxRepository _examTxRepository;
        private readonly IExamTmTreeRepository _examTmTreeRepository;
        private readonly IExamPaperTreeRepository _examPaperTreeRepository;
        private readonly IExamPaperRandomConfigRepository _examPaperRandomConfigRepository;
        private readonly IExamPaperRandomRepository _examPaperRandomRepository;
        private readonly IExamPaperRepository _examPaperRepository;
        private readonly IExamTmGroupRepository _examTmGroupRepository;
        private readonly IExamPaperRandomTmRepository _examPaperRandomTmRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IExamPaperUserRepository _examPaperUserRepository;
        private readonly IExamPaperStartRepository _examPaperStartRepository;
        private readonly IExamPaperAnswerRepository _examPaperAnswerRepository;
        private readonly IExamCerRepository _examCerRepository;
        private readonly IExamCerUserRepository _examCerUserRepository;
        private readonly IExamPracticeCollectRepository _examPracticeCollectRepository;
        private readonly IExamPracticeWrongRepository _examPracticeWrongRepository;
        private readonly IExamPracticeRepository _examPracticeRepository;
        private readonly IExamPlanPracticeRepository _examPlanPracticeRepository;

        private readonly IExamQuestionnaireRepository _examQuestionnaireRepository;
        private readonly IExamQuestionnaireAnswerRepository _examQuestionnaireAnswerRepository;
        private readonly IExamQuestionnaireTmRepository _examQuestionnaireTmRepository;
        private readonly IExamQuestionnaireUserRepository _examQuestionnaireUserRepository;

        private readonly IExamAssessmentRepository _examAssessmentRepository;
        private readonly IExamAssessmentUserRepository _examAssessmentUserRepository;
        private readonly IExamAssessmentTmRepository _examAssessmentTmRepository;
        private readonly IExamAssessmentAnswerRepository _examAssessmentAnswerRepository;
        private readonly IExamAssessmentConfigRepository _examAssessmentConfigRepository;
        private readonly IExamAssessmentConfigSetRepository _examAssessmentConfigSetRepository;

        private readonly IOrganManager _organManager;

        private readonly IStudyPlanRepository _studyPlanRepository;
        private readonly IStudyPlanCourseRepository _studyPlanCourseRepository;
        private readonly IStudyCourseRepository _studyCourseRepository;

        private readonly IExamPlanRepository _examPlanRepository;
        private readonly IExamPlanRecordRepository _examPlanRecordRepository;
        private readonly IExamTmGroupProportionRepository _examTmGroupProportionRepository;
        private readonly IAuthManager _authManager;
        private readonly INotificationManager _notificationManager;

        public ExamManager(ISettingsManager settingsManager,
            IOrganManager organManager,
            IPathManager pathManager,
            ICreateManager createManager,
            IExamTmRepository examTmRepository,
            IExamTxRepository examTxRepository,
            IExamTmTreeRepository examTmTreeRepository,
            IExamPaperTreeRepository examPaperTreeRepository,
            IExamPaperRandomConfigRepository examPaperRandomConfigRepository,
            IExamPaperRandomRepository examPaperRandomRepository,
            IExamPaperRepository examPaperRepository,
            IExamTmGroupRepository examTmGroupRepository,
            IExamPaperRandomTmRepository examPaperRandomTmRepository,
            IUserGroupRepository userGroupRepository,
            IUserRepository userRepository,
            IExamPaperUserRepository examPaperUserRepository,
            IExamPaperStartRepository examPaperStartRepository,
            IExamPaperAnswerRepository examPaperAnswerRepository,
            IExamCerRepository examCerRepository,
            IExamCerUserRepository examCerUserRepository,
            IExamPracticeCollectRepository examPracticeCollectRepository,
            IExamPracticeWrongRepository examPracticeWrongRepository,
            IExamPracticeRepository examPracticeRepository,
            IExamPlanPracticeRepository examPlanPracticeRepository,
            IExamQuestionnaireRepository examQuestionnaireRepository,
            IExamQuestionnaireAnswerRepository examQuestionnaireAnswerRepository,
            IExamQuestionnaireTmRepository examQuestionnaireTmRepository,
            IExamQuestionnaireUserRepository examQuestionnaireUserRepository,
            IExamAssessmentRepository examAssessmentRepository,
            IExamAssessmentUserRepository examAssessmentUserRepository,
            IExamAssessmentTmRepository examAssessmentTmRepository,
            IExamAssessmentAnswerRepository examAssessmentAnswerRepository,
            IExamAssessmentConfigRepository examAssessmentConfigRepository,
            IExamAssessmentConfigSetRepository examAssessmentConfigSetRepository,
            IStudyPlanCourseRepository studyPlanCourseRepository,
            IStudyPlanRepository studyPlanRepository,
            IStudyCourseRepository studyCourseRepository,
            IExamPlanRepository examPlanRepository,
            IExamPlanRecordRepository examPlanRecordRepository,
            IExamTmGroupProportionRepository examTmGroupProportionRepository,
            IAuthManager authManager,
            INotificationManager notificationManager)
        {
            _settingsManager = settingsManager;
            _organManager = organManager;
            _createManager = createManager;
            _pathManager = pathManager;
            _examTmRepository = examTmRepository;
            _examTxRepository = examTxRepository;
            _examTmTreeRepository = examTmTreeRepository;
            _examPaperTreeRepository = examPaperTreeRepository;
            _examPaperRandomConfigRepository = examPaperRandomConfigRepository;
            _examPaperRandomRepository = examPaperRandomRepository;
            _examPaperRepository = examPaperRepository;
            _examTmGroupRepository = examTmGroupRepository;
            _examPaperRandomTmRepository = examPaperRandomTmRepository;
            _userGroupRepository = userGroupRepository;
            _userRepository = userRepository;
            _examPaperUserRepository = examPaperUserRepository;
            _examPaperStartRepository = examPaperStartRepository;
            _examPaperAnswerRepository = examPaperAnswerRepository;
            _examCerRepository = examCerRepository;
            _examCerUserRepository = examCerUserRepository;
            _examPracticeCollectRepository = examPracticeCollectRepository;
            _examPracticeWrongRepository = examPracticeWrongRepository;
            _examPracticeRepository = examPracticeRepository;
            _examPlanPracticeRepository = examPlanPracticeRepository;
            _examQuestionnaireRepository = examQuestionnaireRepository;
            _examQuestionnaireAnswerRepository = examQuestionnaireAnswerRepository;
            _examQuestionnaireTmRepository = examQuestionnaireTmRepository;
            _examQuestionnaireUserRepository = examQuestionnaireUserRepository;
            _examAssessmentRepository = examAssessmentRepository;
            _examAssessmentUserRepository = examAssessmentUserRepository;
            _examAssessmentTmRepository = examAssessmentTmRepository;
            _examAssessmentAnswerRepository = examAssessmentAnswerRepository;
            _examAssessmentConfigRepository = examAssessmentConfigRepository;
            _examAssessmentConfigSetRepository = examAssessmentConfigSetRepository;
            _studyCourseRepository = studyCourseRepository;
            _studyPlanRepository = studyPlanRepository;
            _studyPlanCourseRepository = studyPlanCourseRepository;
            _examPlanRepository = examPlanRepository;
            _examPlanRecordRepository = examPlanRecordRepository;
            _examTmGroupProportionRepository = examTmGroupProportionRepository;
            _authManager = authManager;
            _notificationManager = notificationManager;
        }

        public async Task GetTmDeleteInfo(ExamTm tm)
        {
            tm.Set("Options", tm.Get("options"));
            tm.Set("OptionsValues", tm.Get("optionsValues"));

            tm.Set("TitleHtml", tm.Title);
            tm.Title = StringUtils.StripTags(tm.Title);

            tm.Set("NanduStar", $"{tm.Nandu}<i class='el-icon-star-on' style='color:#FF9900;margin-left:3px;font-size:14px;'></i>");

            var tx = await _examTxRepository.GetAsync(tm.TxId);
            if (tx != null)
            {
                tm.Set("TxName", tx.Name);
                tm.Set("TxTaxis", tx.Taxis);
                tm.Set("BaseTx", tx.ExamTxBase);
            }
            else
            {
                tm.Set("TxName", "题型不存在");
                tm.Set("TxTaxis", "题型不存在");
                tm.Set("BaseTx", "题型不存在");
            }

            var treeName = await _examTmTreeRepository.GetPathNamesAsync(tm.TreeId);
            tm.Set("TreeName", treeName);

        }
        public async Task<ExamTm> GetTmInfo(int tmId)
        {
            var tm = await _examTmRepository.GetAsync(tmId);
            tm.Set("Options", tm.Get("options"));
            tm.Set("optionsValues", tm.Get("optionsValues"));
            return tm;
        }
        public async Task GetBaseTmInfo(ExamTm tm)
        {
            tm.Set("Options", tm.Get("options"));
            tm.Set("OptionsValues", tm.Get("optionsValues"));

            tm.Set("TitleHtml", tm.Title);
            tm.Title = StringUtils.StripTags(tm.Title);

            tm.Set("NanduStar", $"{tm.Nandu}<i class='el-icon-star-on' style='color:#FF9900;margin-left:3px;font-size:14px;'></i>");

            var tx = await _examTxRepository.GetAsync(tm.TxId);
            tm.Set("TxName", tx.Name);
            tm.Set("TxTaxis", tx.Taxis);
            tm.Set("BaseTx", tx.ExamTxBase);


        }
        public async Task GetTmInfo(ExamTm tm)
        {
            await GetBaseTmInfo(tm);

            var treeName = await _examTmTreeRepository.GetPathNamesAsync(tm.TreeId);
            tm.Set("TreeName", treeName);

        }

        public async Task GetTmInfoByPaper(ExamTm tm)
        {
            await GetBaseTmInfo(tm);
            tm.Title = tm.Get("TitleHtml").ToString();

            var tx = await _examTxRepository.GetAsync(tm.TxId);

            var tmTitleList = new List<KeyValuePair<int, string>>();
            if (tx.ExamTxBase == ExamTxBase.Tiankongti && tm.Title.Contains("___"))
            {
                string newTitle = StringUtils.Replace(tm.Title, "___", "|___|");
                var tmTitleSplitList = ListUtils.GetStringList(newTitle, '|');
                var inputIndex = 0;
                for (int i = 0; i < tmTitleSplitList.Count; i++)
                {
                    if (tmTitleSplitList[i].Contains("___"))
                    {
                        tmTitleList.Add(new KeyValuePair<int, string>(inputIndex, tmTitleSplitList[i]));
                        inputIndex++;
                    }
                    else
                    {
                        tmTitleList.Add(new KeyValuePair<int, string>(i, tmTitleSplitList[i]));
                    }
                }
            }

            tm.Set("TitleList", tmTitleList);
        }



        public async Task GetTmInfoByPaperUser(ExamPaperRandomTm tm, ExamPaper paper, int startId, bool paperView = false)
        {
            await GetTmInfoByPaper(tm);

            var optionsRandom = new List<KeyValuePair<string, string>>();
            var options = ListUtils.ToList(tm.Get("options"));
            var abcList = StringUtils.GetABC();
            for (var i = 0; i < options.Count; i++)
            {
                optionsRandom.Add(new KeyValuePair<string, string>(abcList[i], options[i]));
            }
            if (paper.IsExamingTmOptionRandomView && !paperView)
            {
                optionsRandom = ListUtils.GetRandomList(optionsRandom);
            }
            tm.Set("OptionsRandom", optionsRandom);

            var answerStatus = true;
            var answer = await _examPaperAnswerRepository.GetAsync(tm.Id, startId, paper.Id);
            if (string.IsNullOrWhiteSpace(answer.Answer))
            {
                answerStatus = false;
            }


            tm.Set("AnswerInfo", answer);
            tm.Set("AnswerStatus", answerStatus);

            if (paperView)
            {
                tm.Set("IsRight", StringUtils.Equals(tm.Answer, answer.Answer) || answer.Score > 0);
                if (!paper.SecrecyPaperAnswer)
                {
                    tm.Answer = "";
                }
            }
            else
            {
                tm.Answer = "";
            }


        }
        public async Task GetTmInfoByPaperViewAdmin(ExamPaperRandomTm tm, ExamPaper paper, int startId)
        {
            await GetTmInfoByPaper(tm);

            var optionsRandom = new List<KeyValuePair<string, string>>();
            var options = ListUtils.ToList(tm.Get("options"));
            var abcList = StringUtils.GetABC();
            for (var i = 0; i < options.Count; i++)
            {
                optionsRandom.Add(new KeyValuePair<string, string>(abcList[i], options[i]));
            }

            tm.Set("OptionsRandom", optionsRandom);

            var answerStatus = true;
            var answer = await _examPaperAnswerRepository.GetAsync(tm.Id, startId, paper.Id);
            if (string.IsNullOrWhiteSpace(answer.Answer))
            {
                answerStatus = false;
            }

            tm.Set("AnswerInfo", answer);
            tm.Set("AnswerStatus", answerStatus);
            tm.Set("IsRight", StringUtils.Equals(tm.Answer, answer.Answer) || answer.Score > 0);

        }
        public async Task GetTmInfoByPaperMark(ExamPaperRandomTm tm, ExamPaper paper, int startId)
        {
            await GetTmInfoByPaper(tm);

            var optionsRandom = new List<KeyValuePair<string, string>>();
            var options = ListUtils.ToList(tm.Get("options"));
            var abcList = StringUtils.GetABC();
            for (var i = 0; i < options.Count; i++)
            {
                optionsRandom.Add(new KeyValuePair<string, string>(abcList[i], options[i]));
            }

            tm.Set("OptionsRandom", optionsRandom);

            var answer = await _examPaperAnswerRepository.GetAsync(tm.Id, startId, paper.Id);

            var answerStatue = answer.Get("MarkState");

            var markState = false;
            if (answerStatue != null)
            {
                markState = TranslateUtils.ToBool(answer.Get("MarkState").ToString());
            }

            tm.Set("AnswerInfo", answer);
            tm.Set("MarkState", markState);
            tm.Set("IsRight", StringUtils.Equals(tm.Answer, answer.Answer) || answer.Score > 0);

        }

        public async Task GetTmInfoByPracticing(ExamTm tm)
        {
            await GetTmInfoByPaper(tm);

            var optionsRandom = new List<KeyValuePair<string, string>>();
            var options = ListUtils.ToList(tm.Get("options"));
            var abcList = StringUtils.GetABC();
            for (var i = 0; i < options.Count; i++)
            {
                optionsRandom.Add(new KeyValuePair<string, string>(abcList[i], options[i]));
            }

            tm.Set("OptionsRandom", optionsRandom);
            tm.Answer = "";
            tm.Set("OptionsValues", new List<string>());
        }
        public async Task GetPaperInfo(ExamPaper paper, User user, int planId, int courseId, bool cjList = false)
        {
            var myExamTimes = await _examPaperStartRepository.CountAsync(paper.Id, user.Id, planId, courseId);
            var startId = await _examPaperStartRepository.GetNoSubmitIdAsync(paper.Id, user.Id, planId, courseId);
            var cerName = "";
            if (paper.CerId > 0)
            {
                var cer = await _examCerRepository.GetAsync(paper.CerId);
                if (cer != null)
                {
                    cerName = cer.Name;
                }
            }

            var courseName = "培训计划：xxxxx";

            var examUser = await _examPaperUserRepository.GetAsync(paper.Id, user.Id, planId, courseId);
            if (courseId > 0 || planId > 0)
            {
                if (examUser == null)
                {

                    var adminKeyWords = await _organManager.GetUserKeyWords(user.Id);

                    if (planId > 0)
                    {
                        var plan = await _studyPlanRepository.GetAsync(planId);
                        if (plan != null)
                        {
                            adminKeyWords = $"{adminKeyWords}-{plan.PlanName}";
                        }

                    }
                    if (courseId > 0)
                    {
                        var course = await _studyPlanCourseRepository.GetAsync(planId, courseId);
                        if (course != null)
                        {
                            adminKeyWords = $"{adminKeyWords}-{course.CourseName}";
                        }
                    }

                    var examUserId = await _examPaperUserRepository.InsertAsync(new ExamPaperUser
                    {
                        PlanId = planId,
                        CourseId = courseId,
                        ExamTimes = paper.ExamTimes,
                        ExamBeginDateTime = paper.ExamBeginDateTime,
                        ExamEndDateTime = paper.ExamEndDateTime,
                        ExamPaperId = paper.Id,
                        UserId = user.Id,
                        KeyWordsAdmin = await _organManager.GetUserKeyWords(user.Id),
                        KeyWords = paper.Title,
                        Locked = paper.Locked,
                        Moni = paper.Moni,
                        CompanyId = user.CompanyId,
                        DepartmentId = user.DepartmentId,
                        CreatorId = user.CreatorId
                    });
                    examUser = await _examPaperUserRepository.GetAsync(examUserId);
                }
                if (planId > 0)
                {
                    if (courseId > 0)
                    {
                        var course = await _studyPlanCourseRepository.GetAsync(planId, courseId);
                        if (course != null)
                        {
                            courseName = course.CourseName;
                        }
                    }
                    else
                    {
                        var plan = await _studyPlanRepository.GetAsync(planId);
                        if (plan != null)
                        {
                            courseName = plan.PlanName;
                        }
                    }
                }
                else
                {
                    if (courseId > 0)
                    {
                        var course = await _studyCourseRepository.GetAsync(courseId);
                        if (course != null)
                        {
                            courseName = course.Name;
                        }
                    }
                }

            }
            paper.Set("CourseName", courseName);
            paper.Set("ExamUserId", examUser.Id);
            paper.Set("PlanId", planId);
            paper.Set("CourseId", courseId);
            paper.Set("StartId", startId);
            paper.Set("CerName", cerName);
            paper.Set("ExamStartDateTimeStr", DateUtils.Format(examUser.ExamBeginDateTime, DateUtils.FormatStringDateTimeCN));
            paper.Set("ExamEndDateTimeStr", DateUtils.Format(examUser.ExamEndDateTime, DateUtils.FormatStringDateTimeCN));

            paper.Set("MyExamTimes", myExamTimes > examUser.ExamTimes ? examUser.ExamTimes : myExamTimes);
            paper.Set("UserExamTimes", examUser.ExamTimes);

            double longTime = 0;
            if (examUser.ExamBeginDateTime.Value > DateTime.Now)
            {
                var timeSpan = DateUtils.DateTimeToUnixTimestamp(examUser.ExamBeginDateTime.Value);
                longTime = timeSpan;
            }
            paper.Set("ExamStartDateTimeLong", longTime);

            var taskStartIds = _createManager.GetTaskStartIds();
            if (taskStartIds.Contains(startId))
            {
                paper.Set("ExamSubmiting", true);
            }
            else
            {
                paper.Set("ExamSubmiting", false);
            }
            if (cjList)
            {
                var cjlist = await _examPaperStartRepository.GetListAsync(paper.Id, user.Id, planId, courseId);
                if (cjlist != null && cjlist.Count > 0)
                {
                    foreach (var cj in cjlist)
                    {
                        if (!paper.SecrecyScore)
                        {
                            cj.Score = 0;
                        }
                        cj.Set("UseTime", DateUtils.SecondToHms(cj.ExamTimeSeconds));

                    }
                }
                paper.Set("CjList", cjlist);
            }
        }
        public async Task GetPaperInfo(ExamPaper paper, User user, bool cjList = false)
        {
            var myExamTimes = await _examPaperStartRepository.CountAsync(paper.Id, user.Id);
            var startId = await _examPaperStartRepository.GetNoSubmitIdAsync(paper.Id, user.Id);
            var cerName = "";
            if (paper.CerId > 0)
            {
                var cer = await _examCerRepository.GetAsync(paper.CerId);
                if (cer != null)
                {
                    cerName = cer.Name;
                }
            }

            var examUser = await _examPaperUserRepository.GetAsync(paper.Id, user.Id);


            paper.Set("StartId", startId);
            paper.Set("CerName", cerName);
            paper.Set("ExamStartDateTimeStr", DateUtils.Format(examUser.ExamBeginDateTime, DateUtils.FormatStringDateTimeCN));
            paper.Set("ExamEndDateTimeStr", DateUtils.Format(examUser.ExamEndDateTime, DateUtils.FormatStringDateTimeCN));

            paper.Set("MyExamTimes", myExamTimes > examUser.ExamTimes ? examUser.ExamTimes : myExamTimes);
            paper.Set("UserExamTimes", examUser.ExamTimes);

            double longTime = 0;
            if (examUser.ExamBeginDateTime.Value > DateTime.Now)
            {
                var timeSpan = DateUtils.DateTimeToUnixTimestamp(examUser.ExamBeginDateTime.Value);
                longTime = timeSpan;
            }
            paper.Set("ExamStartDateTimeLong", longTime);

            var taskStartIds = _createManager.GetTaskStartIds();
            if (taskStartIds.Contains(startId))
            {
                paper.Set("ExamSubmiting", true);
            }
            else
            {
                paper.Set("ExamSubmiting", false);
            }
            if (cjList)
            {
                var cjlist = await _examPaperStartRepository.GetListAsync(paper.Id, user.Id);
                if (cjlist != null && cjlist.Count > 0)
                {
                    foreach (var cj in cjlist)
                    {
                        if (!paper.SecrecyScore)
                        {
                            cj.Score = 0;
                        }
                        cj.Set("UseTime", DateUtils.SecondToHms(cj.ExamTimeSeconds));

                    }
                }
                paper.Set("CjList", cjlist);
            }
        }
        public async Task GetPaperInfo(ExamPaper paper, User user, ExamPaperStart start)
        {
            var myExamTimes = await _examPaperStartRepository.CountAsync(paper.Id, user.Id);
            var startId = await _examPaperStartRepository.GetNoSubmitIdAsync(paper.Id, user.Id);
            var cerName = "";
            if (paper.CerId > 0)
            {
                var cer = await _examCerRepository.GetAsync(paper.CerId);
                if (cer != null)
                {
                    cerName = cer.Name;
                }
            }

            var examUser = await _examPaperUserRepository.GetAsync(paper.Id, user.Id);


            paper.Set("StartId", startId);
            paper.Set("CerName", cerName);
            paper.Set("ExamStartDateTimeStr", DateUtils.Format(start.BeginDateTime, DateUtils.FormatStringDateTimeCN));
            paper.Set("ExamEndDateTimeStr", DateUtils.Format(start.EndDateTime, DateUtils.FormatStringDateTimeCN));

            paper.Set("MyExamTimes", myExamTimes > examUser.ExamTimes ? examUser.ExamTimes : myExamTimes);
            paper.Set("UserExamTimes", examUser.ExamTimes);
        }
        public async Task GetQuestionnaireInfo(ExamQuestionnaire paper, User user)
        {
            var paperUser = await _examQuestionnaireUserRepository.GetAsync(paper.Id, user.Id);
            paper.Set("ExamStartDateTimeStr", DateUtils.Format(paper.ExamBeginDateTime, DateUtils.FormatStringDateTimeCN));
            paper.Set("ExamEndDateTimeStr", DateUtils.Format(paper.ExamEndDateTime, DateUtils.FormatStringDateTimeCN));
            paper.Set("SubmitType", paperUser.SubmitType);
            paper.Set("State", DateTime.Now >= paper.ExamBeginDateTime && DateTime.Now <= paper.ExamEndDateTime);
        }
        public void GetExamAssessmentInfo(ExamAssessment ass, ExamAssessmentUser assUser, User user)
        {
            ass.Set("ExamStartDateTimeStr", DateUtils.Format(ass.ExamBeginDateTime, DateUtils.FormatStringDateTimeCN));
            ass.Set("ExamEndDateTimeStr", DateUtils.Format(ass.ExamEndDateTime, DateUtils.FormatStringDateTimeCN));
            ass.Set("SubmitType", assUser.SubmitType);
            ass.Set("State", DateTime.Now >= ass.ExamBeginDateTime.Value && DateTime.Now <= ass.ExamEndDateTime.Value);
            ass.Set("ConfigId", assUser.ConfigId);
            ass.Set("ConfigName", assUser.ConfigName);
        }
        public async Task<(bool Success, string msg)> CheckExam(int paperId, int userId)
        {
            var paper = await _examPaperRepository.GetAsync(paperId);
            var paperUser = await _examPaperUserRepository.GetAsync(paperId, userId);
            if (paper != null || paperUser != null)
            {
                var myTimes = await _examPaperStartRepository.CountAsync(paperId, userId);
                var times = paperUser.ExamTimes;
                if (times - myTimes <= 0)
                {
                    return (false, "剩余考试次数不足");
                }

                if (paperUser.ExamBeginDateTime.Value > DateTime.Now)
                {
                    return (false, "考试未开始，请耐心等待");
                }

                if (paperUser.ExamEndDateTime.Value < DateTime.Now)
                {
                    return (false, "考试已过期");
                }
            }
            else
            {
                return (false, "未找到试卷");
            }

            return (true, "");
        }
        public async Task<(bool Success, string msg)> CheckExam(int paperId, int userId, int planId, int courseId)
        {
            var paper = await _examPaperRepository.GetAsync(paperId);
            var paperUser = await _examPaperUserRepository.GetAsync(paperId, userId, planId, courseId);
            if (paper != null || paperUser != null)
            {
                var myTimes = await _examPaperStartRepository.CountAsync(paperId, userId, planId, courseId);
                var times = paperUser.ExamTimes;
                if (times - myTimes <= 0)
                {
                    return (false, "剩余考试次数不足");
                }

                if (paperUser.ExamBeginDateTime.Value > DateTime.Now)
                {
                    return (false, "考试未开始，请耐心等待");
                }
            }
            else
            {
                return (false, "未找到试卷");
            }

            return (true, "");
        }
    }
}
