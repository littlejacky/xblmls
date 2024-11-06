using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IStudyCourseEvaluationUserRepository : IRepository
    {
        Task<int> InsertAsync(StudyCourseEvaluationUser item);
        Task<StudyCourseEvaluationUser> GetAsync(int id);
        Task<StudyCourseEvaluationUser> GetAsync(int planId, int courseId, int evaluationId, int userId);
        Task<(int total, List<StudyCourseEvaluationUser> list)> GetListAsync(int courseId, int pageIndex, int pageSize);
    }
}
