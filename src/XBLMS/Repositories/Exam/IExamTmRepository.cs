using Datory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;


namespace XBLMS.Repositories
{
    public partial interface IExamTmRepository : IRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsAsync(string title, int txId, int companyId);
        Task<bool> ExistsAsync(string title, int txId);
        Task<int> InsertAsync(ExamTm item);
        Task<bool> UpdateAsync(ExamTm item);
        Task<bool> DeleteAsync(int id);

        Task<(int total, List<ExamTm> list)> GetListAsync(int companyId, ExamTmGroup group, int pageIndex, int pageSize);
        Task<int> GetCountAsync(int companyId, ExamTmGroup group);
        Task<ExamTm> GetAsync(int id);
        Task<int> GetCountByTxIdAsync(int txId);
        Task<int> GetCountByTreeIdAsync(int treeId);
        Task<int> GetCountByTreeIdsAsync(List<int> treeIds);

        Task<List<int>> GetIdsAsync(List<int> treeIds, List<int> txIds, List<int> nandus, List<string> zhishidianKeywords, DateTime? dateFrom, DateTime? dateTo);

        Task<int> GetCountAsync(AuthorityAuth auth, bool hasGroup, bool allTm, List<int> tmIds, int txId, int nandu);
        Task<List<ExamTm>> GetListByRandomAsync(AuthorityAuth auth, bool allTm, bool hasGroup, List<int> tmIds, int txId, int nandu1Count = 0, int nandu2Count = 0, int nandu3Count = 0, int nandu4Count = 0, int nandu5Count = 0, List<int> noTmIds = null);
        Task<(int allCount, int addCount, int deleteCount, int lockedCount, int unLockedCount)> GetDataCount(AuthorityAuth auth);

        Task<List<ExamTm>> GetListByRandomAsync(AuthorityAuth auth, List<TmGroup> tmGroupList, List<ExamPaperRandomConfig> configList);
    }
}
