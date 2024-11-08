using Datory;
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
    public class StudyCourseUserRepository : IStudyCourseUserRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<StudyCourseUser> _repository;

        public StudyCourseUserRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<StudyCourseUser>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(StudyCourseUser item)
        {
            return await _repository.InsertAsync(item);
        }

        public async Task<bool> UpdateAsync(StudyCourseUser item)
        {
            return await _repository.UpdateAsync(item);
        }
        public async Task<bool> UpdateByCourseAsync(StudyCourse courseInfo)
        {
            return await _repository.UpdateAsync(Q.
                Set(nameof(StudyCourseUser.Locked), courseInfo.Locked).
                Set(nameof(StudyCourseUser.Credit), courseInfo.Credit).
                Set(nameof(StudyCourseUser.Mark), courseInfo.Mark).
                Set(nameof(StudyCourseUser.KeyWords), courseInfo.Name).
                Where(nameof(StudyCourseUser.PlanId), 0).
                Where(nameof(StudyCourseUser.CourseId), courseInfo.Id)) > 0;
        }

        public async Task<bool> DeleteByCourseAsync(int courseId)
        {
            return await _repository.DeleteAsync(Q.Where(nameof(StudyCourseUser.CourseId), courseId)) > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<StudyCourseUser> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }

        public async Task<StudyCourseUser> GetAsync(int userId, int planId, int courseId)
        {
            return await _repository.GetAsync(Q.
                Where(nameof(StudyCourseUser.UserId), userId).
                Where(nameof(StudyCourseUser.PlanId), planId).
                Where(nameof(StudyCourseUser.CourseId), courseId));
        }
        public async Task<(int total, List<StudyCourseUser> list)> GetListAsync(int userId, bool collection, string keyWords, string mark, string orderby, string state, int pageIndex, int pageSize)
        {
            var query = Q.
                Where(nameof(StudyCourseUser.UserId), userId).
                WhereNullOrFalse(nameof(StudyCourseUser.Locked));
            if (collection)
            {
                query.WhereTrue(nameof(StudyCourseUser.Collection));
            }
            if (!string.IsNullOrEmpty(keyWords))
            {
                query.WhereLike(nameof(StudyCourseUser.KeyWords), $"%{keyWords}%");
            }
            if (!string.IsNullOrEmpty(mark))
            {
                query.WhereLike(nameof(StudyCourseUser.Mark), $"%{mark}%");
            }
            if (!string.IsNullOrEmpty(state))
            {
                if (state == "stateOver")
                {
                    query.Where(nameof(StudyCourseUser.State), StudyStatType.Yiwancheng.GetValue());
                }
                if (state == "stateStuding")
                {
                    query.Where(nameof(StudyCourseUser.State), StudyStatType.Xuexizhong.GetValue());
                }
            }
            if (!string.IsNullOrEmpty(orderby))
            {
                if (orderby == "orderbyEvaluation")
                {
                    query.OrderByDesc(nameof(StudyCourseUser.TotalEvaluation));
                }
                if (orderby == "orderbyCount")
                {
                    query.OrderByDesc(nameof(StudyCourseUser.TotalEvaluation));
                }
            }
            else
            {
                query.OrderByDesc(nameof(StudyCourseUser.Id));
            }

            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }

        public async Task<(int total, List<string>)> GetMarkListAsync(int userId)
        {
            var query = Q.
                Select(nameof(StudyCourseUser.Mark)).
                WhereNullOrFalse(nameof(StudyCourseUser.Locked));
            var list = new List<string>();
            var total = 0;
            var markList = await _repository.GetAllAsync<string>(query);
            if (markList != null && markList.Count > 0)
            {
                foreach (var marks in markList)
                {
                    var listMark = ListUtils.ToList(marks);
                    foreach (var mark in listMark)
                    {
                        if (!markList.Contains(mark))
                        {
                            total++;
                            list.Add(mark);
                        }
                    }

                }
                list = list.OrderBy(x => x).ToList();
            }
            return (total, list);

        }
        public async Task<int> GetAvgEvaluationAsync(int courseId, int minStar)
        {
            return await _repository.CountAsync(Q.
                Where(nameof(StudyCourseUser.AvgEvaluation), ">=", minStar).
                Where(nameof(StudyCourseUser.CourseId), courseId));
        }



        public async Task<List<StudyCourseUser>> GetListAsync(int planId,int userId)
        {
            var query = Q.
                Where(nameof(StudyCourseUser.UserId), userId).
                Where(nameof(StudyCourseUser.PlanId), planId);

            var list = await _repository.GetAllAsync(query);
            return list;
        }

        public async Task<(int total,int overTotal)> GetTotalAsync(int userId)
        {
            var query = Q.Where(nameof(StudyCourseUser.UserId), userId).WhereNullOrFalse(nameof(StudyCourseUser.Locked));

            var total = await _repository.CountAsync(query);
            var overTotal = await _repository.CountAsync(query.Where(nameof(StudyCourseUser.State), StudyStatType.Yiwancheng.GetValue()));

            return (total, overTotal);
        }
        public async Task<long> GetTotalDurationAsync(int userId)
        {
            var query = Q.Where(nameof(StudyCourseUser.UserId), userId).WhereNullOrFalse(nameof(StudyCourseUser.Locked));

            var total = await _repository.SumAsync(nameof(StudyCourseUser.TotalDuration));
            return total;
        }
    }
}
