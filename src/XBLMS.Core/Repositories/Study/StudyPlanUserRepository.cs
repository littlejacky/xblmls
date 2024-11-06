using Datory;
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

        public async Task<bool> ExistsAsync(int planId,int userId)
        {
            return await _repository.ExistsAsync(Q.
                Where(nameof(StudyPlanUser.UserId), userId).
                Where(nameof(StudyPlanUser.PlanId),planId));
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


        public async Task<(int total, List<StudyPlanUser> list)> GetListAsync(string keyWords,int pageIndex, int pageSize)
        {
            var query = Q.NewQuery();

            query.OrderByDesc(nameof(StudyPlanUser.Id));
            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }


    }
}
