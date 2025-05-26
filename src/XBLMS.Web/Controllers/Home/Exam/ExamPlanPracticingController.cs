using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
using XBLMS.Configuration;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Web.Controllers.Home.Exam
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ExamPlanPracticingController : ControllerBase
    {
        private const string Route = "exam/examPlanPracticing";
        private const string RouteTm = Route + "/tm";
        private const string RouteAnswer = Route + "/answer";
        private const string RouteCollection = Route + "/collection";
        private const string RouteCollectionRemove = Route + "/collectionRemove";
        private const string RouteWrongRemove = Route + "/wrongRemove";
        private const string RouteSubmitTiming = Route + "/submitTiming";
        private const string RouteSubmit = Route + "/submit";


        private const string RoutePricticingTmIds = Route+ "/tmids";

        private const string RoutePricticingSubmit = Route+"/submit";

     
        private const string RouteCollectionSubmit = Route + "/collectionSubmit";

        private const string RouteError = Route + "home/exam/practice/error";
        private const string RouteErrorDel = Route + "home/exam/practice/error/del";


        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly IDatabaseManager _databaseManager;
        private readonly IAdministratorRepository _adminRepository;
        private readonly IExamTmRepository _examTmRepository;
        private readonly IExamPlanRecordRepository _examPlanRecordRepository;
        private readonly IExamPlanPracticeRepository _examPlanPracticeRepository;
        private readonly IExamTxRepository _examTxRepository;
        private readonly IExamManager _examManager;
        private readonly IExamPlanAnswerRepository _examPlanAnswerRepository;
        private readonly IExamPracticeCollectRepository _examPracticeCollectRepository;
        private readonly IExamPracticeWrongRepository _examPracticeWrongRepository;
        

        public ExamPlanPracticingController(IAuthManager authManager,
            IConfigRepository configRepository,
            IDatabaseManager databaseManager,
            IExamManager examManager,
            IAdministratorRepository administratorRepository,
            IExamPlanRecordRepository examPlanRecordRepository,
            IExamPlanPracticeRepository examPlanPracticeRepository,
            IExamTmRepository examTmRepository,
            IExamTxRepository examTxRepository,
            IExamPlanAnswerRepository examPlanAnswerRepository,
            IExamPracticeCollectRepository examPracticeCollectRepository,
            IExamPracticeWrongRepository examPracticeWrongRepository)
        {
            _authManager = authManager;
            _examManager = examManager;
            _configRepository = configRepository;
            _databaseManager = databaseManager;
            _adminRepository = administratorRepository;
            _examTmRepository = examTmRepository;
            _examPlanRecordRepository = examPlanRecordRepository;
            _examPlanPracticeRepository = examPlanPracticeRepository;
            _examTxRepository = examTxRepository;
            _examPracticeWrongRepository= examPracticeWrongRepository;
            _examPracticeCollectRepository= examPracticeCollectRepository;
            _examPlanAnswerRepository= examPlanAnswerRepository;
        }
        public class GetSubmitAnswerRequest
        {
            public int PracticeId { get; set; }
            public int Id { get; set; }
            public string Answer { get; set; }
        }
        public class GetSubmitAnswerResult
        {
            public bool IsRight { get; set; }
            public string Answer { get; set; }
            public string Jiexi { get; set; }
        }
        public class GetPracticingRequest
        {
            public int Id { get; set; }
            public string Zsd { get; set; }
        }
        public class GetRequest
        {
            public string keyWord { get; set; }
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
        }
        public class GetHistoryRequest
        {
            public string Order { get; set; }
            public string Type { get; set; }
            public string Title { get; set; }
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
        }
        public class GetResult
        {
            public string Watermark { get; set; }
            public int Total { get; set; }
            public int AnswerTotal { get; set; }
            public int RightTotal { get; set; }
            public int WrongTotal { get; set; }
            public List<int> TmIds { get; set; }
            public string Title { get; set; }
            public bool OpenExist { get; set; }
            public bool IsTiming { get; set; }
            public int TimingMinute { get; set; }
            public int UseTimeSecond { get; set; }
            public int TmIndex { get; set; }
        }
        public class GetCollectionResult
        {
            public int Total { get; set; }
            public List<GetResultZsdForTmCount> List { get; set; }
        }
        public class GetHistoryResult
        {
            public int Total { get; set; }
            public List<GetHistoryResultInfo> List { get; set; }
        }
        public class GetHistoryResultInfo
        {
            public int Id { get; set; }
            public List<string> Zsds { get; set; }
            public string DateTime { get; set; }
            public int TmCount { get; set; }
            public int AnswerCount { get; set; }
            public int RightCount { get; set; }
            public string Source { get; set; }
        }
        public class GetResultInfo
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public List<string> Zsds { get; set; }
            public string DateTime { get; set; }

        }
        public class GetResultZsdForTmCount
        {
            public string Zsd { get; set; }
            public int Count { get; set; }
        }
        public class GetSubmitPracticingRequest
        {
            public int PracticeId { get; set; }
            public int PracticeUserId { get; set; }
            public GetPracticingResultTmInfo Tm { get; set; }
        }
        public class GetPracticingIdsResult
        {
            public List<int> TmIdList { get; set; }
            public int Total { get; set; }
            public int PracticeUserId { get; set; }
            public string Title { get; set; }
        }
        public class GetPracticingResult
        {
            public List<GetPracticingResultTmInfo> List { get; set; }
            public int Total { get; set; }
            public int PracticeUserId { get; set; }
            public string Title { get; set; }
        }
        public class GetPracticingResultTmInfo
        {
            public int Id { get; set; }
            public string Tm { get; set; }
            public List<KeyValuePair<int, string>> TmTitle { get; set; }
            public string Tx { get; set; }
            public string TxType { get; set; }
            public List<string> Options { get; set; }
            public List<string> OptionsValue { get; set; }
            public int ParentId { get; set; }
            public List<GetPracticingResultTmInfo> SmallList { get; set; }
            public string Answer { get; set; }
            public string Zsd { get; set; }
            public string Analysis { get; set; }
            public bool IsRight { get; set; }
            public string RightAnswer { get; set; }
            public bool IsSubmit { get; set; }
            public bool IsCollection { get; set; }
            public bool IsError { get; set; }
            public int ErrorTotal { get; set; }
        }

        public class GetCollectionRequest
        {
            public bool IsBig { get; set; }
            public int TmId { get; set; }
            public bool Collection { get; set; }
        }
    }
}
