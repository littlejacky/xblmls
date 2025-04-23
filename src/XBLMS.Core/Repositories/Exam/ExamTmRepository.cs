using Datory;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;


namespace XBLMS.Core.Repositories
{
    public partial class ExamTmRepository : IExamTmRepository
    {
        private readonly Repository<ExamTm> _repository;

        public ExamTmRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ExamTm>(settingsManager.Database, settingsManager.Redis);
        }


        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }
        public async Task<bool> ExistsAsync(string title, int txId)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(ExamTm.TxId), txId).Where(nameof(ExamTm.Title), title));
        }
        public async Task<bool> ExistsAsync(string title, int txId, int companyId)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(ExamTm.TxId), txId).Where(nameof(ExamTm.Title), title).Where(nameof(ExamTm.CompanyId), companyId));
        }

        public async Task<int> InsertAsync(ExamTm item)
        {
            return await _repository.InsertAsync(item);
        }

        public async Task<bool> UpdateAsync(ExamTm item)
        {
            return await _repository.UpdateAsync(item);
        }

        public async Task<(int total, List<ExamTm> list)> GetListAsync(int companyId, ExamTmGroup group, int pageIndex, int pageSize)
        {
            var query = Q.Where(nameof(ExamTm.CompanyId), companyId);

            if (group != null)
            {
                if (group.GroupType == TmGroupType.Fixed)
                {
                    if (group.TmIds != null && group.TmIds.Count > 0)
                    {
                        group.TmIds = ListUtils.GetRandomList(group.TmIds, 2000);
                        query.WhereIn(nameof(ExamTm.Id), group.TmIds);
                    }
                    else
                    {
                        query.Where(nameof(ExamTm.Id), -1);
                    }
                }
                if (group.GroupType == TmGroupType.Range)
                {
                    var ids = await GetIdsAsync(group.TreeIds, group.TxIds, group.Nandus, group.Zhishidians, group.DateFrom, group.DateTo);
                    if (ids != null && ids.Count > 0)
                    {
                        ids = ListUtils.GetRandomList(ids, 2000);
                        query.WhereIn(nameof(ExamTm.Id), ids);
                    }
                    else
                    {
                        query.Where(nameof(ExamTm.Id), -1);
                    }
                }
            }


            var count = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (count, list);
        }
        public async Task<int> GetCountAsync(int companyId, ExamTmGroup group)
        {
            var query = Q.Where(nameof(ExamTm.CompanyId), companyId);

            if (group != null)
            {
                if (group.GroupType == TmGroupType.Fixed)
                {
                    if (group.TmIds != null && group.TmIds.Count > 0)
                    {
                        group.TmIds = ListUtils.GetRandomList(group.TmIds, 2000);
                        query.WhereIn(nameof(ExamTm.Id), group.TmIds);
                    }
                    else
                    {
                        query.Where(nameof(ExamTm.Id), -1);
                    }
                }
                if (group.GroupType == TmGroupType.Range)
                {
                    var ids = await GetIdsAsync(group.TreeIds, group.TxIds, group.Nandus, group.Zhishidians, group.DateFrom, group.DateTo);
                    if (ids != null && ids.Count > 0)
                    {
                        ids = ListUtils.GetRandomList(ids, 2000);
                        query.WhereIn(nameof(ExamTm.Id), ids);
                    }
                    else
                    {
                        query.Where(nameof(ExamTm.Id), -1);
                    }
                }
            }

            var count = await _repository.CountAsync(query);
            return count;
        }

        public async Task<ExamTm> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            return await _repository.DeleteAsync(id);
        }
        public async Task<int> GetCountByTxIdAsync(int txId)
        {
            return await _repository.CountAsync(Q.Where(nameof(ExamTm.TxId), txId));
        }
        public async Task<int> GetCountByTreeIdAsync(int treeId)
        {
            return await _repository.CountAsync(Q.Where(nameof(ExamTm.TreeId), treeId));
        }
        public async Task<int> GetCountByTreeIdsAsync(List<int> treeIds)
        {
            if (treeIds != null && treeIds.Count() > 0)
            {
                treeIds = ListUtils.GetRandomList(treeIds, 2000);
                return await _repository.CountAsync(Q.WhereIn(nameof(ExamTm.TreeId), treeIds));
            }
            return 0;

        }

        private static Query GetTmGroupQuery(List<int> treeIds, List<int> txIds, List<int> nandus, List<string> zhishidianKeywords, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = Q.NewQuery();

            if (treeIds != null && treeIds.Count > 0)
            {
                treeIds = ListUtils.GetRandomList(treeIds, 2000);
                query.WhereIn(nameof(ExamTm.TreeId), treeIds);
            }
            if (txIds != null && txIds.Count > 0)
            {
                query.WhereIn(nameof(ExamTm.TxId), txIds);
            }
            if (nandus != null && nandus.Count > 0)
            {
                query.WhereIn(nameof(ExamTm.Nandu), nandus);
            }
            if (zhishidianKeywords != null && zhishidianKeywords.Count > 0)
            {
                query.Where(q =>
                {
                    foreach (var zhishidian in zhishidianKeywords)
                    {
                        var like = $"%{zhishidian}%";
                        q.OrWhereLike(nameof(ExamTm.Zhishidian), like);
                    }
                    return q;
                });
            }
            if (dateFrom.HasValue)
            {
                query.Where(nameof(ExamTm.CreatedDate), ">=", DateUtils.ToString(dateFrom));
            }
            if (dateTo.HasValue)
            {
                query.Where(nameof(ExamTm.CreatedDate), "<=", DateUtils.ToString(dateTo));
            }
            return query;
        }
        public async Task<List<int>> GetIdsAsync(List<int> treeIds, List<int> txIds, List<int> nandus, List<string> zhishidianKeywords, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = GetTmGroupQuery(treeIds, txIds, nandus, zhishidianKeywords, dateFrom, dateTo);
            query.WhereNullOrFalse(nameof(ExamTm.Locked));

            return await _repository.GetAllAsync<int>(query.Select(nameof(ExamTm.Id)));
        }

        public async Task<List<ExamTm>> GetListByRandomAsync(AuthorityAuth auth, List<TmGroup> tmGroupList, List<ExamPaperRandomConfig> configList)
        {
            if (configList == null || configList.Count == 0)
            {
                return null;
            }

            var resultList = new List<ExamTm>();

            foreach (var config in configList)
            {
                var txId = config.TxId;
                var nandu1Count = config.Nandu1TmCount;
                var nandu2Count = config.Nandu2TmCount;
                var nandu3Count = config.Nandu3TmCount;
                var nandu4Count = config.Nandu4TmCount;
                var nandu5Count = config.Nandu5TmCount;

                // 如果没有设置任何难度的题目数量，则跳过
                if (nandu1Count + nandu2Count + nandu3Count + nandu4Count + nandu5Count <= 0)
                {
                    continue;
                }

                // 根据题目组列表和占比计算每个题目组的题目数量
                if (tmGroupList != null && tmGroupList.Count > 0)
                {
                    // 计算每个难度级别的题目总数
                    var totalNandu1 = 0;
                    var totalNandu2 = 0;
                    var totalNandu3 = 0;
                    var totalNandu4 = 0;
                    var totalNandu5 = 0;

                    // 根据每个题目组的占比分配题目数量
                    foreach (var tmGroup in tmGroupList)
                    {
                        if (tmGroup.TmIds == null || tmGroup.TmIds.Count == 0)
                        {
                            continue;
                        }

                        // 根据占比计算每个难度级别的题目数量
                        var groupNandu1Count = (int)Math.Ceiling(nandu1Count * tmGroup.Ratio);
                        var groupNandu2Count = (int)Math.Ceiling(nandu2Count * tmGroup.Ratio);
                        var groupNandu3Count = (int)Math.Ceiling(nandu3Count * tmGroup.Ratio);
                        var groupNandu4Count = (int)Math.Ceiling(nandu4Count * tmGroup.Ratio);
                        var groupNandu5Count = (int)Math.Ceiling(nandu5Count * tmGroup.Ratio);

                        // 获取该题目组中各难度级别的题目
                        var nandu1List = await GetListByRandomAsync(auth, false, true, tmGroup.TmIds, txId, groupNandu1Count, 0, 0, 0, 0);
                        var nandu2List = await GetListByRandomAsync(auth, false, true, tmGroup.TmIds, txId, 0, groupNandu2Count, 0, 0, 0);
                        var nandu3List = await GetListByRandomAsync(auth, false, true, tmGroup.TmIds, txId, 0, 0, groupNandu3Count, 0, 0);
                        var nandu4List = await GetListByRandomAsync(auth, false, true, tmGroup.TmIds, txId, 0, 0, 0, groupNandu4Count, 0);
                        var nandu5List = await GetListByRandomAsync(auth, false, true, tmGroup.TmIds, txId, 0, 0, 0, 0, groupNandu5Count);

                        // 将获取到的题目添加到结果列表中
                        if (nandu1List != null && nandu1List.Count > 0)
                        {
                            resultList.AddRange(nandu1List);
                            totalNandu1 += nandu1List.Count;
                        }
                        if (nandu2List != null && nandu2List.Count > 0)
                        {
                            resultList.AddRange(nandu2List);
                            totalNandu2 += nandu2List.Count;
                        }
                        if (nandu3List != null && nandu3List.Count > 0)
                        {
                            resultList.AddRange(nandu3List);
                            totalNandu3 += nandu3List.Count;
                        }
                        if (nandu4List != null && nandu4List.Count > 0)
                        {
                            resultList.AddRange(nandu4List);
                            totalNandu4 += nandu4List.Count;
                        }
                        if (nandu5List != null && nandu5List.Count > 0)
                        {
                            resultList.AddRange(nandu5List);
                            totalNandu5 += nandu5List.Count;
                        }
                    }

                    // 如果题目数量不足，从所有题目中随机抽取补足
                    if (totalNandu1 < nandu1Count)
                    {
                        var remainingCount = nandu1Count - totalNandu1;
                        var additionalList = await GetListByRandomAsync(auth, true, false, null, txId, remainingCount, 0, 0, 0, 0, resultList.Select(s => s.Id).ToList());
                        if (additionalList != null && additionalList.Count > 0)
                        {
                            resultList.AddRange(additionalList);
                        }
                    }
                    if (totalNandu2 < nandu2Count)
                    {
                        var remainingCount = nandu2Count - totalNandu2;
                        var additionalList = await GetListByRandomAsync(auth, true, false, null, txId, 0, remainingCount, 0, 0, 0, resultList.Select(s => s.Id).ToList());
                        if (additionalList != null && additionalList.Count > 0)
                        {
                            resultList.AddRange(additionalList);
                        }
                    }
                    if (totalNandu3 < nandu3Count)
                    {
                        var remainingCount = nandu3Count - totalNandu3;
                        var additionalList = await GetListByRandomAsync(auth, true, false, null, txId, 0, 0, remainingCount, 0, 0, resultList.Select(s => s.Id).ToList());
                        if (additionalList != null && additionalList.Count > 0)
                        {
                            resultList.AddRange(additionalList);
                        }
                    }
                    if (totalNandu4 < nandu4Count)
                    {
                        var remainingCount = nandu4Count - totalNandu4;
                        var additionalList = await GetListByRandomAsync(auth, true, false, null, txId, 0, 0, 0, remainingCount, 0, resultList.Select(s => s.Id).ToList());
                        if (additionalList != null && additionalList.Count > 0)
                        {
                            resultList.AddRange(additionalList);
                        }
                    }
                    if (totalNandu5 < nandu5Count)
                    {
                        var remainingCount = nandu5Count - totalNandu5;
                        var additionalList = await GetListByRandomAsync(auth, true, false, null, txId, 0, 0, 0, 0, remainingCount, resultList.Select(s => s.Id).ToList());
                        if (additionalList != null && additionalList.Count > 0)
                        {
                            resultList.AddRange(additionalList);
                        }
                    }
                }
                else
                {
                    // 如果没有题目组，则从所有题目中随机抽取
                    var nandu1List = await GetListByRandomAsync(auth, true, false, null, txId, nandu1Count, 0, 0, 0, 0);
                    var nandu2List = await GetListByRandomAsync(auth, true, false, null, txId, 0, nandu2Count, 0, 0, 0);
                    var nandu3List = await GetListByRandomAsync(auth, true, false, null, txId, 0, 0, nandu3Count, 0, 0);
                    var nandu4List = await GetListByRandomAsync(auth, true, false, null, txId, 0, 0, 0, nandu4Count, 0);
                    var nandu5List = await GetListByRandomAsync(auth, true, false, null, txId, 0, 0, 0, 0, nandu5Count);

                    if (nandu1List != null && nandu1List.Count > 0)
                    {
                        resultList.AddRange(nandu1List);
                    }
                    if (nandu2List != null && nandu2List.Count > 0)
                    {
                        resultList.AddRange(nandu2List);
                    }
                    if (nandu3List != null && nandu3List.Count > 0)
                    {
                        resultList.AddRange(nandu3List);
                    }
                    if (nandu4List != null && nandu4List.Count > 0)
                    {
                        resultList.AddRange(nandu4List);
                    }
                    if (nandu5List != null && nandu5List.Count > 0)
                    {
                        resultList.AddRange(nandu5List);
                    }
                }
            }

            // 对结果列表进行随机排序
            return resultList.OrderBy(order => StringUtils.Guid()).ToList();
        }

        public async Task<List<ExamTm>> GetListByRandomAsync(AuthorityAuth auth, bool allTm, bool hasGroup, List<int> tmIds, int txId, int nandu1Count = 0, int nandu2Count = 0, int nandu3Count = 0, int nandu4Count = 0, int nandu5Count = 0, List<int> noTmIds = null)
        {
            var query = Q.
                   WhereNullOrFalse(nameof(ExamTm.Locked)).
                   Where(nameof(ExamTm.TxId), txId);

            if (!allTm && hasGroup)
            {
                if (tmIds == null || tmIds.Count == 0)
                {
                    return null;
                }
                else
                {
                    query.WhereIn(nameof(ExamTm.Id), tmIds);
                }
            }
            if(noTmIds!=null && noTmIds.Count > 0)
            {
                query.WhereNotIn(nameof(ExamTm.Id), noTmIds);
            }

            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(ExamTmGroup.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(ExamTmGroup.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(ExamTmGroup.CreatorId), auth.AdminId);
            }


            if (txId > 0)
            {
                query.Where(nameof(ExamTm.TxId), txId);
            }

            if (nandu1Count > 0)
            {
                query.Where(nameof(ExamTm.Nandu), 1);

                var list = await _repository.GetAllAsync(query);
                return list.OrderBy(order => StringUtils.Guid()).Take(nandu1Count).ToList();
            }
            if (nandu2Count > 0)
            {
                query.Where(nameof(ExamTm.Nandu), 2);

                var list = await _repository.GetAllAsync(query);
                return list.OrderBy(order => StringUtils.Guid()).Take(nandu2Count).ToList();
            }
            if (nandu3Count > 0)
            {
                query.Where(nameof(ExamTm.Nandu), 3);

                var list = await _repository.GetAllAsync(query);
                return list.OrderBy(order => StringUtils.Guid()).Take(nandu3Count).ToList();
            }
            if (nandu4Count > 0)
            {
                query.Where(nameof(ExamTm.Nandu), 4);

                var list = await _repository.GetAllAsync(query);
                return list.OrderBy(order => StringUtils.Guid()).Take(nandu4Count).ToList();
            }
            if (nandu5Count > 0)
            {
                query.Where(nameof(ExamTm.Nandu), 5);

                var list = await _repository.GetAllAsync(query);
                return list.OrderBy(order => StringUtils.Guid()).Take(nandu5Count).ToList();
            }
            return null;

        }
        public async Task<int> GetCountAsync(AuthorityAuth auth, bool hasGroup, bool allTm, List<int> tmIds, int txId, int nandu)
        {
            var query = Q.
                WhereNullOrFalse(nameof(ExamTm.Locked)).
                Where(nameof(ExamTm.TxId), txId).
                Where(nameof(ExamTm.Nandu), nandu);

            if (!allTm && hasGroup)
            {
                if (tmIds != null && tmIds.Count > 0)
                {
                    query.WhereIn(nameof(ExamTm.Id), tmIds);
                }
                else
                {
                    return 0;
                }
            }

            if (txId > 0)
            {
                query.Where(nameof(ExamTm.TxId), txId);
            }

            if (nandu > 0)
            {
                query.Where(nameof(ExamTm.Nandu), nandu);
            }

            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(ExamTmGroup.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(ExamTmGroup.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(ExamTmGroup.CreatorId), auth.AdminId);
            }

            return await _repository.CountAsync(query);

        }

        public async Task<(int allCount, int addCount, int deleteCount, int lockedCount, int unLockedCount)> GetDataCount(AuthorityAuth auth)
        {
            var total = 0;
            var lockedTotal = 0;
            var unLockedTotal = 0;
            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(ExamTm.CompanyId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamTm.CompanyId), auth.CurManageOrganIds).WhereTrue(nameof(ExamTm.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamTm.CompanyId), auth.CurManageOrganIds).WhereNullOrFalse(nameof(ExamTm.Locked)));
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(ExamTm.DepartmentId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamTm.DepartmentId), auth.CurManageOrganIds).WhereTrue(nameof(ExamTm.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamTm.DepartmentId), auth.CurManageOrganIds).WhereNullOrFalse(nameof(ExamTm.Locked)));
            }
            else
            {
                total = await _repository.CountAsync(Q.Where(nameof(ExamTm.CreatorId), auth.AdminId));
                lockedTotal = await _repository.CountAsync(Q.Where(nameof(ExamTm.CreatorId), auth.AdminId).WhereTrue(nameof(ExamTm.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.Where(nameof(ExamTm.CreatorId), auth.AdminId).WhereNullOrFalse(nameof(ExamTm.Locked)));
            }

            return (total, 0, 0, lockedTotal, unLockedTotal);
        }
    }
}
