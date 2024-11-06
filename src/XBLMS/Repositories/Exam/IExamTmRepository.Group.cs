using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;


namespace XBLMS.Repositories
{
    public partial interface IExamTmRepository
    {
        Task<int> Group_CountAsync(AuthorityAuth auth, ExamTmGroup group);
        Task<List<int>> Group_RangeIdsAsync(AuthorityAuth auth, ExamTmGroup group);
        Task<(int total, List<ExamTm> list)> Group_SelectListAsync(AuthorityAuth auth, ExamTmGroup group, List<int> treeIds, int txId, int nandu, string keyword, string order, string orderType, int pageIndex, int pageSize);
        Task<(int total, List<ExamTm> list)> Group_ListAsync(AuthorityAuth auth, ExamTmGroup group, List<int> treeIds, int txId, int nandu, string keyword, string order, string orderType, bool? locked, int pageIndex, int pageSize);
    }
}
