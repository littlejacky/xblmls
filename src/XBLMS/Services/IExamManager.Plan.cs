using System.Threading.Tasks;

namespace XBLMS.Services
{
    public partial interface IExamManager
    {
        Task<bool> CreateDailyTrainingTasks();
    }

}
