using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IExamPaperStartRepository : IRepository
    {
        Task<ExamPaperStart> GetAsync(int id);
        Task<int> CountAsync(int paperId, int userId);
        Task<int> CountAsync(int paperId, int userId, int planId, int courseId);
        Task<ExamPaperStart> GetNoSubmitAsync(int paperId, int userId);
        Task<ExamPaperStart> GetNoSubmitAsync(int paperId, int userId, int planId, int courseId);
        Task<int> GetNoSubmitIdAsync(int paperId, int userId);
        Task<int> GetNoSubmitIdAsync(int paperId, int userId, int planId, int courseId);
        Task<int> InsertAsync(ExamPaperStart item);
        Task ClearByUserAsync(int userId);
        Task ClearByPaperAsync(int paperId);
        Task ClearByPaperAndUserAsync(int paperId, int userId);
        Task DeleteAsync(int id);
        Task UpdateAsync(ExamPaperStart item);
        Task IncrementAsync(int id);
        Task<List<ExamPaperStart>> GetListAsync(int paperId, int userId);
        Task<List<ExamPaperStart>> GetListAsync(int paperId, int userId, int planId, int courseId);
        Task<(int total, List<ExamPaperStart> list)> GetListAsync(int userId, string dateFrom, string dateTo, string keyWords, int pageIndex, int pageSize);
        Task<(int total, List<ExamPaperStart> list)> GetListByAdminAsync(int paperId, int planId, int courseId, string dateFrom, string dateTo, string keyWords, int pageIndex, int pageSize, bool isMark = true);
        Task<(int total, List<ExamPaperStart> list)> GetListByAdminAsync(int paperId, string dateFrom, string dateTo, string keyWords, int pageIndex, int pageSize, bool isMark = true);
        Task<(int total, List<ExamPaperStart> list)> GetListByMarkerAsync(int markerId, string keyWords, int pageIndex, int pageSize);

        Task<List<int>> GetPaperIdsAsync(int userId);
        Task<decimal> GetMaxScoreAsync(int userId, int paperId);
        Task<decimal> GetMaxScoreAsync(int userId, int paperId, int planId, int courseId);
        Task UpdateLockedAsync(int paperId, bool locked);
        Task UpdateKeyWordsAsync(int paperId, string keyWords);

        Task<decimal> GetMaxScoreAsync(int paperId);
        Task<decimal> GetMinScoreAsync(int paperId);
        Task<int> CountAsync(int paperId);
        Task<int> CountDistinctAsync(int paperId);
        Task<decimal> SumScoreAsync(int paperId);
        Task<decimal> SumScoreDistinctAsync(int paperId);
        Task<int> CountByPassAsync(int paperId, int passScore);
        Task<int> CountByPassDistinctAsync(int paperId, int passScore);
        Task<int> CountByMarkAsync(int paperId);
    }
}
