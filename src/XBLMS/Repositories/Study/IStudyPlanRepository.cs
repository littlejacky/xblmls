using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IStudyPlanRepository : IRepository
    {
        Task<int> InsertAsync(StudyPlan item);
        Task<bool> UpdateAsync(StudyPlan item);
        Task<bool> DeleteAsync(int id);
        Task<StudyPlan> GetAsync(int id);
        Task<(int total, List<StudyPlan> list)> GetListAsync(string keyWords, int pageIndex, int pageSize);
        Task<int> MaxAsync();
        Task<List<int>> GetYearListAsync();
    }
}
