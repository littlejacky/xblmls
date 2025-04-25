using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Services
{
    public partial interface INotificationManager
    {
        Task SendExamPaperArrangedNotificationAsync(User user, ExamPaper paper);
        Task SendExamPracticeArrangedNotificationAsync(User user, ExamPractice practice);
    }
}
