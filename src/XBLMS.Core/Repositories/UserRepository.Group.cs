using Datory;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class UserRepository
    {
        public async Task<int> Group_CountWithoutLockedAsync(AuthorityAuth auth, UserGroup group)
        {
            var query = Q.WhereNullOrFalse(nameof(User.Locked));

            if (group.GroupType == UsersGroupType.All)
            {
                Group_AuthQuery(auth, query);

                return await _repository.CountAsync(query);
            }
            if (group.GroupType == UsersGroupType.Fixed)
            {
                if (group.UserIds != null && group.UserIds.Count > 0)
                {
                    return group.UserIds.Count;
                }
                return 0;
            }
            if (group.GroupType == UsersGroupType.Range)
            {
                var isRange = false;
                if (group.DutyIds != null && group.DutyIds.Count > 0)
                {
                    query.WhereIn(nameof(User.DutyId), group.DutyIds);
                    isRange = true;
                }
                if (group.DepartmentIds != null && group.DepartmentIds.Count > 0)
                {
                    query.WhereIn(nameof(User.DepartmentId), group.DepartmentIds);
                    isRange = true;
                }
                if (group.CompanyIds != null && group.CompanyIds.Count > 0)
                {
                    query.WhereIn(nameof(User.CompanyId), group.CompanyIds);
                    isRange = true;
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


            return 0;
        }

        public async Task<List<int>> Group_IdsWithoutLockedAsync(AuthorityAuth auth, UserGroup group)
        {
            var query = Q.Select(nameof(User.Id));
            query.WhereNullOrFalse(nameof(User.Locked));


            if (group.GroupType == UsersGroupType.All)
            {
                Group_AuthQuery(auth, query);

                return await _repository.GetAllAsync<int>(query);
            }
            if (group.GroupType == UsersGroupType.Fixed)
            {
                if (group.UserIds != null && group.UserIds.Count > 0)
                {
                    query.WhereIn(nameof(User.Id), group.UserIds);
                    return await _repository.GetAllAsync<int>(query);
                }
                return null;
            }
            if (group.GroupType == UsersGroupType.Range)
            {
                var isRange = false;
                if (group.DutyIds != null && group.DutyIds.Count > 0)
                {
                    query.WhereIn(nameof(User.DutyId), group.DutyIds);
                    isRange = true;
                }
                if (group.DepartmentIds != null && group.DepartmentIds.Count > 0)
                {
                    query.WhereIn(nameof(User.DepartmentId), group.DepartmentIds);
                    isRange = true;
                }
                if (group.CompanyIds != null && group.CompanyIds.Count > 0)
                {
                    query.WhereIn(nameof(User.CompanyId), group.CompanyIds);
                    isRange = true;
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
            return null;
        }
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
        private Query Group_RangeQuery(AuthorityAuth auth, List<int> groupUserIds, List<int> organIds, string organType, int range, int dayOfLastActivity, string keyword)
        {
            var query = Q.NewQuery();

            Group_AuthQuery(auth, query);

            if (!string.IsNullOrEmpty(organType) && organIds != null && organIds.Count > 0)
            {
                if (organType == "company")
                {
                    if (!organIds.Contains(1))
                    {
                        query.WhereIn(nameof(Administrator.CompanyId), organIds);
                    }
                }
                if (organType == "department")
                {
                    query.WhereIn(nameof(Administrator.DepartmentId), organIds);
                }
                if (organType == "duty")
                {
                    query.WhereIn(nameof(Administrator.DutyId), organIds);
                }
            }

            if (range == 0)
            {
                if (groupUserIds != null && groupUserIds.Count > 0)
                {
                    query.WhereNotIn(nameof(User.Id), groupUserIds);
                }

            }
            else
            {
                if (groupUserIds != null && groupUserIds.Count > 0)
                {
                    query.WhereIn(nameof(User.Id), groupUserIds);
                }
                else
                {
                    query.Where(nameof(User.Id), -1);
                }
            }



            if (dayOfLastActivity > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                query.Where(nameof(User.LastActivityDate), ">=", DateUtils.ToString(dateTime));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(User.UserName), like)
                    .OrWhereLike(nameof(User.Email), like)
                    .OrWhereLike(nameof(User.Mobile), like)
                    .OrWhereLike(nameof(User.DisplayName), like)
                );
            }
            return query;
        }
        public async Task<int> Group_RangeCountAsync(AuthorityAuth auth, List<int> groupUserIds, List<int> organIds, string organType, int range, int dayOfLastActivity, string keyword)
        {
            var query = Group_RangeQuery(auth, groupUserIds, organIds, organType, range, dayOfLastActivity, keyword);
            return await _repository.CountAsync(query);
        }
        public async Task<List<User>> Group_RangeUsersAsync(AuthorityAuth auth, List<int> groupUserIds, List<int> organIds, string organType, int range, int dayOfLastActivity, string keyword, string order, int offset, int limit)
        {
            var query = Group_RangeQuery(auth, groupUserIds, organIds, organType, range, dayOfLastActivity, keyword);

            if (!string.IsNullOrEmpty(order))
            {
                if (StringUtils.EqualsIgnoreCase(order, nameof(User.UserName)))
                {
                    query.OrderBy(nameof(User.UserName));
                }
                else
                {
                    query.OrderByDesc(order);
                }
            }
            else
            {
                query.OrderByDesc(nameof(User.Id));
            }

            query.Offset(offset).Limit(limit);

            return await _repository.GetAllAsync(query);
        }


        private Query Group_Query(AuthorityAuth auth, UserGroup group, List<int> organIds, string organType, int dayOfLastActivity, string keyword)
        {
            var query = Q.NewQuery();

            Group_AuthQuery(auth, query);


            if (!string.IsNullOrEmpty(organType) && organIds != null && organIds.Count > 0)
            {
                if (organType == "company")
                {
                    if (!organIds.Contains(1))
                    {
                        query.WhereIn(nameof(Administrator.CompanyId), organIds);
                    }
                }
                if (organType == "department")
                {
                    query.WhereIn(nameof(Administrator.DepartmentId), organIds);
                }
                if (organType == "duty")
                {
                    query.WhereIn(nameof(Administrator.DutyId), organIds);
                }
            }

            if (group != null)
            {
                if (group.GroupType == UsersGroupType.Fixed)
                {
                    if (group.UserIds != null && group.UserIds.Count > 0)
                    {
                        query.WhereIn(nameof(User.Id), group.UserIds);
                    }
                    else
                    {
                        query.Where(nameof(User.Id), -1);
                        return query;
                    }

                }
                if (group.GroupType == UsersGroupType.Range)
                {
                    var isRange = false;
                    if (group.DutyIds != null && group.DutyIds.Count > 0)
                    {
                        query.WhereIn(nameof(User.DutyId), group.DutyIds);
                        isRange = true;
                    }
                    if (group.DepartmentIds != null && group.DepartmentIds.Count > 0)
                    {
                        query.WhereIn(nameof(User.DepartmentId), group.DepartmentIds);
                        isRange = true;
                    }
                    if (group.CompanyIds != null && group.CompanyIds.Count > 0)
                    {
                        query.WhereIn(nameof(User.CompanyId), group.CompanyIds);
                        isRange = true;
                    }
                    if (!isRange)
                    {
                        query.Where(nameof(User.Id), -1);
                        return query;
                    }
                }
            }


            if (dayOfLastActivity > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                query.Where(nameof(User.LastActivityDate), ">=", DateUtils.ToString(dateTime));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(User.UserName), like)
                    .OrWhereLike(nameof(User.Email), like)
                    .OrWhereLike(nameof(User.Mobile), like)
                    .OrWhereLike(nameof(User.DisplayName), like)
                );
            }
            return query;
        }

        public async Task<int> Group_CountAsync(AuthorityAuth auth, UserGroup group, List<int> organIds, string organType, int dayOfLastActivity, string keyword)
        {
            var query = Group_Query(auth, group, organIds, organType, dayOfLastActivity, keyword);
            return await _repository.CountAsync(query);
        }
        public async Task<List<User>> Group_UsersAsync(AuthorityAuth auth, UserGroup group, List<int> organIds, string organType, int dayOfLastActivity, string keyword, string order, int offset, int limit)
        {
            var query = Group_Query(auth, group, organIds, organType, dayOfLastActivity, keyword);

            if (!string.IsNullOrEmpty(order))
            {
                if (StringUtils.EqualsIgnoreCase(order, nameof(User.UserName)))
                {
                    query.OrderBy(nameof(User.UserName));
                }
                else
                {
                    query.OrderByDesc(order);
                }
            }
            else
            {
                query.OrderByDesc(nameof(User.Id));
            }

            query.Offset(offset).Limit(limit);

            return await _repository.GetAllAsync(query);
        }

        public async Task<List<int>> Group_UserIdsAsync(AuthorityAuth auth, UserGroup group, List<int> organIds, string organType, int dayOfLastActivity, string keyword, string order)
        {
            var query = Group_Query(auth, group, organIds, organType, dayOfLastActivity, keyword);

            if (!string.IsNullOrEmpty(order))
            {
                if (StringUtils.EqualsIgnoreCase(order, nameof(User.UserName)))
                {
                    query.OrderBy(nameof(User.UserName));
                }
                else
                {
                    query.OrderByDesc(order);
                }
            }
            else
            {
                query.OrderByDesc(nameof(User.Id));
            }

            query.Select(nameof(User.Id));

            return await _repository.GetAllAsync<int>(query);
        }
    }
}

