using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Services
{
    public partial interface INotificationManager
    {
        Task SendExamArrangedNotificationAsync(User user, ExamPaper paper);
    }
}
