using Datory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Core.Repositories
{
    public class ExamPlanAnswerRepository : IExamPlanAnswerRepository
    {
        private readonly Repository<ExamPlanAnswer> _repository;

        public ExamPlanAnswerRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ExamPlanAnswer>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<ExamPlanAnswer> GetAsync(int userId, int tmId, int practiceId)
        {
            return await _repository.GetAsync(Q.
                Where(nameof(ExamPlanAnswer.TmId), tmId).
                Where(nameof(ExamPlanAnswer.PracticeId), practiceId).
                Where(nameof(ExamPlanAnswer.UserId), userId));
        }

        public async Task<int> InsertAsync(ExamPlanAnswer item)
        {
            return await _repository.InsertAsync(item);
        }

        public async Task UpdateAsync(ExamPlanAnswer item)
        {
            await _repository.UpdateAsync(item);
        }
        public async Task DeleteByUserId(int userId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(ExamPlanAnswer.UserId), userId));
        }
        public async Task DeleteByPracticeId(int practiceId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(ExamPlanAnswer.PracticeId), practiceId));
        }

        public async Task<int> CountByPracticeId(int id)
        {
            return await _repository.CountAsync(Q.Where(nameof(ExamPlanAnswer.PracticeId), id));
        }

        public async Task<List<ExamPlanAnswer>> ListByPracticeId(int id)
        {
            return await _repository.GetAllAsync(Q.Where(nameof(ExamPlanAnswer.PracticeId), id));
        }

        public async Task<decimal> ScoreSumAsync(int startId)
        {
            var scoreList = await _repository.GetAllAsync<decimal>(Q.
                Select(nameof(ExamPlanAnswer.Score)).
                Where(nameof(ExamPlanAnswer.PracticeId), startId));

            if (scoreList != null && scoreList.Count > 0)
            {
                return scoreList.Sum();
            }

            return 0;
        }
        public async Task<decimal> ObjectiveScoreSumAsync(int startId)
        {
            var scoreList = await _repository.GetAllAsync<decimal>(Q.
              Select(nameof(ExamPlanAnswer.Score)).
              Where(nameof(ExamPlanAnswer.ExamTmType), ExamTmType.Objective.GetValue()).
              Where(nameof(ExamPlanAnswer.PracticeId), startId));

            if (scoreList != null && scoreList.Count > 0)
            {
                return scoreList.Sum();
            }

            return 0;
        }
        public async Task<decimal> SubjectiveScoreSumAsync(int startId)
        {
            var scoreList = await _repository.GetAllAsync<decimal>(Q.
             Select(nameof(ExamPlanAnswer.Score)).
             Where(nameof(ExamPlanAnswer.ExamTmType), ExamTmType.Subjective.GetValue()).
             Where(nameof(ExamPlanAnswer.PracticeId), startId));

            if (scoreList != null && scoreList.Count > 0)
            {
                return scoreList.Sum(); 
            }

            return 0;
        }
    }
}
