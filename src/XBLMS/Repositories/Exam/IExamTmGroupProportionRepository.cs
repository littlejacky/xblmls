using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public partial interface IExamTmGroupProportionRepository : IRepository
    {
        Task<int> InsertAsync(ExamTmGroupProportion item);
        Task<bool> UpdateAsync(ExamTmGroupProportion item);
        Task<List<ExamTmGroupProportion>> GetListAsync(int examPaperId);
        Task<bool> DeleteAsync(int Id);
        Task<int> DeleteByPaperAsync(int examPaperId);
    }
}
