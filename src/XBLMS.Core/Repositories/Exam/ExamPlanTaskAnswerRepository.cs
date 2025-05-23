using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Core.Repositories
{
    public class ExamPlanTaskAnswerRepository : IExamPlanTaskAnswerRepository
    {
        private readonly Repository<ExamPlanTaskAnswer> _repository;

        public ExamPlanTaskAnswerRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ExamPlanTaskAnswer>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<ExamPlanTaskAnswer> GetAsync(int userId, int tmId, int practiceId)
        {
            return await _repository.GetAsync(Q.
                Where(nameof(ExamPlanTaskAnswer.TmId), tmId).
                Where(nameof(ExamPlanTaskAnswer.PracticeId), practiceId).
                Where(nameof(ExamPlanTaskAnswer.UserId), userId));
        }

        public async Task<int> InsertAsync(ExamPlanTaskAnswer item)
        {
            return await _repository.InsertAsync(item);
        }

        public async Task UpdateAsync(ExamPlanTaskAnswer item)
        {
            await _repository.UpdateAsync(item);
        }
        public async Task DeleteByUserId(int userId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(ExamPlanTaskAnswer.UserId), userId));
        }
        public async Task DeleteByPracticeId(int practiceId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(ExamPlanTaskAnswer.PracticeId), practiceId));
        }
    }
}
