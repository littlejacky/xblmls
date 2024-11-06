using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public partial interface IUserRepository
    {
        Task<int> Group_CountWithoutLockedAsync(AuthorityAuth auth, UserGroup group);
        Task<List<int>> Group_IdsWithoutLockedAsync(AuthorityAuth auth, UserGroup group);

        Task<int> Group_RangeCountAsync(AuthorityAuth auth, List<int> groupUserIds, List<int> organIds, string organType, int range, int dayOfLastActivity, string keyword);
        Task<List<User>> Group_RangeUsersAsync(AuthorityAuth auth, List<int> groupUserIds, List<int> organIds, string organType, int range, int dayOfLastActivity, string keyword, string order, int offset, int limit);

        Task<int> Group_CountAsync(AuthorityAuth auth, UserGroup group, List<int> organIds, string organType, int dayOfLastActivity, string keyword);
        Task<List<User>> Group_UsersAsync(AuthorityAuth auth, UserGroup group, List<int> organIds, string organType, int dayOfLastActivity, string keyword, string order, int offset, int limit);
        Task<List<int>> Group_UserIdsAsync(AuthorityAuth auth, UserGroup group, List<int> organIds, string organType, int dayOfLastActivity, string keyword, string order);
    }
}
