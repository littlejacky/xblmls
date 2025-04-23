using Datory;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class ExamPlanRepository : IExamPlanRepository
    {
        private readonly Repository<ExamPlan> _repository;

        public ExamPlanRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ExamPlan>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<int> InsertAsync(ExamPlan item)
        {
            return await _repository.InsertAsync(item);
        }
        public async Task<bool> UpdateAsync(ExamPlan item)
        {
            return await _repository.UpdateAsync(item);
        }
        public async Task<(int total, List<ExamPlan> list)> GetListAsync(AuthorityAuth auth, List<int> treeIds, string keyword, int pageIndex, int pageSize)
        {
            var query = new Query();

            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(ExamPlan.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(ExamPlan.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(ExamPlan.CreatorId), auth.AdminId);
            }



            if (treeIds != null && treeIds.Count > 0)
            {
                query.WhereIn(nameof(ExamPlan.TreeId), treeIds);
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(ExamPlan.Title), like)
                    .OrWhereLike(nameof(ExamPlan.Subject), like)
                );
            }
            query.OrderByDesc(nameof(ExamPlan.Id));

            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }
        public async Task<ExamPlan> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
        public async Task<bool> DeleteAsync(int Id)
        {
            var result = await _repository.DeleteAsync(Id);
            return result;
        }
        public async Task<int> GetCountAsync(List<int> treeIds)
        {
            return await _repository.CountAsync(Q.WhereIn(nameof(ExamPlan.TreeId), treeIds));
        }
        public async Task<int> MaxAsync()
        {
            var maxId = await _repository.MaxAsync(nameof(ExamPlan.Id));
            if (maxId.HasValue) return maxId.Value;
            return 0;
        }
        public async Task<List<int>> GetIdsAsync(List<int> ids, string keyword)
        {
            var query = Q.Select(nameof(ExamPlan.Id));

            if (ids != null && ids.Count > 0)
            {
                query.WhereIn(nameof(ExamPlan.Id), ids);
            }
            else
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(ExamPlan.Title), like)
                    .OrWhereLike(nameof(ExamPlan.Subject), like)
                );
            }
            return await _repository.GetAllAsync<int>(query);
        }
        public async Task<int> GetCerCount(int cerId)
        {
            return await _repository.CountAsync(Q.Where(nameof(ExamPlan.CerId), cerId));
        }
        public async Task<int> GetGroupCount(int groupId)
        {
            var total = 0;
            var allGroupIds = await _repository.GetAllAsync<string>(Q.Select(nameof(ExamPlan.UserGroupIds)));
            var allGroupIdList = ListUtils.ToList(allGroupIds);
            if (allGroupIdList != null)
            {
                foreach (var groupIds in allGroupIdList)
                {
                    if (groupIds != null && groupIds.Contains(groupId.ToString()))
                    {
                        total++;
                    }
                }
            }
            return total;
        }
        public async Task<int> GetTmGroupCount(int groupId)
        {
            var total = 0;

            var query = Q.Select(nameof(ExamPlan.TmGroupIds));

            var allGroupIds = await _repository.GetAllAsync<string>(query);
            var allGroupIdList = ListUtils.ToList(allGroupIds);
            if (allGroupIdList != null)
            {
                foreach (var groupIds in allGroupIdList)
                {
                    if (groupIds != null && groupIds.Contains(groupId.ToString()))
                    {
                        total++;
                    }
                }
            }
            return total;
        }

        public async Task<(int allCount, int addCount, int deleteCount, int lockedCount, int unLockedCount)> GetDataCount(AuthorityAuth auth)
        {
            var total = 0;
            var lockedTotal = 0;
            var unLockedTotal = 0;
            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(ExamPlan.CompanyId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamPlan.CompanyId), auth.CurManageOrganIds).WhereTrue(nameof(ExamPlan.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamPlan.CompanyId), auth.CurManageOrganIds).WhereNullOrFalse(nameof(ExamPlan.Locked)));
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(ExamPlan.DepartmentId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamPlan.DepartmentId), auth.CurManageOrganIds).WhereTrue(nameof(ExamPlan.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamPlan.DepartmentId), auth.CurManageOrganIds).WhereNullOrFalse(nameof(ExamPlan.Locked)));
            }
            else
            {
                total = await _repository.CountAsync(Q.Where(nameof(ExamPlan.CreatorId), auth.AdminId));
                lockedTotal = await _repository.CountAsync(Q.Where(nameof(ExamPlan.CreatorId), auth.AdminId).WhereTrue(nameof(ExamPlan.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.Where(nameof(ExamPlan.CreatorId), auth.AdminId).WhereNullOrFalse(nameof(ExamPlan.Locked)));
            }

            return (total, 0, 0, lockedTotal, unLockedTotal);
        }

        public async Task<List<ExamPlan>> GetActivePlansAsync(DateTime currentDate)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(ExamPlan.Locked), false)
                .Where(nameof(ExamPlan.StartDate), "<=", DateUtils.ToString(currentDate))
                .Where(nameof(ExamPlan.EndDate), ">=", DateUtils.ToString(currentDate))
                .OrderBy(nameof(ExamPlan.Id)));
        }
    }
}
