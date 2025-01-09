using System.Threading.Tasks;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Core.Services
{
    public partial class OrganManager : IOrganManager
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrganCompanyRepository _companyRepository;
        private readonly IOrganDepartmentRepository _departmentRepository;
        private readonly IOrganDutyRepository _dutyRepository;
        private readonly IDatabaseManager _databaseManager;
        public OrganManager(IOrganCompanyRepository companyRepository,
            IOrganDepartmentRepository departmentRepository,
            IAdministratorRepository administratorRepository,
            IUserRepository userRepository,
            IOrganDutyRepository dutyRepository,
            IDatabaseManager databaseManager)
        {
            _companyRepository = companyRepository;
            _departmentRepository = departmentRepository;
            _administratorRepository = administratorRepository;
            _userRepository = userRepository;
            _dutyRepository = dutyRepository;
            _databaseManager = databaseManager;
        }
        public async Task<int> GetGroupCount(int groupId)
        {
            return await _databaseManager.StudyPlanRepository.GetGroupCount(groupId)
                + await _databaseManager.ExamQuestionnaireRepository.GetGroupCount(groupId)
                + await _databaseManager.ExamPaperRepository.GetGroupCount(groupId);
        }
        public async Task DeleteUser(int userId)
        {
            await _userRepository.DeleteAsync(userId);

            await _databaseManager.UserGroupRepository.DeleteByUserId(userId);
            await _databaseManager.ExamQuestionnaireUserRepository.ClearByUserAsync(userId);
            await _databaseManager.ExamQuestionnaireAnswerRepository.ClearByUserAsync(userId);
            await _databaseManager.ExamPracticeWrongRepository.DeleteByUserId(userId);
            await _databaseManager.ExamPracticeRepository.DeleteAsync(userId);
            await _databaseManager.ExamPracticeCollectRepository.DeleteByUserId(userId);
            await _databaseManager.ExamPracticeAnswerRepository.DeleteByUserId(userId);
            await _databaseManager.ExamPaperUserRepository.ClearByUserAsync(userId);
            await _databaseManager.ExamPaperStartRepository.ClearByUserAsync(userId);
            await _databaseManager.ExamPaperAnswerRepository.ClearByUserAsync(userId);
            await _databaseManager.ExamCerUserRepository.ClearAsync(userId);
            await _databaseManager.StudyCourseEvaluationItemUserRepository.DeleteByUserId(userId);
            await _databaseManager.StudyCourseEvaluationUserRepository.DeleteByUserId(userId);
            await _databaseManager.StudyCourseUserRepository.DeleteByUserId(userId);
            await _databaseManager.StudyPlanUserRepository.DeleteByUserId(userId);
            await _databaseManager.StudyCourseWareUserRepository.DeleteByUserId(userId);
        }
    }
}
