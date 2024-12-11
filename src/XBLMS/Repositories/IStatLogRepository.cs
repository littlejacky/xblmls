using Datory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IStatLogRepository : IRepository
    {
        Task<StatLog> GetAsync(int id);
        Task InsertAsync(StatLog statLog);
        Task<(int total, List<StatLog> list)> GetListAsync(AuthorityAuth auth, DateTime? lowerDate, DateTime? higherDate, int pageIndex, int pageSize);
    }
}
