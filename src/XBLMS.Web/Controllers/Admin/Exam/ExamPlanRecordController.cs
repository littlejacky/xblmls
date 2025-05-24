using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
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
    public partial class ExamPlanRecordController : ControllerBase
    {
        private const string Route = "exam/examPlanRecord";
        private const string RouteDelete = Route + "/del";
        private const string RouteLock = Route + "/lock";
        private const string RouteUnLock = Route + "/unLock";

        private readonly IAuthManager _authManager;
        private readonly IExamManager _examManager;
        private readonly IExamPlanRecordRepository _examPlanRecordRepository;
        private readonly IExamPaperTreeRepository _examPaperTreeRepository;
        private readonly IExamPaperUserRepository _examPaperUserRepository;
        private readonly IExamPaperStartRepository _examPaperStartRepository;
        private readonly IStudyCourseRepository _studyCourseRepository;
        private readonly IStudyPlanCourseRepository _studyPlanCourseRepository;
        public ExamPlanRecordController(IAuthManager authManager,
            IExamManager examManager,
            IExamPlanRecordRepository examPlanRecordRepository,
            IExamPaperTreeRepository examPaperTreeRepository,
            IExamPaperUserRepository examPaperUserRepository,
            IExamPaperStartRepository examPaperStartRepository,
            IStudyCourseRepository studyCourseRepository,
            IStudyPlanCourseRepository studyPlanCourseRepository)
        {
            _authManager = authManager;
            _examManager = examManager;
            _examPlanRecordRepository = examPlanRecordRepository;
            _examPaperTreeRepository = examPaperTreeRepository;
            _examPaperUserRepository = examPaperUserRepository;
            _examPaperStartRepository = examPaperStartRepository;
            _studyCourseRepository = studyCourseRepository;
            _studyPlanCourseRepository = studyPlanCourseRepository;
        }
        public class GetLockedRequest
        {
            public int Id { get; set; }
            public bool Locked { get; set; }
        }
        public class GetRequest
        {
            public bool TreeIsChildren { get; set; }
            public int TreeId { get; set; }
            public string Keyword { get; set; }
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
        }
        public class GetResult
        {
            public List<ExamPlanRecord> Items { get; set; }
            public int Total { get; set; }

        }
    }
}
