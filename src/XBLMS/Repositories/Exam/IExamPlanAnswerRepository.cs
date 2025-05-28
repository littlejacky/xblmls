using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IExamPlanAnswerRepository : IRepository
    {
        Task DeleteByPracticeId(int practiceId);
        Task DeleteByUserId(int userId);
        Task<ExamPlanAnswer> GetAsync(int userId, int tmId, int practiceId);
        Task<int> CountByPracticeId(int id);
        Task<List<ExamPlanAnswer>> ListByPracticeId(int id);
        Task<int> InsertAsync(ExamPlanAnswer item);

        Task UpdateAsync(ExamPlanAnswer item);

        Task<decimal> ScoreSumAsync(int startId);
        Task<decimal> ObjectiveScoreSumAsync(int startId);
        Task<decimal> SubjectiveScoreSumAsync(int startId);
    }
}
