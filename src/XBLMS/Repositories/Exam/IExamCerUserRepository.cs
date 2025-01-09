using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IExamCerUserRepository : IRepository
    {
        Task<bool> ExistsAsync(int userID, int examPaperId);
        Task<bool> ExistsAsync(int userID, int examPaperId, int planId, int courseId);
        Task<ExamCerUser> GetAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> ClearAsync(int userId);
        Task<ExamCerUser> GetAsync(int userId, int examPaperId);
        Task<List<ExamCerUser>> GetListAsync(int examPaperId);
        Task<List<ExamCerUser>> GetListAsync();
        Task<int> InsertAsync(ExamCerUser item);
        Task<(int total, List<ExamCerUser> list)> GetListAsync(int userId, int pageIndex, int pageSize);
        Task<(int total, List<ExamCerUser> list)> GetListAsync(int cerId, string keyWords, string beginDate, string endDate, int pageIndex, int pageSize);
        Task<(int total, List<ExamCerUser> list)> GetListAsync(int cerId, int planId, int courseId, string keyWords, string beginDate, string endDate, int pageIndex, int pageSize);
        Task<int> UpdateImgAsync(int id, string img);
        Task<int> GetCountAsync(int cerId);
    }
}
