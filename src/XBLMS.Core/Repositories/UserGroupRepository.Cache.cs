using Datory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Core.Repositories
{
    public partial class UserGroupRepository
    {
        public async Task<UserGroup> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
        public async Task<List<UserGroup>> GetListAsync(AuthorityAuth auth)
        {
            var query = Q.NewQuery();
            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(UserGroup.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(UserGroup.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(UserGroup.CreatorId), auth.AdminId);
            }
            query.OrderByDesc(nameof(UserGroup.Id));

            var list = await _repository.GetAllAsync(query);

            if(list!=null && list.Count > 0)
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
        public async Task<List<UserGroup>> GetListAsync()
        {
            return await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(UserGroup.Id))
                .CachingGet(CacheKey)
            );

        }
        public async Task<List<UserGroup>> GetListWithoutLockedAsync()
        {
            var list = (await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(UserGroup.Id))
            )).ToList();

            return list.Where(g => g.Locked == false).ToList();
        }
        public async Task<List<UserGroup>> GetListWithoutLockedAsync(AuthorityAuth auth)
        {
            var list = await GetListAsync(auth);

            return list.Where(g => g.Locked == false).ToList();
        }
    }
}
