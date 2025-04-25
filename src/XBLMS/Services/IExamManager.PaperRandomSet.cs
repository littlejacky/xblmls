using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Services
{
    public partial interface IExamManager
    {
        Task<bool> PaperRandomSet(ExamPaper paper, AuthorityAuth auth);
        Task<int> SetExamPaperRantomByRandomNowAndExaming(ExamPaper paper, AuthorityAuth auth, int? userId = null);
        Task<List<ExamTm>> SetExamPaperRantomByRandomNowAndExaming(ExamPlan paper, AuthorityAuth auth, int userId);
    }

}
