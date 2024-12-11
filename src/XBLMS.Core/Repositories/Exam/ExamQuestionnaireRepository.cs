using Datory;
using SqlKata;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Core.Repositories
{
    public partial class ExamQuestionnaireRepository : IExamQuestionnaireRepository
    {
        private readonly Repository<ExamQuestionnaire> _repository;

        public ExamQuestionnaireRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ExamQuestionnaire>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;


        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }
        public async Task<int> InsertAsync(ExamQuestionnaire item)
        {
            return await _repository.InsertAsync(item);
        }
        public async Task<bool> UpdateAsync(ExamQuestionnaire item)
        {
            return await _repository.UpdateAsync(item);
        }
        public async Task<(int total, List<ExamQuestionnaire> list)> GetListAsync(AuthorityAuth auth, string keyword, int pageIndex, int pageSize)
        {
            var query = new Query();

            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(ExamPaper.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(ExamPaper.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(ExamPaper.CreatorId), auth.AdminId);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(ExamQuestionnaire.Title), like)
                );
            }
            query.OrderByDesc(nameof(ExamQuestionnaire.Id));

            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }
        public async Task<ExamQuestionnaire> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
        public async Task<ExamQuestionnaire> GetAsync(string guId)
        {
            return await _repository.GetAsync(Q.Where(nameof(ExamQuestionnaire.Guid), guId));
        }
        public async Task<bool> DeleteAsync(int Id)
        {
            var result = await _repository.DeleteAsync(Id);
            return result;
        }

        public async Task<List<int>> GetIdsAsync(List<int> ids, string keyword)
        {
            var query = Q.Select(nameof(ExamQuestionnaire.Id));

            if (ids != null && ids.Count > 0)
            {
                query.WhereIn(nameof(ExamQuestionnaire.Id), ids);
            }
            else
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(ExamQuestionnaire.Title), like)
                );
            }
            return await _repository.GetAllAsync<int>(query);
        }


        public async Task<int> MaxIdAsync()
        {
            var maxId= await _repository.MaxAsync(nameof(ExamQuestionnaire.Id));
            if (maxId.HasValue)
            {
                return maxId.Value;
            }
            return 0;
        }

        public async Task IncrementAsync(int id)
        {
            await _repository.IncrementAsync(nameof(ExamQuestionnaire.AnswerTotal),Q.Where(nameof(ExamQuestionnaire.Id), id));
        }

        public async Task<(int allCount, int addCount, int deleteCount, int lockedCount, int unLockedCount)> GetDataCount(AuthorityAuth auth)
        {
            var total = 0;
            var lockedTotal = 0;
            var unLockedTotal = 0;
            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(ExamQuestionnaire.CompanyId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamQuestionnaire.CompanyId), auth.CurManageOrganIds).WhereTrue(nameof(ExamQuestionnaire.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamQuestionnaire.CompanyId), auth.CurManageOrganIds).WhereNullOrFalse(nameof(ExamQuestionnaire.Locked)));
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                total = await _repository.CountAsync(Q.WhereIn(nameof(ExamQuestionnaire.DepartmentId), auth.CurManageOrganIds));
                lockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamQuestionnaire.DepartmentId), auth.CurManageOrganIds).WhereTrue(nameof(ExamQuestionnaire.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.WhereIn(nameof(ExamQuestionnaire.DepartmentId), auth.CurManageOrganIds).WhereNullOrFalse(nameof(ExamQuestionnaire.Locked)));
            }
            else
            {
                total = await _repository.CountAsync(Q.Where(nameof(ExamQuestionnaire.CreatorId), auth.AdminId));
                lockedTotal = await _repository.CountAsync(Q.Where(nameof(ExamQuestionnaire.CreatorId), auth.AdminId).WhereTrue(nameof(ExamQuestionnaire.Locked)));
                unLockedTotal = await _repository.CountAsync(Q.Where(nameof(ExamQuestionnaire.CreatorId), auth.AdminId).WhereNullOrFalse(nameof(ExamQuestionnaire.Locked)));
            }

            return (total, 0, 0, lockedTotal, unLockedTotal);
        }
    }
}
