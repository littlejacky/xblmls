﻿using Datory;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public partial interface IUserGroupRepository : IRepository
    {
        Task<int> InsertAsync(UserGroup group);

        Task UpdateAsync(UserGroup group);

        Task DeleteAsync(int groupId);
        Task DeleteByUserId(int userId);

        Task ResetAsync(AuthorityAuth auth);
    }
}
