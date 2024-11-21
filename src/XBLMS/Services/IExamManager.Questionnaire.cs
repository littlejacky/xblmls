using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Services
{
    public partial interface IExamManager
    {
        Task GetQuestionnaireInfo(ExamQuestionnaire paper, User user);
        Task ClearQuestionnaire(int examPaperId);
        Task ArrangeQuestionnaire(AuthorityAuth auth, ExamQuestionnaire paper);
        Task SetQuestionnairTm(List<ExamQuestionnaireTm> tmList,int paperId);
    }

}
