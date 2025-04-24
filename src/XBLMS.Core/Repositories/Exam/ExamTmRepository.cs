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

        public async Task<List<ExamTm>> GetListByRandomAsync(AuthorityAuth auth, List<TmGroup> tmGroupList, List<ExamPaperRandomConfig> configList, ExamPracticeWrong wrong = null)
        {
            if (configList == null || configList.Count == 0)
            {
                return null;
            }

            // 1. 计算错题类别权重 (TreeId -> Wrong Count)
            Dictionary<int, int> wrongCategoryWeights = new Dictionary<int, int>();
            if (wrong != null && wrong.TmIds != null && wrong.TmIds.Count > 0)
            {
                // 假设 TreeId 代表题目类别，查询错题对应的 TreeId
                // 注意：这里需要确保 ExamTm 有 TreeId 属性，并且该查询能正确执行
                var wrongTmDetails = await _repository.GetAllAsync(
                    Q.Select(nameof(ExamTm.Id), nameof(ExamTm.TreeId))
                     .WhereIn(nameof(ExamTm.Id), wrong.TmIds)
                     .WhereNotNull(nameof(ExamTm.TreeId)) // 确保 TreeId 不为空
                     .WhereNullOrFalse(nameof(ExamTm.Locked)) // 考虑题目是否锁定
                );

                wrongCategoryWeights = wrongTmDetails
                    .GroupBy(tm => tm.TreeId) // 按 TreeId 分组
                    .ToDictionary(g => g.Key, g => g.Count()); // 统计每个 TreeId 的错题数量
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

                        // 获取该题目组中各难度级别的题目 - ***需要修改这里的调用***
                        var nandu1List = await GetWeightedRandomListAsync(auth, false, true, tmGroup.TmIds, txId, 1, groupNandu1Count, wrongCategoryWeights);
                        var nandu2List = await GetWeightedRandomListAsync(auth, false, true, tmGroup.TmIds, txId, 2, groupNandu2Count, wrongCategoryWeights);
                        var nandu3List = await GetWeightedRandomListAsync(auth, false, true, tmGroup.TmIds, txId, 3, groupNandu3Count, wrongCategoryWeights);
                        var nandu4List = await GetWeightedRandomListAsync(auth, false, true, tmGroup.TmIds, txId, 4, groupNandu4Count, wrongCategoryWeights);
                        var nandu5List = await GetWeightedRandomListAsync(auth, false, true, tmGroup.TmIds, txId, 5, groupNandu5Count, wrongCategoryWeights);

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

                    // 如果题目数量不足，从所有题目中随机抽取补足 - ***需要修改这里的调用***
                    var excludeIds = resultList.Select(s => s.Id).ToList();
                    if (totalNandu1 < nandu1Count)
                    {
                        var remainingCount = nandu1Count - totalNandu1;
                        var additionalList = await GetWeightedRandomListAsync(auth, true, false, null, txId, 1, remainingCount, wrongCategoryWeights, excludeIds);
                        if (additionalList != null && additionalList.Count > 0) resultList.AddRange(additionalList);
                    }
                    if (totalNandu2 < nandu2Count)
                    {
                        var remainingCount = nandu2Count - totalNandu2;
                        var additionalList = await GetWeightedRandomListAsync(auth, true, false, null, txId, 2, remainingCount, wrongCategoryWeights, excludeIds);
                        if (additionalList != null && additionalList.Count > 0) resultList.AddRange(additionalList);
                    }
                    if (totalNandu3 < nandu3Count)
                    {
                        var remainingCount = nandu3Count - totalNandu3;
                        var additionalList = await GetWeightedRandomListAsync(auth, true, false, null, txId, 3, remainingCount, wrongCategoryWeights, excludeIds);
                        if (additionalList != null && additionalList.Count > 0) resultList.AddRange(additionalList);
                    }
                    if (totalNandu4 < nandu4Count)
                    {
                        var remainingCount = nandu4Count - totalNandu4;
                        var additionalList = await GetWeightedRandomListAsync(auth, true, false, null, txId, 4, remainingCount, wrongCategoryWeights, excludeIds);
                        if (additionalList != null && additionalList.Count > 0) resultList.AddRange(additionalList);
                    }
                    if (totalNandu5 < nandu5Count)
                    {
                        var remainingCount = nandu5Count - totalNandu5;
                        var additionalList = await GetWeightedRandomListAsync(auth, true, false, null, txId, 5, remainingCount, wrongCategoryWeights, excludeIds);
                        if (additionalList != null && additionalList.Count > 0) resultList.AddRange(additionalList);
                    }
                }
                else
                {
                    // 如果没有题目组，则从所有题目中随机抽取 - ***需要修改这里的调用***
                    var nandu1List = await GetWeightedRandomListAsync(auth, true, false, null, txId, 1, nandu1Count, wrongCategoryWeights);
                    var nandu2List = await GetWeightedRandomListAsync(auth, true, false, null, txId, 2, nandu2Count, wrongCategoryWeights);
                    var nandu3List = await GetWeightedRandomListAsync(auth, true, false, null, txId, 3, nandu3Count, wrongCategoryWeights);
                    var nandu4List = await GetWeightedRandomListAsync(auth, true, false, null, txId, 4, nandu4Count, wrongCategoryWeights);
                    var nandu5List = await GetWeightedRandomListAsync(auth, true, false, null, txId, 5, nandu5Count, wrongCategoryWeights);

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

            // 对结果列表进行随机排序 (可选，如果前面加权抽取已保证随机性，这里可以只合并)
            return resultList.OrderBy(order => StringUtils.Guid()).ToList();
        }

        // 新增：加权随机抽样方法
        private async Task<List<ExamTm>> GetWeightedRandomListAsync(AuthorityAuth auth, bool allTm, bool hasGroup, List<int> tmIds, int txId, int nandu, int count, Dictionary<int, int> categoryWeights, List<int> noTmIds = null)
        {
            if (count <= 0) return new List<ExamTm>(); // 如果不需要抽题，直接返回空列表

            var query = Q.
                   WhereNullOrFalse(nameof(ExamTm.Locked)).
                   Where(nameof(ExamTm.Nandu), nandu); // 直接查询指定难度

            // 应用 txId 筛选
            if (txId > 0)
            {
                query.Where(nameof(ExamTm.TxId), txId);
            }

            // 应用题目组筛选 (如果不是 allTm 且有 group)
            if (!allTm && hasGroup)
            {
                if (tmIds == null || tmIds.Count == 0)
                {
                    return new List<ExamTm>(); // 如果组内无题目，返回空
                }
                else
                {
                    // 限制查询范围在给定的 tmIds 内
                    query.WhereIn(nameof(ExamTm.Id), tmIds);
                }
            }

            // 应用排除 ID 筛选
            if (noTmIds != null && noTmIds.Count > 0)
            {
                query.WhereNotIn(nameof(ExamTm.Id), noTmIds);
            }

            // 应用权限筛选 (根据 auth 类型)
            // 注意：这里的权限筛选可能需要基于 ExamTm 的 CompanyId, DepartmentId, CreatorId
            // 请确保 ExamTm 模型有这些字段，或者调整这里的逻辑以匹配你的数据模型
            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                // 假设 ExamTm 有 CompanyId
                query.Where(nameof(ExamTm.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                // 假设 ExamTm 有 DepartmentId
                query.Where(nameof(ExamTm.DepartmentId), auth.CurManageOrganId);
            }
            else // 假设其他情况基于创建者
            {
                // 假设 ExamTm 有 CreatorId
                query.Where(nameof(ExamTm.CreatorId), auth.AdminId);
            }


            // 获取所有符合条件的候选题目
            var candidates = await _repository.GetAllAsync(query);

            if (candidates == null || candidates.Count == 0)
            {
                return new List<ExamTm>(); // 没有候选题目
            }

            // 如果候选题目数量不足所需数量，直接返回打乱顺序的候选题目
            if (candidates.Count <= count)
            {
                return candidates.OrderBy(x => StringUtils.Guid()).ToList();
            }

            // 执行加权随机抽样
            return SelectWeightedRandom(candidates, count, categoryWeights);
        }

        // 新增：加权随机抽样辅助方法
        private List<ExamTm> SelectWeightedRandom(List<ExamTm> candidates, int count, Dictionary<int, int> categoryWeights)
        {
            if (candidates == null || candidates.Count == 0 || count <= 0) return new List<ExamTm>();
            if (count >= candidates.Count) return candidates.OrderBy(x => StringUtils.Guid()).ToList();

            var random = new Random();
            var selected = new List<ExamTm>();

            // 计算每个候选者的权重
            var weightedCandidates = candidates.Select(tm =>
            {
                int weight = 1; // 基础权重
                if (categoryWeights.TryGetValue(tm.TreeId, out int wrongCount))
                {
                    // 权重增加策略：可以根据需要调整，例如错的越多权重增加越多
                    // 简单策略：每错一次，权重+5 (可调)
                    weight += wrongCount * 5;
                }
                return new { Item = tm, Weight = weight };
            }).ToList();

            long totalWeight = weightedCandidates.Sum(wc => (long)wc.Weight);

            for (int i = 0; i < count; i++)
            {
                if (totalWeight <= 0 || !weightedCandidates.Any()) break;

                long randomNumber = (long)(random.NextDouble() * totalWeight);
                long cumulativeWeight = 0;
                int selectedIndex = -1;

                for (int j = 0; j < weightedCandidates.Count; j++)
                {
                    cumulativeWeight += weightedCandidates[j].Weight;
                    if (randomNumber < cumulativeWeight)
                    {
                        selectedIndex = j;
                        break;
                    }
                }

                if (selectedIndex != -1)
                {
                    var chosen = weightedCandidates[selectedIndex];
                    selected.Add(chosen.Item);
                    totalWeight -= chosen.Weight; // 从总权重中移除
                    weightedCandidates.RemoveAt(selectedIndex); // 从候选列表中移除
                }
                else if (weightedCandidates.Any()) // Fallback 以防万一
                {
                    int fallbackIndex = random.Next(weightedCandidates.Count);
                    var chosen = weightedCandidates[fallbackIndex];
                    selected.Add(chosen.Item);
                    totalWeight -= chosen.Weight;
                    weightedCandidates.RemoveAt(fallbackIndex);
                }
                else
                {
                    break; // 没有候选者了
                }
            }

            return selected;
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
            if (noTmIds != null && noTmIds.Count > 0)
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
