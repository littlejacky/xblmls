using Datory;
using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IExamPlanAnswerRepository : IRepository
    {
        Task DeleteByPracticeId(int practiceId);
        Task DeleteByUserId(int userId);
        Task<ExamPlanAnswer> GetAsync(int userId,int tmId,int practiceId);

        Task<int> InsertAsync(ExamPlanAnswer item);

        Task UpdateAsync(ExamPlanAnswer item);

    }
}
