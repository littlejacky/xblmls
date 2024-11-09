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
        Task<bool> UpdateByPlanAsync(StudyPlan planInfo);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByPlanAsync(int planId);
        Task<StudyPlanUser> GetAsync(int id);
        Task<StudyPlanUser> GetAsync(int planId, int userId);
        Task<(int total, List<StudyPlanUser> list)> GetListAsync(int year, string state, string keyWords, int userId, int pageIndex, int pageSize);
        Task<(decimal totalCredit, decimal totalOverCredit)> GetCreditAsync(int userId);
        Task<int> GetTaskCountAsync(int userId);
    }
}
