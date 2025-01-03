using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class StudyCourseRepository : IStudyCourseRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<StudyCourse> _repository;

        public StudyCourseRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<StudyCourse>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }
        public async Task<int> InsertAsync(StudyCourse item)
        {
            return await _repository.InsertAsync(item);
        }
        public async Task<int> IncrementTotalUserAsync(int id)
        {
            return await _repository.IncrementAsync(nameof(StudyCourse.TotalUser), Q.Where(nameof(StudyCourse.Id), id));
        }

        public async Task<bool> UpdateAsync(StudyCourse item)
        {
            return await _repository.UpdateAsync(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<StudyCourse> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }


        public async Task<(int total, List<StudyCourse> list)> GetListAsync(AuthorityAuth auth, string keyWords, string type, int treeId, bool children, int pageIndex, int pageSize)
        {
            var query = Q.NewQuery();

            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(StudyCourse.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(StudyCourse.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(StudyCourse.CreatorId), auth.AdminId);
            }


            if (!string.IsNullOrEmpty(type))
            {
                if (type == "online")
                {
                    query.WhereNullOrFalse(nameof(StudyCourse.OffLine));
                }
                if (type == "offline")
                {
                    query.WhereTrue(nameof(StudyCourse.OffLine));
                }
                if (type == "public")
                {
                    query.WhereTrue(nameof(StudyCourse.Public));
                }
            }
            if (!string.IsNullOrEmpty(keyWords))
            {
                query.Where(q => q.WhereLike(nameof(StudyCourse.Name), $"%{keyWords}%").OrWhereLike(nameof(StudyCourse.Mark), $"%{keyWords}%"));
            }
            if (treeId > 0)
            {
                if (children)
                {
                    keyWords = $"'{treeId}'";
                    query.WhereLike(nameof(StudyCourse.TreePath), $"%{keyWords}%");
                }
                else
                {
                    query.Where(nameof(StudyCourse.TreeId), treeId);
                }

            }
            query.OrderByDesc(nameof(StudyCourse.Id));
            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }

        public async Task<(int total, List<StudyCourse> list)> GetListByTeacherAsync(int teacherId, string keyWords, int pageIndex, int pageSize)
        {
            var query = Q.Where(nameof(StudyCourse.TeacherId), teacherId);

            if (!string.IsNullOrEmpty(keyWords))
            {
                query.Where(q => q.WhereLike(nameof(StudyCourse.Name), $"%{keyWords}%").OrWhereLike(nameof(StudyCourse.Mark), $"%{keyWords}%"));
            }

            query.OrderByDesc(nameof(StudyCourse.Id));
            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }
        public async Task<int> MaxAsync()
        {
            var maxId = await _repository.MaxAsync(nameof(StudyCourse.Id));
            if (maxId.HasValue)
            {
                return maxId.Value + 1;
            }
            return 1;
        }

        public async Task<(int total, int count)> CountAsync(int treeId)
        {
            var keyWords = $"'{treeId}'";

            var total = await _repository.CountAsync(Q.WhereLike(nameof(StudyCourse.TreePath), $"%{keyWords}%"));
            var count = await _repository.CountAsync(Q.Where(nameof(StudyCourse.TreeId), treeId));
            return (total, count);
        }

        public async Task<List<string>> GetMarkListAsync()
        {
            var markList = new List<string>();
            var allMark = await _repository.GetAllAsync<string>(Q.Select(nameof(StudyCourse.Mark)));
            if (allMark != null && allMark.Count > 0)
            {
                foreach (var marks in allMark)
                {
                    var listMark = ListUtils.ToList(marks);
                    foreach (var mark in listMark)
                    {
                        if (!markList.Contains(mark))
                        {
                            markList.Add(mark);
                        }
                    }

                }
            }
            return markList;
        }
        public async Task<int> GetPaperUseCount(int paperId)
        {
            return await _repository.CountAsync(Q.Where(nameof(StudyCourse.ExamId), paperId));
        }
        public async Task<int> GetPaperQUseCount(int paperId)
        {
            return await _repository.CountAsync(Q.Where(nameof(StudyCourse.ExamQuestionnaireId), paperId));
        }
        public async Task<int> GetEvaluationUseCount(int eId)
        {
            return await _repository.CountAsync(Q.Where(nameof(StudyCourse.StudyCourseEvaluationId), eId));
        }
        public async Task<(int allCount, int addCount, int deleteCount, int lockedCount, int unLockedCount)> GetDataCount(AuthorityAuth auth)
        {
            var total = 0;
            var lockedTotal = 0;
            var unLockedTotal = 0;
            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(StudyCourse.CompanyId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(StudyCourse.CompanyId), auth.CurManageOrganIds).WhereTrue(nameof(StudyCourse.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(StudyCourse.CompanyId), auth.CurManageOrganIds).WhereNullOrFalse(nameof(StudyCourse.Locked)));
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(StudyCourse.DepartmentId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(StudyCourse.DepartmentId), auth.CurManageOrganIds).WhereTrue(nameof(StudyCourse.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(StudyCourse.DepartmentId), auth.CurManageOrganIds).WhereNullOrFalse(nameof(StudyCourse.Locked)));
            }
            else
            {
                total = await _repository.CountAsync(Q.Where(nameof(StudyCourse.CreatorId), auth.AdminId));
                lockedTotal = await _repository.CountAsync(Q.Where(nameof(StudyCourse.CreatorId), auth.AdminId).WhereTrue(nameof(StudyCourse.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.Where(nameof(StudyCourse.CreatorId), auth.AdminId).WhereNullOrFalse(nameof(StudyCourse.Locked)));
            }

            return (total, 0, 0, lockedTotal, unLockedTotal);
        }

    }
}
