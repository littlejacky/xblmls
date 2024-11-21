using System.Collections.Generic;
using System.Threading.Tasks;

namespace XBLMS.Repositories
{
    public partial interface IAdministratorRepository
    {
        Task<int> Organ_CountAsync();
        Task<int> Organ_CountAsync(int companyId, int departmentId, int dutyId);
        Task<int> Organ_CountAsync(List<int> companyIds, List<int> departmentIds, List<int> dutyIds);
    }
}
