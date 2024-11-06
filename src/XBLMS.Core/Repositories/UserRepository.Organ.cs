using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SqlKata;
using XBLMS.Core.Utils;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class UserRepository
    {
        public async Task<int> Organ_CountAsync()
        {
            var query = Q.NewQuery();
            return await _repository.CountAsync(query);
        }
        public async Task<int> Organ_CountAsync(int companyId, int departmentId, int dutyId)
        {
            var query = Q.NewQuery();
            if (companyId > 0)
            {
                query.Where(nameof(User.CompanyId), companyId);
            }
            if (departmentId > 0)
            {
                query.Where(nameof(User.DepartmentId), departmentId);
            }
            if (dutyId > 0)
            {
                query.Where(nameof(User.DutyId), dutyId);
            }

            return await _repository.CountAsync(query);
        }
        public async Task<int> Organ_CountAsync(List<int> companyIds, List<int> departmentIds, List<int> dutyIds)
        {
            var query = new Query();


            if (companyIds != null && companyIds.Count > 0)
            {
                query.WhereIn(nameof(User.CompanyId), companyIds);
            }
            if (departmentIds != null && departmentIds.Count > 0)
            {
                query.WhereIn(nameof(User.DepartmentId), departmentIds);
            }
            if (dutyIds != null && dutyIds.Count > 0)
            {
                query.WhereIn(nameof(User.DutyId), dutyIds);
            }

            return await _repository.CountAsync(query);
        }
    }
}

