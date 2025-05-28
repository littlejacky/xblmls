using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using XBLMS.Configuration;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ExamPlanRecordManagerController : ControllerBase
    {
        private const string Route = "exam/examPlanRecordManager";

        private const string RouteUser = Route + "/user";
        private const string RouteUserUpdateDateTime = RouteUser + "/datetime";
        private const string RouteUserUpdateExamTimes = RouteUser + "/examtimes";
        private const string RouteUserDelete = RouteUser + "/remove";
        private const string RouteUserDeleteOne = RouteUser + "/removeone";
        private const string RouteUserExport = RouteUser + "/export";


        private const string RouteScore = Route + "/score";
        private const string RouteScoreExport = RouteScore + "/export";

        private const string RouteMark = Route + "/mark";
        private const string RouteMarkSetMarker = Route + "/marker";


        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IOrganManager _organManager;
        private readonly IUserRepository _userRepository;
        private readonly IExamPlanRecordRepository _examPlanRecordRepository;
        //private readonly IExamPaperUserRepository _examPaperUserRepository;
        private readonly IExamPlanPracticeRepository _examPlanPracticeRepository;
        private readonly IExamPlanAnswerRepository _examPlanAnswerRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public ExamPlanRecordManagerController(IAuthManager authManager,
            IPathManager pathManager,
            IOrganManager organManager,
            IUserRepository userRepository,
            IAdministratorRepository administratorRepository,
            //IExamPaperUserRepository examPaperUserRepository,
            IExamPlanAnswerRepository examPlanAnswerRepository,
            IExamPlanPracticeRepository examPlanPracticeRepository,
            IExamPlanRecordRepository examPlanRecordRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _organManager = organManager;
            _userRepository = userRepository;
            _administratorRepository = administratorRepository;
            //_examPaperUserRepository = examPaperUserRepository;
            _examPlanAnswerRepository = examPlanAnswerRepository;
            _examPlanPracticeRepository = examPlanPracticeRepository;
            _examPlanRecordRepository = examPlanRecordRepository;
        }

        public class GetSelectMarkInfo
        {
            public int Id { get; set; }
            public string DisplayName {  get; set; }
            public string UserName { get; set; }
        }

        public class GetResult
        {
            public string Title { get; set; }
            public int TotalScore { get; set; }
            public int PassScore { get; set; }
            public int TotalUser { get; set; }
            public int TotalExamTimes { get; set; }
            public int TotalExamTimesDistinct { get; set; }
            public int TotalPass { get; set; }
            public int TotalPassDistinct { get; set; }
            public decimal MaxScore { get; set; }
            public decimal MinScore { get; set; }
            public decimal TotalUserScore { get; set; }
            public List<GetSelectMarkInfo> MarkerList { get; set; }
        }
        public class GetUserRequest
        {
            public int Id { get; set; }
            public string Keywords { get; set; }
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
        }
        public class GetUserResult
        {
            public int Total { get; set; }
            public List<ExamPaperUser> List { get; set; }
        }
        public class GetSetMarkerRequest
        {
            public int Id { get; set; }
            public List<int> Ids { get; set; }
        }

        public class GetSocreRequest
        {
            public int Id { get; set; }
            public int PlanId { get; set; }
            public int CourseId { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
            public string Keywords { get; set; }
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
        }
        public class GetScoreResult
        {
            public int Total { get; set; }
            public List<ExamPaperStart> List { get; set; }
            public List<GetSelectMarkInfo> MarkerList { get; set; }
        }


        public class GetUserUpdateRequest
        {
            public int Id { get; set; }
            public List<int> Ids { get; set; }
            public bool Increment { get; set; }
            public DateTime ExamBeginDateTime { get; set; }
            public DateTime ExamEndDateTime { get; set; }
        }
    }
}
