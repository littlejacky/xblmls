using System.Threading.Tasks;

namespace XBLMS.Core.Services
{
    public partial class OrganManager
    {
        public async Task DeleteCompany(int id)
        {
            await _companyRepository.DeleteAsync(id);
            await _administratorRepository.DeleteByCompanyIdAsync(id);
        }
    }
}
