using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IStudyCourseUserRepository : IRepository
    {
        Task<int> InsertAsync(StudyCourseUser item);
        Task<bool> UpdateAsync(StudyCourseUser item);
        Task<bool> UpdateByCourseAsync(StudyCourse courseInfo);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByCourseAsync(int courseId);
        Task<StudyCourseUser> GetAsync(int id);
        Task<StudyCourseUser> GetAsync(int userId, int planId, int courseId);
        Task<(int total, List<StudyCourseUser> list)> GetLastListAsync(int userId, int pageIndex, int pageSize);
        Task<(int total, List<StudyCourseUser> list)> GetListAsync(int userId, bool collection, string keyWords, string mark, string orderby, string state, int pageIndex, int pageSize);
        Task<(int total, List<string>)> GetMarkListAsync(int userId);
        Task<int> GetAvgEvaluationAsync(int courseId, int minStar);
        Task<List<StudyCourseUser>> GetListAsync(int planId, int userId);
        Task<(int total, int overTotal)> GetTotalAsync(int userId);
        Task<long> GetTotalDurationAsync(int userId);
        Task<int> GetTaskCountAsync(int userId);
        Task<int> GetOverCountAsync(int planId, bool isSelect);
        Task<int> GetOverCountAsync(int planId, int courseId, bool? isOver);
        Task<int> GetOverCountAsync(int planId, int userId, bool isSelect);
        Task<decimal> GetOverTotalCreditAsync(int planId, bool isSelect);
        Task<decimal> GetOverTotalCreditAsync(int planId, int userId, bool isSelect);

    }
}
