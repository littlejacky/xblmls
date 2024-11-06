using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
using XBLMS.Configuration;
using XBLMS.Core.Repositories;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Web.Controllers.Home.Study
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class StudyCourseEvaluationController : ControllerBase
    {
        private const string Route = "study/studyCourseEvaluation";

        private readonly IConfigRepository _configRepository;
        private readonly IAuthManager _authManager;
        private readonly IStudyManager _studyManager;
        private readonly IOrganManager _organManager;
        private readonly IStudyCourseRepository _studyCourseRepository;
        private readonly IStudyCourseUserRepository _studyCourseUserRepository;
        private readonly IStudyPlanCourseRepository _studyPlanCourseRepository;
        private readonly IStudyPlanRepository _studyPlanRepository;
        private readonly IStudyCourseEvaluationItemRepository _studyCourseEvaluationItemRepository;
        private readonly IStudyCourseEvaluationUserRepository _studyCourseEvaluationUserRepository;
        private readonly IStudyCourseEvaluationItemUserRepository _studyCourseEvaluationItemUserRepository;

        public StudyCourseEvaluationController(IConfigRepository configRepository,
            IAuthManager authManager,
            IStudyManager studyManager,
            IOrganManager organManager,
            IStudyCourseRepository studyCourseRepository,
            IStudyCourseUserRepository studyCourseUserRepository,
            IStudyPlanCourseRepository studyPlanCourseRepository,
            IStudyPlanRepository studyPlanRepository,
            IStudyCourseEvaluationUserRepository studyCourseEvaluationUserRepository,
            IStudyCourseEvaluationItemUserRepository studyCourseEvaluationItemUserRepository,
            IStudyCourseEvaluationItemRepository studyCourseEvaluationItemRepository)
        {
            _configRepository = configRepository;
            _authManager = authManager;
            _studyManager = studyManager;
            _organManager = organManager;
            _studyCourseRepository = studyCourseRepository;
            _studyCourseUserRepository = studyCourseUserRepository;
            _studyPlanCourseRepository = studyPlanCourseRepository;
            _studyPlanRepository = studyPlanRepository;
            _studyCourseEvaluationUserRepository = studyCourseEvaluationUserRepository;
            _studyCourseEvaluationItemUserRepository = studyCourseEvaluationItemUserRepository;
            _studyCourseEvaluationItemRepository = studyCourseEvaluationItemRepository;
        }
        public class GetRequest
        {
            public int PlanId { get; set; }
            public int CourseId { get; set; }
            public int EId { get; set; }
        }
        public class GetResult
        {
            public string Title { get; set; }
            public List<StudyCourseEvaluationItem> List { get; set; }
        }
        public class GetSubmitRequest
        {
            public int CourseId { get; set; }
            public int PlanId { get; set; }
            public int EId { get; set; }
            public List<StudyCourseEvaluationItem> List { get; set; }
        }
    }
}
