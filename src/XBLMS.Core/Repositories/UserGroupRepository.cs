﻿using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class UserGroupRepository : IUserGroupRepository
    {
        private readonly Repository<UserGroup> _repository;
        private readonly IConfigRepository _configRepository;

        public UserGroupRepository(ISettingsManager settingsManager, IConfigRepository configRepository)
        {
            _repository = new Repository<UserGroup>(settingsManager.Database, settingsManager.Redis);
            _configRepository = configRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey => CacheUtils.GetListKey(_repository.TableName);

        public async Task<int> InsertAsync(UserGroup group)
        {
            return await _repository.InsertAsync(group);
        }

        public async Task UpdateAsync(UserGroup group)
        {
            await _repository.UpdateAsync(group);
        }


        public async Task DeleteAsync(int groupId)
        {
            await _repository.DeleteAsync(groupId);
        }
        public async Task DeleteByUserId(int userId)
        {
            var allGroup = await GetListAsync();
            foreach (var group in allGroup)
            {
                if (group.GroupType == UsersGroupType.Fixed)
                {
                    if (ListUtils.Contains(group.UserIds, userId))
                    {
                        ListUtils.Remove(group.UserIds, userId);
                        await UpdateAsync(group);
                    }
                }
            }

        }
        public async Task ResetAsync(AuthorityAuth auth)
        {
            await _repository.InsertAsync(new UserGroup
            {
                GroupName = "全部用户",
                GroupType = UsersGroupType.All,
                CompanyId = auth.CompanyId,
                DepartmentId = auth.DepartmentId,
                CreatorId = auth.AdminId
            });
        }
    }
}
