using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IStudyCourseFilesRepository : IRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<int> InsertAsync(StudyCourseFiles file);
        Task<bool> UpdateAsync(StudyCourseFiles file);
        Task<bool> DeleteAsync(int id);
        Task<List<StudyCourseFiles>> GetAllAsync(AuthorityAuth auth, string keyWords);
        Task<List<int>> GetIdsAsync(int groupId, string keyword, int organId);
        Task<List<StudyCourseFiles>> GetAllAsync(AuthorityAuth auth, int groupId);
        Task<List<StudyCourseFiles>> GetAllAsync(AuthorityAuth auth, int groupId, string keyword,int organId);

        Task<StudyCourseFiles> GetAsync(int id);
        Task<bool> IsExistsAsync(string fileName, int companyId, int groupId);
        Task<int> CountAsync();
        Task<long> SumFileSizeAsync(List<int> groupIds);
        Task<(int allCount, int addCount, int deleteCount, int lockedCount, int unLockedCount)> GetDataCount(AuthorityAuth auth);
    }
}
