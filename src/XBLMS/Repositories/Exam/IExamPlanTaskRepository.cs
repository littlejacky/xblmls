using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IExamPlanTaskRepository : IRepository
    {
        Task<ExamPlanPractice> GetAsync(int id);
        Task<(int total, List<ExamPlanPractice> list)> GetListAsync(int userId, string dateFrom, string dateTo, int pageIndex, int pageSize,bool isPlan=false);

        Task<int> InsertAsync(ExamPlanPractice item);
        Task IncrementAnswerCountAsync(int id);
        Task IncrementRightCountAsync(int id);
        Task DecrementRightCountAsync(int id);

        Task UpdateAsync(ExamPlanPractice item);

        Task DeleteAsync(int userId);
        Task<(int answerTotal, int rightTotal, int allAnswerTotal, int allRightTotal, int collectAnswerTotal, int collectRightTotal, int wrongAnswerTotal, int wrongRightTotal)> SumAsync(int userId);

    }
}
