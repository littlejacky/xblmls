using Datory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IExamPlanPracticeRepository : IRepository
    {
        Task<ExamPlanPractice> GetAsync(int id);
        Task<(int total, List<ExamPlanPractice> list)> GetListAsync(int userId, string keyWords, string dateFrom, string dateTo, int pageIndex, int pageSize,bool isUnfinished = false);

        Task<int> InsertAsync(ExamPlanPractice item);
        Task IncrementAnswerCountAsync(int id);
        Task IncrementRightCountAsync(int id);
        Task DecrementRightCountAsync(int id);

        Task UpdateAsync(ExamPlanPractice item);

        Task DeleteAsync(int userId);
        Task<(int answerTotal, int rightTotal, int allAnswerTotal, int allRightTotal, int collectAnswerTotal, int collectRightTotal, int wrongAnswerTotal, int wrongRightTotal)> SumAsync(int userId);
        Task IncrementAsync(int id);
        Task<int> UpdateEndDateTimeAsync(int id, DateTime datetime);

        Task<List<ExamPlanPractice>> ListUnfinishAsync();
        Task<int> CountUserAsync(int recordId);

        Task<decimal> GetMaxScoreAsync(int recordId);
        Task<decimal> GetMinScoreAsync(int recordId);
        Task<decimal> SumScoreAsync(int recordId);
        Task<int> CountAsync(int recordId);
        Task<int> CountDistinctAsync(int recordId);
        Task<decimal> SumScoreDistinctAsync(int recordId);
        Task<int> CountByPassAsync(int recordId, int passScore);
        Task<int> CountByPassDistinctAsync(int recordId, int passScore);
    }
}
