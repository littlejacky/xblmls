using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public partial interface IExamTmGroupRepository
    {
        Task<ExamTmGroup> GetAsync(int id);
        Task<List<ExamTmGroup>> GetListAsync();
        Task<List<ExamTmGroup>> GetListAsync(AuthorityAuth auth);
        Task<List<ExamTmGroup>> GetListWithoutLockedAsync(AuthorityAuth auth);
        Task<List<ExamTmGroup>> GetListWithoutLockedAsync();
        Task<List<ExamTmGroup>> GetListWithoutLockedAsync(int companyId);
    }
}
