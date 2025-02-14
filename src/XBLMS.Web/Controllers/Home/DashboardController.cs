using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
using XBLMS.Configuration;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class DashboardController : ControllerBase
    {
        private const string Route = "dashboard";

        private readonly IConfigRepository _configRepository;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IOrganManager _organManager;
        private readonly IExamManager _examManager;
        private readonly IExamPaperUserRepository _examPaperUserRepository;
        private readonly IExamPaperRepository _examPaperRepository;
        private readonly IExamPaperStartRepository _examPaperStartRepository;
        private readonly IExamQuestionnaireRepository _examQuestionnaireRepository;
        private readonly IExamQuestionnaireUserRepository _examQuestionnaireUserRepository;
        private readonly IExamAssessmentRepository _examAssessmentRepository;
        private readonly IExamAssessmentUserRepository _examAssessmentUserRepository;

        private readonly IStudyManager _studyManager;
        private readonly IStudyCourseUserRepository _studyCourseUserRepository;
        private readonly IStudyCourseRepository _studyCourseRepository;
        private readonly IStudyPlanRepository _studyPlanRepository;
        private readonly IStudyPlanUserRepository _studyPlanUserRepository;
        private readonly IStudyPlanCourseRepository _studyPlanCourseRepository;
        private readonly IExamCerUserRepository _examCerUserRepository;
        private readonly IExamCerRepository _examCerRepository;

        public DashboardController(IConfigRepository configRepository,
            ISettingsManager settingsManager,
            IOrganManager organManager,
            IAuthManager authManager,
            IExamManager examManager,
            IExamPaperUserRepository examPaperUserRepository,
            IExamPaperRepository examPaperRepository,
            IExamPaperStartRepository examPaperStartRepository,
            IExamQuestionnaireRepository examQuestionnaireRepository,
            IExamQuestionnaireUserRepository examQuestionnaireUserRepository,
            IExamAssessmentRepository examAssessmentRepository,
            IExamAssessmentUserRepository examAssessmentUserRepository,
            IStudyManager studyManager,
            IStudyCourseUserRepository studyCourseUserRepository,
            IStudyCourseRepository studyCourseRepository,
            IStudyPlanRepository studyPlanRepository,
            IStudyPlanUserRepository studyPlanUserRepository,
            IStudyPlanCourseRepository studyPlanCourseRepository,
            IExamCerUserRepository examCerUserRepository,
            IExamCerRepository examCerRepository)
        {
            _configRepository = configRepository;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _organManager = organManager;
            _examManager = examManager;
            _examPaperUserRepository = examPaperUserRepository;
            _examPaperRepository = examPaperRepository;
            _examPaperStartRepository = examPaperStartRepository;
            _examQuestionnaireRepository = examQuestionnaireRepository;
            _examQuestionnaireUserRepository = examQuestionnaireUserRepository;
            _examAssessmentRepository = examAssessmentRepository;
            _examAssessmentUserRepository = examAssessmentUserRepository;
            _studyManager = studyManager;
            _studyCourseUserRepository = studyCourseUserRepository;
            _studyCourseRepository = studyCourseRepository;
            _studyPlanRepository = studyPlanRepository;
            _studyPlanUserRepository = studyPlanUserRepository;
            _studyPlanCourseRepository = studyPlanCourseRepository;
            _examCerUserRepository = examCerUserRepository;
            _examCerRepository = examCerRepository;
        }

        public class GetResult
        {
            public User User { get; set; }
            public double AllPercent { get; set; }
            public double ExamPercent { get; set; }
            public double ExamMoniPercent { get; set; }
            public int ExamTotal { get; set; }
            public int ExamMoniTotal { get; set; }

            public ExamPaper ExamPaper { get; set; }
            public ExamPaper ExamMoni { get; set; }

            public int PracticeAnswerTmTotal { get; set; }
            public double PracticeAnswerPercent { get; set; }
            public int PracticeAllTmTotal { get; set; }
            public double PracticeAllPercent { get; set; }
            public int PracticeCollectTmTotal { get; set; }
            public double PracticeCollectPercent { get; set; }
            public int PracticeWrongTmTotal { get; set; }
            public double PracticeWrongPercent { get; set; }

            public int TaskPaperTotal { get; set; }
            public int TaskQTotal { get; set; }
            public int TaskAssTotal { get; set; }
            public int TaskPlanTotal { get; set; }
            public int TaskCourseTotal { get; set; }

            public List<StudyCourse> CourseList { get; set; }
            public StudyPlanUser StudyPlan { get; set; }

            public decimal StudyPlanTotalCredit { get; set; }
            public decimal StudyPlanTotalOverCredit { get; set; }

            public int TotalCourse { get; set; }
            public int TotalOverCourse { get; set; }

            public long TotalDuration { get; set; }

            public ExamCerUser TopCer { get; set; }

            public int PlanCount { get; set; }
            public int PlanOverCount { get; set; }

            public string DateStr { get; set; }
            public string Version { get; set; }

        }

    }
}
