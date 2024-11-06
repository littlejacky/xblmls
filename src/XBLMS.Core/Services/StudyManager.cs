using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;

namespace XBLMS.Core.Services
{
    public partial class StudyManager : IStudyManager
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IOrganManager _organManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        private readonly IStudyCourseTreeRepository _studyCourseTreeRepository;
        private readonly IStudyCourseRepository _studyCourseRepository;
        private readonly IStudyPlanUserRepository _studyPlanUserRepository;
        private readonly IStudyCourseUserRepository _studyCourseUserRepository;


        public StudyManager(ISettingsManager settingsManager,
            IOrganManager organManager,
            IPathManager pathManager,
            IUserRepository userRepository,
            IUserGroupRepository userGroupRepository,
            IStudyCourseTreeRepository studyCourseTreeRepository,
            IStudyCourseRepository studyCourseRepository,
            IStudyPlanUserRepository studyPlanUserRepository,
            IStudyCourseUserRepository studyCourseUserRepository)
        {
            _settingsManager = settingsManager;
            _organManager = organManager;
            _pathManager = pathManager;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _studyCourseTreeRepository = studyCourseTreeRepository;
            _studyCourseRepository = studyCourseRepository;
            _studyPlanUserRepository = studyPlanUserRepository;
            _studyCourseUserRepository = studyCourseUserRepository;
        }
    }
}
