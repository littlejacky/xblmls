using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public partial interface IStudyCourseRepository
    {
        Task<(int total, List<StudyCourse> list)> Select_GetListAsync(string keyWords, string type, int pageIndex, int pageSize);
    }
}
