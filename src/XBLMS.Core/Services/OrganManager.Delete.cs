using Datory;
using System.Threading.Tasks;
using XBLMS.Models;
using XBLMS.Services;

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
