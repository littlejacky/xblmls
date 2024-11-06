using Datory;
using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IStudyCourseEvaluationItemUserRepository : IRepository
    {
        Task<int> InsertAsync(StudyCourseEvaluationItemUser item);
        Task<StudyCourseEvaluationItemUser> GetAsync(int id);
        Task<string> GetTextAsync(int courseId, int userId);
    }
}
