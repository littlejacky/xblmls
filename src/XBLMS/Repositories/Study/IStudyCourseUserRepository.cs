using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IStudyCourseUserRepository : IRepository
    {
        Task<int> InsertAsync(StudyCourseUser item);
        Task<bool> UpdateAsync(StudyCourseUser item);
        Task<bool> DeleteAsync(int id);
        Task<StudyCourseUser> GetAsync(int id);
        Task<StudyCourseUser> GetAsync(int userId, int planId, int courseId);
        Task<(int total, List<StudyCourseUser> list)> GetListAsync(int userId, bool collection, string keyWords, string mark, string orderby, string state, int pageIndex, int pageSize);
        Task<(int total, List<string>)> GetMarkListAsync(int userId);
        Task<int> GetAvgEvaluationAsync(int courseId, int minStar);
        Task<List<StudyCourseUser>> GetListAsync(int planId, int userId);
        Task<(int total, int overTotal)> GetTotalAsync(int userId);
        Task<long> GetTotalDurationAsync(int userId);

    }
}
