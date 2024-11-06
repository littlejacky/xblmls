using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public partial interface IUserGroupRepository
    {
        Task<UserGroup> GetAsync(int id);
        Task<List<UserGroup>> GetListAsync();
        Task<List<UserGroup>> GetListWithoutLockedAsync();
        Task<List<UserGroup>> GetListAsync(AuthorityAuth auth);
        Task<List<UserGroup>> GetListWithoutLockedAsync(AuthorityAuth auth);
    }
}
