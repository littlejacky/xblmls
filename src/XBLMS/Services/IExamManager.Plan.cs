using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Services
{
    public partial interface IExamManager
    {
        Task<bool> CreateDailyTrainingTasks();
        Task<bool> CreateImmediatelyTrainingTasks(ExamPlan plan);

        Task MarkUnfinishPlanPractices();
    }

}
