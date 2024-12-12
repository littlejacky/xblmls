using Datory;
using SqlKata;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;


namespace XBLMS.Core.Repositories
{
    public partial class ExamTmRepository
    {
        private void Group_AuthQuery(AuthorityAuth auth, Query query)
        {
            if (auth.AuthType == AuthorityType.AdminCompany || auth.AuthType == AuthorityType.Admin)
            {
                if (auth.CurManageOrganId != 1 || !auth.CurManageOrganIds.Contains(1))
                {
                    query.WhereIn(nameof(User.CompanyId), auth.CurManageOrganIds);
                }
            }
            if (auth.AuthType == AuthorityType.AdminDepartment)
            {
                query.WhereIn(nameof(User.DepartmentId), auth.CurManageOrganIds);
            }
            if (auth.AuthType == AuthorityType.AdminSelf)
            {
                query.Where(nameof(User.CreatorId), auth.AdminId);
            }
        }
        public async Task<List<int>> Group_RangeIdsAsync(AuthorityAuth auth, ExamTmGroup group)
        {
            var query = Q.Select(nameof(ExamTm.Id)).WhereNullOrFalse(nameof(ExamTm.Locked));

            if (group != null)
            {
                if (group.GroupType == TmGroupType.Range)
                {
                    var isRange = false;
                    if (group.DateFrom.HasValue)
                    {
                        isRange = true;
                        query.Where(nameof(ExamTm.CreatedDate), ">=", DateUtils.ToString(group.DateFrom));
                    }
                    if (group.DateTo.HasValue)
                    {
                        isRange = true;
                        query.Where(nameof(ExamTm.CreatedDate), "<=", DateUtils.ToString(group.DateTo));
                    }
                    if (group.TreeIds != null && group.TreeIds.Count > 0)
                    {
                        isRange = true;
                        query.WhereIn(nameof(ExamTm.TreeId), group.TreeIds);
                    }
                    if (group.TxIds != null && group.TxIds.Count > 0)
                    {
                        isRange = true;
                        query.WhereIn(nameof(ExamTm.TxId), group.TxIds);
                    }
                    if (group.Nandus != null && group.Nandus.Count > 0)
                    {
                        isRange = true;
                        query.WhereIn(nameof(ExamTm.Nandu), group.Nandus);
                    }
                    if (group.Zhishidians != null && group.Zhishidians.Count > 0)
                    {
                        isRange = true;
                        query.Where(q =>
                        {
                            foreach (var zhishidian in group.Zhishidians)
                            {
                                var like = $"%{zhishidian}%";
                                q.OrWhereLike(nameof(ExamTm.Zhishidian), like);
                            }
                            return q;
                        });
                    }
                    if (isRange)
                    {
                        return await _repository.GetAllAsync<int>(query);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }
        public async Task<int> Group_CountAsync(AuthorityAuth auth, ExamTmGroup group)
        {
            var query = Q.WhereNullOrFalse(nameof(ExamTm.Locked));

            if (group != null)
            {
                if (group.GroupType == TmGroupType.All)
                {
                    Group_AuthQuery(auth, query);

                    return await _repository.CountAsync(query);
                }
                if (group.GroupType == TmGroupType.Fixed)
                {
                    if (group.TmIds != null && group.TmIds.Count > 0)
                    {
                        return group.TmIds.Count;
                    }
                    return 0;
                }
                if (group.GroupType == TmGroupType.Range)
                {
                    var isRange = false;
                    if (group.DateFrom.HasValue)
                    {
                        isRange = true;
                        query.Where(nameof(ExamTm.CreatedDate), ">=", DateUtils.ToString(group.DateFrom));
                    }
                    if (group.DateTo.HasValue)
                    {
                        isRange = true;
                        query.Where(nameof(ExamTm.CreatedDate), "<=", DateUtils.ToString(group.DateTo));
                    }
                    if (group.TreeIds != null && group.TreeIds.Count > 0)
                    {
                        isRange = true;
                        query.WhereIn(nameof(ExamTm.TreeId), group.TreeIds);
                    }
                    if (group.TxIds != null && group.TxIds.Count > 0)
                    {
                        isRange = true;
                        query.WhereIn(nameof(ExamTm.TxId), group.TxIds);
                    }
                    if (group.Nandus != null && group.Nandus.Count > 0)
                    {
                        isRange = true;
                        query.WhereIn(nameof(ExamTm.Nandu), group.Nandus);
                    }
                    if (group.Zhishidians != null && group.Zhishidians.Count > 0)
                    {
                        isRange = true;
                        query.Where(q =>
                        {
                            foreach (var zhishidian in group.Zhishidians)
                            {
                                var like = $"%{zhishidian}%";
                                q.OrWhereLike(nameof(ExamTm.Zhishidian), like);
                            }
                            return q;
                        });
                    }
                    if (isRange)
                    {
                        return await _repository.CountAsync(query);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            return 0;
        }




        public async Task<(int total, List<ExamTm> list)> Group_SelectListAsync(AuthorityAuth auth, ExamTmGroup group, List<int> treeIds, int txId, int nandu, string keyword, string order, string orderType, int pageIndex, int pageSize)
        {
            var query = Q.WhereNullOrFalse(nameof(ExamTm.Locked));

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

            if (group != null)
            {
                if (group.GroupType == TmGroupType.Fixed)
                {
                    if (group.TmIds != null && group.TmIds.Count > 0)
                    {
                        query.WhereNotIn(nameof(ExamTm.Id), group.TmIds);
                    }
                }
            }


            if (treeIds != null && treeIds.Count > 0)
            {
                query.WhereIn(nameof(ExamTm.TreeId), treeIds);
            }

            if (txId > 0)
            {
                query.Where(nameof(ExamTm.TxId), txId);
            }
            if (nandu > 0)
            {
                query.Where(nameof(ExamTm.Nandu), nandu);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(ExamTm.Title), like)
                    .OrWhereLike(nameof(ExamTm.Zhishidian), like)
                    .OrWhereLike(nameof(ExamTm.Jiexi), like)
                    .OrWhereLike(nameof(ExamTm.Answer), like)
                );
            }

            var count = await _repository.CountAsync(query);


            if (!string.IsNullOrWhiteSpace(order))
            {
                if (order == "nandu")
                {
                    if (orderType == OrderType.asc.ToString())
                    {
                        query.OrderBy(nameof(ExamTm.Nandu));
                    }
                    else
                    {
                        query.OrderByDesc(nameof(ExamTm.Nandu));
                    }
                }
                if (order == "score")
                {
                    if (orderType == OrderType.asc.ToString())
                    {
                        query.OrderBy(nameof(ExamTm.Score));
                    }
                    else
                    {
                        query.OrderByDesc(nameof(ExamTm.Score));
                    }
                }
                if (order == "tx")
                {
                    if (orderType == OrderType.asc.ToString())
                    {
                        query.OrderBy(nameof(ExamTm.TxId));
                    }
                    else
                    {
                        query.OrderByDesc(nameof(ExamTm.TxId));
                    }
                }
            }
            else
            {
                if (orderType == OrderType.asc.ToString())
                {
                    query.OrderBy(nameof(ExamTm.Id));
                }
                else
                {
                    query.OrderByDesc(nameof(ExamTm.Id));
                }
            }
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (count, list);
        }

        public async Task<(int total, List<ExamTm> list)> Group_ListAsync(AuthorityAuth auth, ExamTmGroup group, List<int> treeIds, int txId, int nandu, string keyword, string order, string orderType, bool? locked, int pageIndex, int pageSize)
        {
            var query = Q.NewQuery();

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

            if (group != null)
            {
                if (group.GroupType == TmGroupType.Fixed)
                {
                    if (group.TmIds != null && group.TmIds.Count > 0)
                    {
                        query.WhereIn(nameof(ExamTm.Id), group.TmIds);
                    }
                }
                if (group.GroupType == TmGroupType.Range)
                {
                    if (group.DateFrom.HasValue)
                    {
                        query.Where(nameof(ExamTm.CreatedDate), ">=", DateUtils.ToString(group.DateFrom));
                    }
                    if (group.DateTo.HasValue)
                    {
                        query.Where(nameof(ExamTm.CreatedDate), "<=", DateUtils.ToString(group.DateTo));
                    }
                    if (group.TreeIds != null && group.TreeIds.Count > 0)
                    {
                        query.WhereIn(nameof(ExamTm.TreeId), group.TreeIds);
                    }
                    if (group.TxIds != null && group.TxIds.Count > 0)
                    {
                        query.WhereIn(nameof(ExamTm.TxId), group.TxIds);
                    }
                    if (group.Nandus != null && group.Nandus.Count > 0)
                    {
                        query.WhereIn(nameof(ExamTm.Nandu), group.Nandus);
                    }
                    if (group.Zhishidians != null && group.Zhishidians.Count > 0)
                    {
                        query.Where(q =>
                        {
                            foreach (var zhishidian in group.Zhishidians)
                            {
                                var like = $"%{zhishidian}%";
                                q.OrWhereLike(nameof(ExamTm.Zhishidian), like);
                            }
                            return q;
                        });
                    }
                }
            }

            if (treeIds != null && treeIds.Count > 0)
            {
                query.WhereIn(nameof(ExamTm.TreeId), treeIds);
            }

            if (txId > 0)
            {
                query.Where(nameof(ExamTm.TxId), txId);
            }
            if (nandu > 0)
            {
                query.Where(nameof(ExamTm.Nandu), nandu);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(ExamTm.Title), like)
                    .OrWhereLike(nameof(ExamTm.Zhishidian), like)
                    .OrWhereLike(nameof(ExamTm.Jiexi), like)
                    .OrWhereLike(nameof(ExamTm.Answer), like)
                );
            }

            if (locked.HasValue)
            {
                if (locked.Value)
                {
                    query.WhereTrue(nameof(ExamTm.Locked));
                }
                else
                {
                    query.WhereNullOrFalse(nameof(ExamTm.Locked));
                }
            }

            var count = await _repository.CountAsync(query);


            if (!string.IsNullOrEmpty(order))
            {
                if (order == "nandu")
                {
                    if (orderType == OrderType.asc.ToString())
                    {
                        query.OrderBy(nameof(ExamTm.Nandu));
                    }
                    else
                    {
                        query.OrderByDesc(nameof(ExamTm.Nandu));
                    }
                }
                if (order == "score")
                {
                    if (orderType == OrderType.asc.ToString())
                    {
                        query.OrderBy(nameof(ExamTm.Score));
                    }
                    else
                    {
                        query.OrderByDesc(nameof(ExamTm.Score));
                    }
                }
                if (order == "tx")
                {
                    if (orderType == OrderType.asc.ToString())
                    {
                        query.OrderBy(nameof(ExamTm.TxId));
                    }
                    else
                    {
                        query.OrderByDesc(nameof(ExamTm.TxId));
                    }
                }
            }
            else
            {
                if (orderType == OrderType.asc.ToString())
                {
                    query.OrderBy(nameof(ExamTm.Id));
                }
                else
                {
                    query.OrderByDesc(nameof(ExamTm.Id));
                }
            }
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (count, list);
        }

    }
}
