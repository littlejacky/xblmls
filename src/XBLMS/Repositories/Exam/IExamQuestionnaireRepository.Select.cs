using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public partial interface IExamQuestionnaireRepository : IRepository
    {
        Task<(int total, List<ExamQuestionnaire> list)> Select_GetListAsync(AuthorityAuth auth, string keyword, int pageIndex, int pageSize);
    }
}
