using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
using XBLMS.Configuration;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Web.Controllers.Home.Exam
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ExamPlanPracticeLogController : ControllerBase
    {
        private const string Route = "exam/examPlanPracticeLog";
        private const string RouteDelete = Route + "/del";

        private readonly IConfigRepository _configRepository;
        private readonly IAuthManager _authManager;
        private readonly IExamPlanPracticeRepository _examPlanPracticeRepository;

        public ExamPlanPracticeLogController(IConfigRepository configRepository, IAuthManager authManager, IExamPlanPracticeRepository examPracticeRepository)
        {
            _configRepository = configRepository;
            _authManager = authManager;
            _examPlanPracticeRepository = examPracticeRepository;
        }
        public class GetRequest
        {
            public string KeyWords { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
        }
        public class GetResult
        {
            public List<ExamPlanPractice> List { get; set; }
            public int Total { get; set; }
        }
    }
}
