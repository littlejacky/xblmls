using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public partial interface IUserRepository : IRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<(bool success, string errorMessage)> ValidateAsync(string userName, string email, string mobile, string password);
        Task<(User user, string errorMessage)> InsertAsync(User user, string password, bool isChecked, string ipAddress);

        Task<(bool success, string errorMessage)> UpdateAsync(User user);

        Task UpdateLastActivityDateAndCountOfLoginAsync(User user);

        Task UpdateLastActivityDateAsync(User user);

        Task<(bool success, string errorMessage)> ChangePasswordAsync(int userId, string password);


        Task LockAsync(IList<int> userIds);

        Task UnLockAsync(IList<int> userIds);

        Task<bool> IsUserNameExistsAsync(string userName);

        Task<bool> IsEmailExistsAsync(string email);

        Task<bool> IsMobileExistsAsync(string mobile);

        Task<(User user, string userName, string errorMessage)> ValidateAsync(string account, string password,
            bool isPasswordMd5);

        Task<(bool success, string errorMessage)> ValidateStateAsync(User user);


        Task<int> GetCountAsync(int companyId, int departmentId, int dutyId);
        Task<int> GetCountAsync(List<int> companyIds, List<int> departmentIds, List<int> dutyIds);

        Task<User> DeleteAsync(int userId);

        Task<(int total, List<User> list)> GetListAsync(AuthorityAuth auth, List<int> companyIds, List<int> departmentIds, List<int> dutyIds, string keyWords, int pageIndex, int pageSize);

        Task<List<int>> GetUserIdsWithOutLockedAsync(AuthorityAuth auth, List<int> companyIds, List<int> departmentIds, List<int> dutyIds);
        Task<List<int>> GetUserIdsWithOutLockedAsync(AuthorityAuth auth);
    }
}
