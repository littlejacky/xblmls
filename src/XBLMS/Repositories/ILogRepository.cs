using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface ILogRepository : IRepository
    {
        Task AddAdminLogAsync(Administrator admin, string ipAddress, string action, string summary = "");

        Task AddUserLogAsync(User user, string ipAddress, string action, string summary = "");

        Task DeleteIfThresholdAsync();

        Task DeleteAllAdminLogsAsync();

        Task DeleteAllUserLogsAsync();


        Task<int> GetUserLogsCountAsync(AuthorityAuth auth, int userId, string keyword, string dateFrom, string dateTo);

        Task<List<Log>> GetUserLogsAsync(AuthorityAuth auth, int userId, string keyword, string dateFrom, string dateTo, int offset, int limit);

        Task<List<Log>> GetUserLogsAsync(int userId, int offset, int limit);

        Task<List<Log>> GetAdminLogsAsync(int adminId, int offset, int limit);



        Task<int> GetAdminLogsCountAsync(AuthorityAuth auth, List<int> adminIds, string keyword, string dateFrom, string dateTo);

        Task<List<Log>> GetAdminLogsAsync(AuthorityAuth auth, List<int> adminIds, string keyword, string dateFrom, string dateTo, int offset, int limit);
    }
}
