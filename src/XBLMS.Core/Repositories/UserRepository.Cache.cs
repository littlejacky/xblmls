using Datory;
using SqlKata;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class UserRepository
    {
        private string GetCacheKeyByUserId(int userId)
        {
            return CacheUtils.GetEntityKey(TableName, "userId", userId.ToString());
        }

        private string GetCacheKeyByGuid(string guid)
        {
            return CacheUtils.GetEntityKey(TableName, "guid", guid);
        }

        private string GetCacheKeyByUserName(string userName)
        {
            return CacheUtils.GetEntityKey(TableName, "userName", userName);
        }

        private string GetCacheKeyByMobile(string mobile)
        {
            return CacheUtils.GetEntityKey(TableName, "mobile", mobile);
        }

        private string GetCacheKeyByEmail(string email)
        {
            return CacheUtils.GetEntityKey(TableName, "email", email);
        }

        private string[] GetCacheKeysToRemove(User user)
        {
            if (user == null) return null;

            var list = new List<string>
            {
                GetCacheKeyByUserId(user.Id),
                GetCacheKeyByGuid(user.Guid),
                GetCacheKeyByUserName(user.UserName),
            };

            if (!string.IsNullOrEmpty(user.Mobile))
            {
                list.Add(GetCacheKeyByMobile(user.Mobile));
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                list.Add(GetCacheKeyByEmail(user.Email));
            }

            return list.ToArray();
        }

        public async Task<User> GetByAccountAsync(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;

            var user = await GetByUserNameAsync(account);
            if (user != null)
            {
                return user;
            }
            if (StringUtils.IsMobile(account))
            {
                return await GetByMobileAsync(account);
            }
            if (StringUtils.IsEmail(account))
            {
                return await GetByEmailAsync(account);
            }
            return null;
        }
        
        private async Task<User> GetAsync(Query query)
        {
            var user = await _repository.GetAsync(query);

            if (user != null && string.IsNullOrEmpty(user.DisplayName))
            {
                user.DisplayName = user.UserName;
            }

            return user;
        }

        public async Task<User> GetByUserIdAsync(int userId)
        {
            if (userId <= 0) return null;

            return await GetAsync(Q
                .Where(nameof(User.Id), userId)
                .CachingGet(GetCacheKeyByUserId(userId))
            );
        }

        public async Task<User> GetByGuidAsync(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid)) return null;

            return await GetAsync(Q
                .Where(nameof(User.Guid), guid)
                .CachingGet(GetCacheKeyByGuid(guid))
            );
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;

            return await GetAsync(Q
                .Where(nameof(User.UserName), userName)
                .CachingGet(GetCacheKeyByUserName(userName))
            );
        }

        public async Task<User> GetByMobileAsync(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return null;

            return await GetAsync(Q
                .Where(nameof(User.Mobile), mobile)
                .CachingGet(GetCacheKeyByMobile(mobile))
            );
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            return await GetAsync(Q
                .Where(nameof(User.Email), email)
                .CachingGet(GetCacheKeyByEmail(email))
            );
        }

        public async Task<string> GetDisplayAsync(int userId)
        {
            if (userId <= 0) return string.Empty;

            var user = await GetByUserIdAsync(userId);
            return GetDisplay(user);
        }

        public string GetDisplay(User user)
        {
            if (user == null) return string.Empty;

            return string.IsNullOrEmpty(user.DisplayName) || user.UserName == user.DisplayName ? user.UserName : $"{user.DisplayName}({user.UserName})";
        }


        public async Task<(int allCount, int addCount, int deleteCount, int lockedCount, int unLockedCount)> GetDataCount(AuthorityAuth auth)
        {
            var total = 0;
            var lockedTotal = 0;
            var unLockedTotal = 0;
            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(User.CompanyId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(User.CompanyId), auth.CurManageOrganIds).WhereTrue(nameof(User.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(User.CompanyId), auth.CurManageOrganIds).WhereNullOrFalse(nameof(User.Locked)));
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(User.DepartmentId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(User.DepartmentId), auth.CurManageOrganIds).WhereTrue(nameof(User.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(User.DepartmentId), auth.CurManageOrganIds).WhereNullOrFalse(nameof(User.Locked)));
            }
            else
            {
                total = await _repository.CountAsync(Q.Where(nameof(User.CreatorId), auth.AdminId));
                lockedTotal = await _repository.CountAsync(Q.Where(nameof(User.CreatorId), auth.AdminId).WhereTrue(nameof(User.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.Where(nameof(User.CreatorId), auth.AdminId).WhereNullOrFalse(nameof(User.Locked)));
            }

            return (total, 0, 0, lockedTotal, unLockedTotal);
        }
    }
}

