using Datory;
using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IExamPlanTaskAnswerRepository : IRepository
    {
        Task DeleteByPracticeId(int practiceId);
        Task DeleteByUserId(int userId);
        Task<ExamPlanTaskAnswer> GetAsync(int userId,int tmId,int practiceId);

        Task<int> InsertAsync(ExamPlanTaskAnswer item);

        Task UpdateAsync(ExamPlanTaskAnswer item);

    }
}
