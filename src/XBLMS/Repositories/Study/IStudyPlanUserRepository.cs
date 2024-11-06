using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IStudyPlanUserRepository : IRepository
    {
        Task<bool> ExistsAsync(int planId, int userId);
        Task<int> InsertAsync(StudyPlanUser item);
        Task<bool> UpdateAsync(StudyPlanUser item);
        Task<bool> DeleteAsync(int id);
        Task<StudyPlanUser> GetAsync(int id);
        Task<(int total, List<StudyPlanUser> list)> GetListAsync(string keyWords, int pageIndex, int pageSize);
    }
}
