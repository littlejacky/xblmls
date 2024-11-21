using Datory;
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

        public async Task<List<ExamTm>> GetListByRandomAsync(AuthorityAuth auth, bool allTm, bool hasGroup, List<int> tmIds, int txId, int nandu1Count = 0, int nandu2Count = 0, int nandu3Count = 0, int nandu4Count = 0, int nandu5Count = 0)
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
    }
}
