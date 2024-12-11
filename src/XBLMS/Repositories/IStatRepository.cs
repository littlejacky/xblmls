using Datory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IStatRepository : IRepository
    {
        Task AddCountAsync(StatType statType);

        Task AddCountAsync(StatType statType, int adminId);

        Task AddCountAsync(StatType statType, Administrator admin);
        Task AddCountAsync(StatType statType, User user);

        Task<List<Stat>> GetStatsAsync(AuthorityAuth auth, DateTime lowerDate, DateTime higherDate, StatType statType);

        Task<List<Stat>> GetStatsAsync(DateTime lowerDate, DateTime higherDate,
            StatType statType);

        Task<List<Stat>> GetStatsAsync(DateTime lowerDate, DateTime higherDate,
            StatType statType, int adminId);

        Task DeleteAllAsync();
        Task<int> SumAsync(StatType statType, AuthorityAuth auth);
    }
}
