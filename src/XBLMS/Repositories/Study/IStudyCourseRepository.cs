using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public partial interface IStudyCourseRepository : IRepository
    {
        Task<int> InsertAsync(StudyCourse item);
        Task<int> IncrementTotalUserAsync(int id);
        Task<bool> UpdateAsync(StudyCourse item);
        Task<bool> DeleteAsync(int id);
        Task<StudyCourse> GetAsync(int id);
        Task<(int total, List<StudyCourse> list)> GetListAsync(AuthorityAuth auth, string keyWords, string type, int treeId, bool children, int pageIndex, int pageSize);
        Task<(int total, List<StudyCourse> list)> GetListByTeacherAsync(int teacherId, string keyWords, int pageIndex, int pageSize);
        Task<int> MaxAsync();
        Task<(int total, int count)> CountAsync(int treeId);
        Task<List<string>> GetMarkListAsync();
    }
}
