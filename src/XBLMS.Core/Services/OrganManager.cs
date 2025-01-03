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
        private readonly IExamPaperRepository _examPaperRepository;
        private readonly IExamQuestionnaireRepository _examQuestionnaireRepository;
        private readonly IStudyPlanRepository _studyPlanRepository;
        public OrganManager(IOrganCompanyRepository companyRepository,
            IOrganDepartmentRepository departmentRepository,
            IAdministratorRepository administratorRepository,
            IUserRepository userRepository,
            IOrganDutyRepository dutyRepository,
            IExamPaperRepository examPaperRepository,
            IExamQuestionnaireRepository examQuestionnaireRepository,
            IStudyPlanRepository studyPlanRepository)
        {
            _companyRepository = companyRepository;
            _departmentRepository = departmentRepository;
            _administratorRepository = administratorRepository;
            _userRepository = userRepository;
            _dutyRepository = dutyRepository;
            _examPaperRepository = examPaperRepository;
            _examQuestionnaireRepository = examQuestionnaireRepository;
            _studyPlanRepository = studyPlanRepository;
        }
        public async Task<int> GetGroupCount(int groupId)
        {
            return await _studyPlanRepository.GetGroupCount(groupId)
                + await _examQuestionnaireRepository.GetGroupCount(groupId)
                + await _examPaperRepository.GetGroupCount(groupId);
        }
    }
}
