using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IStudyPlanCourseRepository : IRepository
    {
        Task<int> InsertAsync(StudyPlanCourse item);
        Task<bool> UpdateAsync(StudyPlanCourse item);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByPlanAsync(int planId);
        Task<bool> DeleteByNotIdsAsync(List<int> notIds, int planId);
        Task<StudyPlanCourse> GetAsync(int id);
        Task<StudyPlanCourse> GetAsync(int planId, int courseId);
        Task<List<StudyPlanCourse>> GetListAsync(bool isSelect, int planId);
        Task<int> CountAsync(int planId, bool isSelect);
        Task<decimal> GetTotalCreditAsync(int planId, bool isSelect);
    }
}
