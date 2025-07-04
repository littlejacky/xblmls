using Datory;
using Dm;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public class ExamPlanPracticeRepository : IExamPlanPracticeRepository
    {
        private readonly Repository<ExamPlanPractice> _repository;

        public ExamPlanPracticeRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ExamPlanPractice>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
        public async Task<ExamPlanPractice> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }

        public async Task<(int total, List<ExamPlanPractice> list)> GetListAsync(int userId, string keyWords, string dateFrom, string dateTo, int pageIndex, int pageSize, bool isUnfinished = false)
        {
            var query = Q.Where(nameof(ExamPlanPractice.UserId), userId);

            if(!string.IsNullOrWhiteSpace(keyWords))
            {
                query.WhereLike(nameof(ExamPlanPractice.Title), $"%{keyWords}%");
            }

            if (!string.IsNullOrWhiteSpace(dateFrom))
            {
                query.Where(nameof(ExamPlanPractice.CreatedDate), ">=", TranslateUtils.ToDateTime(dateFrom));
            }

            if (!string.IsNullOrWhiteSpace(dateTo))
            {
                query.Where(nameof(ExamPlanPractice.CreatedDate), "<=", TranslateUtils.ToDateTime(dateTo));
            }

            if (isUnfinished)
            {
                query.WhereNull(nameof(ExamPlanPractice.EndDateTime));
            }

            var count = await _repository.CountAsync(query);

            var list = await _repository.GetAllAsync(query.OrderByDesc(nameof(ExamCer.Id)).ForPage(pageIndex, pageSize));
            return (count, list);
        }

        public async Task<int> InsertAsync(ExamPlanPractice item)
        {
            return await _repository.InsertAsync(item);
        }

        public async Task UpdateAsync(ExamPlanPractice item)
        {
            await _repository.UpdateAsync(item);
        }
        public async Task IncrementAnswerCountAsync(int id)
        {
            await _repository.IncrementAsync(nameof(ExamPlanPractice.AnswerCount), Q.Where(nameof(ExamPlanPractice.Id), id));
        }
        public async Task IncrementRightCountAsync(int id)
        {
            await _repository.IncrementAsync(nameof(ExamPlanPractice.RightCount), Q.Where(nameof(ExamPlanPractice.Id), id));
        }
        public async Task DecrementRightCountAsync(int id)
        {
            await _repository.DecrementAsync(nameof(ExamPlanPractice.RightCount), Q.Where(nameof(ExamPlanPractice.Id), id));
        }

        public async Task DeleteAsync(int userId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(ExamPlanPractice.UserId), userId));
        }

        public async Task<(int answerTotal, int rightTotal, int allAnswerTotal, int allRightTotal, int collectAnswerTotal, int collectRightTotal, int wrongAnswerTotal, int wrongRightTotal)> SumAsync(int userId)
        {
            var answerTotal = await _repository.SumAsync(nameof(ExamPlanPractice.AnswerCount), Q.Where(nameof(ExamPlanPractice.UserId), userId));
            var rightTtoal = await _repository.SumAsync(nameof(ExamPlanPractice.RightCount), Q.Where(nameof(ExamPlanPractice.UserId), userId));


            var allAnswerTotal = await _repository.SumAsync(nameof(ExamPlanPractice.AnswerCount), Q.Where(nameof(ExamPlanPractice.PracticeType), PracticeType.All.GetValue()).Where(nameof(ExamPlanPractice.UserId), userId));
            var allRightTotal = await _repository.SumAsync(nameof(ExamPlanPractice.RightCount), Q.Where(nameof(ExamPlanPractice.PracticeType), PracticeType.All.GetValue()).Where(nameof(ExamPlanPractice.UserId), userId));

            var collectAnswerTotal = await _repository.SumAsync(nameof(ExamPlanPractice.AnswerCount), Q.Where(nameof(ExamPlanPractice.PracticeType), PracticeType.Collect.GetValue()).Where(nameof(ExamPlanPractice.UserId), userId));
            var collectRightTotal = await _repository.SumAsync(nameof(ExamPlanPractice.RightCount), Q.Where(nameof(ExamPlanPractice.PracticeType), PracticeType.Collect.GetValue()).Where(nameof(ExamPlanPractice.UserId), userId));

            var wrongAnswerTotal = await _repository.SumAsync(nameof(ExamPlanPractice.AnswerCount), Q.Where(nameof(ExamPlanPractice.PracticeType), PracticeType.Wrong.GetValue()).Where(nameof(ExamPlanPractice.UserId), userId));
            var wrongRightTotal = await _repository.SumAsync(nameof(ExamPlanPractice.RightCount), Q.Where(nameof(ExamPlanPractice.PracticeType), PracticeType.Wrong.GetValue()).Where(nameof(ExamPlanPractice.UserId), userId));

            return (answerTotal, rightTtoal, allAnswerTotal, allRightTotal, collectAnswerTotal, collectRightTotal, wrongAnswerTotal, wrongRightTotal);
        }

        public async Task IncrementAsync(int id)
        {
            await _repository.IncrementAsync(nameof(ExamPlanPractice.ExamTimeSeconds), Q.Where(nameof(ExamPlanPractice.Id), id), 5);
        }
        public async Task<int> UpdateEndDateTimeAsync(int id, DateTime datetime)
        {
            return await _repository.UpdateAsync(Q.Set(nameof(ExamPlanPractice.EndDateTime), datetime).Where(nameof(ExamPlanPractice.Id), id));
        }

        public async Task<List<ExamPlanPractice>> ListUnfinishAsync()
        {
            return await _repository.GetAllAsync(Q.WhereNull(nameof(ExamPlanPractice.EndDateTime)));
        }

        public async Task<int> CountUserAsync(int recordId)
        {
            return await _repository.CountAsync(Q.
                Where(nameof(ExamPlanPractice.PlanRecordId), recordId).
                GroupBy(nameof(ExamPlanPractice.UserId)));
        }

        public async Task<decimal> GetMaxScoreAsync(int recordId)
        {
            var maxItem = await _repository.GetAsync(Q.
                WhereNotNull(nameof(ExamPlanPractice.EndDateTime)).
                Where(nameof(ExamPlanPractice.PlanRecordId), recordId).
                OrderByDesc(nameof(ExamPlanPractice.Score)).Limit(1));
            if (maxItem != null)
            {
                return maxItem.Score;
            }
            return 0;
        }

        public async Task<decimal> GetMinScoreAsync(int recordId)
        {
            var minItem = await _repository.GetAsync(Q.
                WhereNotNull(nameof(ExamPlanPractice.EndDateTime)).
                Where(nameof(ExamPlanPractice.PlanRecordId), recordId).
                OrderBy(nameof(ExamPlanPractice.Score)).Limit(1));
            if (minItem != null)
            {
                return minItem.Score;
            }
            return 0;
        }

        public async Task<decimal> SumScoreAsync(int recordId)
        {
            var listScore = await _repository.GetAllAsync<decimal>(Q.
                Select(nameof(ExamPlanPractice.Score)).
                WhereNotNull(nameof(ExamPlanPractice.EndDateTime)).
                Where(nameof(ExamPlanPractice.PlanRecordId), recordId));
            if (listScore != null && listScore.Count > 0)
            {
                return listScore.Sum();
            }
            return 0;
        }

        public async Task<int> CountAsync(int recordId)
        {
            return await _repository.CountAsync(Q.
                WhereNotNull(nameof(ExamPlanPractice.EndDateTime)).
                Where(nameof(ExamPlanPractice.PlanRecordId), recordId));
        }

        public async Task<int> CountDistinctAsync(int recordId)
        {
            return await _repository.CountAsync(Q.
                WhereNotNull(nameof(ExamPlanPractice.EndDateTime)).
                Where(nameof(ExamPlanPractice.PlanRecordId), recordId).
                GroupBy(nameof(ExamPlanPractice.UserId)));
        }

        public async Task<decimal> SumScoreDistinctAsync(int recordId)
        {
            var listScore = await _repository.GetAllAsync<decimal>(Q.
                Select(nameof(ExamPlanPractice.Score)).
                WhereNotNull(nameof(ExamPlanPractice.EndDateTime)).
                Where(nameof(ExamPlanPractice.PlanRecordId), recordId).
                GroupBy(nameof(ExamPlanPractice.UserId), nameof(ExamPlanPractice.Score)));
            if (listScore != null && listScore.Count > 0)
            {
                return listScore.Sum();
            }
            return 0;
        }

        public async Task<int> CountByPassAsync(int recordId, int passScore)
        {
            return await _repository.CountAsync(Q.
                Where(nameof(ExamPlanPractice.Score), ">=", passScore).
                WhereNotNull(nameof(ExamPlanPractice.EndDateTime)).
                Where(nameof(ExamPlanPractice.PlanRecordId), recordId));
        }

        public async Task<int> CountByPassDistinctAsync(int recordId, int passScore)
        {
            return await _repository.CountAsync(Q.
                Where(nameof(ExamPlanPractice.Score), ">=", passScore).
                WhereNotNull(nameof(ExamPlanPractice.EndDateTime)).
                Where(nameof(ExamPlanPractice.PlanRecordId), recordId).
                GroupBy(nameof(ExamPlanPractice.UserId)));
        }
    }
}
