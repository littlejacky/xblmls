using Datory;
using DocumentFormat.OpenXml.Office2010.Excel;
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
    public partial class ExamPlanRecordRepository : IExamPlanRecordRepository
    {
        private readonly Repository<ExamPlanRecord> _repository;

        public ExamPlanRecordRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ExamPlanRecord>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<int> InsertAsync(ExamPlanRecord item)
        {
            return await _repository.InsertAsync(item);
        }
        public async Task<bool> UpdateAsync(ExamPlanRecord item)
        {
            return await _repository.UpdateAsync(item);
        }
        public async Task<(int total, List<ExamPlanRecord> list)> GetListAsync(AuthorityAuth auth, List<int> treeIds, string keyword, int pageIndex, int pageSize)
        {
            var query = new Query();

            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(ExamPlanRecord.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(ExamPlanRecord.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(ExamPlanRecord.CreatorId), auth.AdminId);
            }



            //if (treeIds != null && treeIds.Count > 0)
            //{
            //    query.WhereIn(nameof(ExamPlanRecord.TreeId), treeIds);
            //}
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(ExamPlanRecord.Title), like)
                    //.OrWhereLike(nameof(ExamPlanRecord.Subject), like)
                );
            }
            query.OrderByDesc(nameof(ExamPlanRecord.Id));

            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }
        public async Task<ExamPlanRecord> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
        public async Task<bool> DeleteAsync(int Id)
        {
            var result = await _repository.DeleteAsync(Id);
            return result;
        }
        //public async Task<int> GetCountAsync(List<int> treeIds)
        //{
        //    return await _repository.CountAsync(Q.WhereIn(nameof(ExamPlanRecord.TreeId), treeIds));
        //}
        public async Task<int> MaxAsync()
        {
            var maxId = await _repository.MaxAsync(nameof(ExamPlanRecord.Id));
            if (maxId.HasValue) return maxId.Value;
            return 0;
        }
        public async Task<List<int>> GetIdsAsync(List<int> ids, string keyword)
        {
            var query = Q.Select(nameof(ExamPlanRecord.Id));

            if (ids != null && ids.Count > 0)
            {
                query.WhereIn(nameof(ExamPlanRecord.Id), ids);
            }
            else
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(ExamPlanRecord.Title), like)
                    //.OrWhereLike(nameof(ExamPlanRecord.Subject), like)
                );
            }
            return await _repository.GetAllAsync<int>(query);
        }
        //public async Task<int> GetCerCount(int cerId)
        //{
        //    return await _repository.CountAsync(Q.Where(nameof(ExamPlanRecord.CerId), cerId));
        //}
        public async Task<int> GetGroupCount(int groupId)
        {
            var total = 0;
            var allGroupIds = await _repository.GetAllAsync<string>(Q.Select(nameof(ExamPlanRecord.UserGroupIds)));
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

            var query = Q.Select(nameof(ExamPlanRecord.TmGroupIds));

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
                total = await _repository.CountAsync(Q.WhereIn(nameof(ExamPlanRecord.CompanyId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamPlanRecord.CompanyId), auth.CurManageOrganIds));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamPlanRecord.CompanyId), auth.CurManageOrganIds));
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(ExamPlanRecord.DepartmentId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamPlanRecord.DepartmentId), auth.CurManageOrganIds));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamPlanRecord.DepartmentId), auth.CurManageOrganIds));
            }
            else
            {
                total = await _repository.CountAsync(Q.Where(nameof(ExamPlanRecord.CreatorId), auth.AdminId));
                lockedTotal = await _repository.CountAsync(Q.Where(nameof(ExamPlanRecord.CreatorId), auth.AdminId));
                unLockedTotal = await _repository.CountAsync(Q.Where(nameof(ExamPlanRecord.CreatorId), auth.AdminId));
            }

            return (total, 0, 0, lockedTotal, unLockedTotal);
        }
    }
}
