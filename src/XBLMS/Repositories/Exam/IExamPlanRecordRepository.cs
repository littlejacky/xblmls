using Datory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public partial interface IExamPlanRecordRepository : IRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<ExamPlanRecord> GetAsync(int id);
        Task<int> InsertAsync(ExamPlanRecord item);
        Task<bool> UpdateAsync(ExamPlanRecord item);
        Task<(int total, List<ExamPlanRecord> list)> GetListAsync(AuthorityAuth auth, List<int> treeIds, string keyword, int pageIndex, int pageSize);
        Task<bool> DeleteAsync(int Id);
        Task<int> MaxAsync();
        Task<List<ExamPlanRecord>> GetIdsAsync(List<int> ids, string keyword);
        //Task<int> GetCerCount(int cerId);
        Task<int> GetGroupCount(int groupId);
        Task<int> GetTmGroupCount(int groupId);
        Task<(int allCount, int addCount, int deleteCount, int lockedCount, int unLockedCount)> GetDataCount(AuthorityAuth auth);
    }
}
