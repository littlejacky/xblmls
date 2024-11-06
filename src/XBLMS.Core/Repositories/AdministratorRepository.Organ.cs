using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SqlKata;
using XBLMS.Core.Utils;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class AdministratorRepository
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
                query.Where(nameof(Administrator.CompanyId), companyId);
            }
            if (departmentId > 0)
            {
                query.Where(nameof(Administrator.DepartmentId), departmentId);
            }
            if (dutyId > 0)
            {
                query.Where(nameof(Administrator.DutyId), dutyId);
            }

            return await _repository.CountAsync(query);
        }
        public async Task<int> Organ_CountAsync(List<int> companyIds, List<int> departmentIds, List<int> dutyIds)
        {
            var query = new Query();


            if (companyIds != null && companyIds.Count > 0)
            {
                query.WhereIn(nameof(Administrator.CompanyId), companyIds);
            }
            if (departmentIds != null && departmentIds.Count > 0)
            {
                query.WhereIn(nameof(Administrator.DepartmentId), departmentIds);
            }
            if (dutyIds != null && dutyIds.Count > 0)
            {
                query.WhereIn(nameof(Administrator.DutyId), dutyIds);
            }

            return await _repository.CountAsync(query);
        }
    }
}
