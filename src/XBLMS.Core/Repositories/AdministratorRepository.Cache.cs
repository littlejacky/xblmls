﻿using Datory;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class AdministratorRepository
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

        private List<string> GetCacheKeys(Administrator admin)
        {
            if (admin == null) return new List<string>();

            var keys = new List<string>
            {
                GetCacheKeyByUserId(admin.Id),
                GetCacheKeyByGuid(admin.Guid),
                GetCacheKeyByUserName(admin.UserName)
            };

            if (!string.IsNullOrEmpty(admin.Mobile))
            {
                keys.Add(GetCacheKeyByMobile(admin.Mobile));
            }

            if (!string.IsNullOrEmpty(admin.Email))
            {
                keys.Add(GetCacheKeyByEmail(admin.Email));
            }

            return keys;
        }

        public async Task<Administrator> GetByAccountAsync(string account)
        {
            var admin = await GetByUserNameAsync(account);
            if (admin != null) return admin;
            if (StringUtils.IsMobile(account)) return await GetByMobileAsync(account);
            if (StringUtils.IsEmail(account)) return await GetByEmailAsync(account);

            return null;
        }

        private async Task<Administrator> GetAsync(Query query)
        {
            var admin = await _repository.GetAsync(query);

            if (admin != null && string.IsNullOrEmpty(admin.DisplayName))
            {
                admin.DisplayName = admin.UserName;
            }

            return admin;
        }

        public async Task<Administrator> GetByUserIdAsync(int userId)
        {
            if (userId <= 0) return null;

            return await GetAsync(Q
                .Where(nameof(Administrator.Id), userId)
                .CachingGet(GetCacheKeyByUserId(userId))
            );
        }

        public async Task<Administrator> GetByGuidAsync(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid)) return null;

            return await GetAsync(Q
                .Where(nameof(Administrator.Guid), guid)
                .CachingGet(GetCacheKeyByGuid(guid))
            );
        }

        public async Task<Administrator> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;

            return await GetAsync(Q
                .Where(nameof(Administrator.UserName), userName)
                .CachingGet(GetCacheKeyByUserName(userName))
            );
        }

        public async Task<Administrator> GetByMobileAsync(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return null;

            return await GetAsync(Q
                .Where(nameof(Administrator.Mobile), mobile)
                .CachingGet(GetCacheKeyByMobile(mobile))
            );
        }

        public async Task<Administrator> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var cacheKey = GetCacheKeyByEmail(email);
            return await GetAsync(Q
                .Where(nameof(Administrator.Email), email)
                .CachingGet(cacheKey)
            );
        }

        public string GetUserUploadFileName(string filePath)
        {
            var dt = DateTime.Now;
            return $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{PathUtils.GetExtension(filePath)}";
        }

        public async Task<string> GetDisplayAsync(int userId)
        {
            if (userId <= 0) return string.Empty;

            var admin = await GetByUserIdAsync(userId);
            return GetDisplay(admin);
        }

        public string GetDisplay(Administrator admin)
        {
            if (admin == null) return string.Empty;

            return string.IsNullOrEmpty(admin.DisplayName) || admin.UserName == admin.DisplayName ? admin.UserName : $"{admin.DisplayName}({admin.UserName})";
        }

        public async Task<(int allCount, int addCount, int deleteCount, int lockedCount, int unLockedCount)> GetDataCount(AuthorityAuth auth)
        {
            var total = 0;
            var lockedTotal = 0;
            var unLockedTotal = 0;
            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(Administrator.CompanyId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(Administrator.CompanyId), auth.CurManageOrganIds).WhereTrue(nameof(Administrator.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(Administrator.CompanyId), auth.CurManageOrganIds).WhereNullOrFalse(nameof(Administrator.Locked)));
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(Administrator.DepartmentId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(Administrator.DepartmentId), auth.CurManageOrganIds).WhereTrue(nameof(Administrator.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(Administrator.DepartmentId), auth.CurManageOrganIds).WhereNullOrFalse(nameof(Administrator.Locked)));
            }
            else
            {
                total = await _repository.CountAsync(Q.Where(nameof(Administrator.CreatorId), auth.AdminId));
                lockedTotal = await _repository.CountAsync(Q.Where(nameof(Administrator.CreatorId), auth.AdminId).WhereTrue(nameof(Administrator.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.Where(nameof(Administrator.CreatorId), auth.AdminId).WhereNullOrFalse(nameof(Administrator.Locked)));
            }

            return (total, 0, 0, lockedTotal, unLockedTotal);
        }
    }
}
