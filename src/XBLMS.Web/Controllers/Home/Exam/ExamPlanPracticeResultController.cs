using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using XBLMS.Configuration;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Web.Controllers.Home.Exam
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ExamPlanPracticeResultController : ControllerBase
    {
        private const string Route = "exam/examPracticeResult";

        private readonly IConfigRepository _configRepository;
        private readonly IAuthManager _authManager;
        private readonly IExamPlanPracticeRepository _examPlanPracticeRepository;

        public ExamPlanPracticeResultController(IConfigRepository configRepository,
            IAuthManager authManager, IExamPlanPracticeRepository examPlanPracticeRepository)
        {
            _configRepository = configRepository;
            _authManager = authManager;
            _examPlanPracticeRepository = examPlanPracticeRepository;
        }
        public class GetResult
        {
            public int Total { get; set; }
            public int AnswerTotal { get; set; }
            public int RightTotal { get; set; }
            public int WrongTotal { get; set; }
        }
    }
}
