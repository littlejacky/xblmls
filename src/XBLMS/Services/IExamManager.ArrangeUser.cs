using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Services
{
    public partial interface IExamManager
    {
        Task Arrange(ExamPaper paper, AuthorityAuth auth);
        Task Arrange(int paperId, int userId);
    }

}
