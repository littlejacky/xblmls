using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public partial interface IStudyCourseTreeRepository : IRepository
    {
        Task<int> InsertAsync(StudyCourseTree item);

        Task<bool> UpdateAsync(StudyCourseTree item);

        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAsync(List<int> ids);

        Task<string> GetPathAsync(int id);
        Task<List<int>> GetParentIdListAsync(int id);
        Task<List<StudyCourseTree>> GetListAsync();
        Task<string> GetPathNamesAsync(int id);
        Task<StudyCourseTree> GetAsync(int id);
        Task<List<int>> GetIdsAsync(int id);
    }
}
