using Datory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Core.Repositories
{
    public class StudyPlanUserRepository : IStudyPlanUserRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<StudyPlanUser> _repository;

        public StudyPlanUserRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<StudyPlanUser>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<bool> ExistsAsync(int planId, int userId)
        {
            return await _repository.ExistsAsync(Q.
                Where(nameof(StudyPlanUser.UserId), userId).
                Where(nameof(StudyPlanUser.PlanId), planId));
        }

        public async Task<int> InsertAsync(StudyPlanUser item)
        {
            return await _repository.InsertAsync(item);
        }

        public async Task<bool> UpdateAsync(StudyPlanUser item)
        {
            return await _repository.UpdateAsync(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<StudyPlanUser> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
        public async Task<StudyPlanUser> GetAsync(int planId, int userId)
        {
            return await _repository.GetAsync(Q.
                Where(nameof(StudyPlanUser.UserId), userId).
                Where(nameof(StudyPlanUser.PlanId), planId));
        }

        public async Task<(int total, List<StudyPlanUser> list)> GetListAsync(int year, string state, string keyWords, int userId, int pageIndex, int pageSize)
        {
            var query = Q.Where(nameof(StudyPlanUser.UserId), userId).WhereNullOrFalse(nameof(StudyPlanUser.Locked));

            if (year > 0)
            {
                query.Where(nameof(StudyPlanUser.PlanYear), year);
            }
            if (!string.IsNullOrEmpty(state))
            {
                query.Where(nameof(StudyPlanUser.State), state);
            }
            if (!string.IsNullOrEmpty(keyWords))
            {
                query.WhereLike(nameof(StudyPlanUser.KeyWords), $"%{keyWords}%");
            }

            query.OrderByDesc(nameof(StudyPlanUser.Id));
            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }


        public async Task<(decimal totalCredit, decimal totalOverCredit)> GetCreditAsync(int userId)
        {

            decimal totalCredit = 0;
            decimal totalOverCredit = 0;

            var query = Q.Where(nameof(StudyPlanUser.UserId), userId).WhereNullOrFalse(nameof(StudyPlanUser.Locked));
            var list = await _repository.GetAllAsync(query);
            if(list!=null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    totalCredit += item.Credit;
                    totalOverCredit += item.TotalCredit;
                }
            }
            return (Math.Round(totalCredit, 2), Math.Round(totalOverCredit, 2));
        }
    }
}
