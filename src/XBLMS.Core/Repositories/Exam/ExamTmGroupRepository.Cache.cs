using Datory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Core.Repositories
{
    public partial class ExamTmGroupRepository
    {
        public async Task<ExamTmGroup> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }

        public async Task<List<ExamTmGroup>> GetListAsync()
        {
            var list = (await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(ExamTmGroup.Id))
            )).ToList();

            return list;
        }
        public async Task<List<ExamTmGroup>> GetListAsync(AuthorityAuth auth)
        {
            var query = Q.NewQuery();
            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(ExamTmGroup.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(ExamTmGroup.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(ExamTmGroup.CreatorId), auth.AdminId);
            }
            query.OrderByDesc(nameof(ExamTmGroup.Id));

            var list = await _repository.GetAllAsync(query);

            if (list != null && list.Count > 0)
            {
                return list;
            }
            else
            {
                await ResetAsync(auth);
                list = await _repository.GetAllAsync(query);
                return list;
            }
        }
        public async Task<List<ExamTmGroup>> GetListWithoutLockedAsync(AuthorityAuth auth)
        {
            var all = await GetListAsync(auth);
            if (all != null && all.Count > 0)
            {
                return all.Where(g => g.Locked == false).ToList();
            }
            return null;
        }
        public async Task<List<ExamTmGroup>> GetListWithoutLockedAsync()
        {
            return await _repository.GetAllAsync(Q
                 .WhereNullOrFalse(nameof(ExamTmGroup.Locked))
                 .OrderByDesc(nameof(ExamTmGroup.Id))
             );
        }
        public async Task<List<ExamTmGroup>> GetListWithoutLockedAsync(int companyId)
        {
            return await _repository.GetAllAsync(Q
                 .Where(nameof(ExamTmGroup.CompanyId), companyId)
                 .WhereNullOrFalse(nameof(ExamTmGroup.Locked))
                 .OrderByDesc(nameof(ExamTmGroup.Id))
             );
        }
    }
}
