using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Core.Repositories
{
    public class StudyPlanCourseRepository : IStudyPlanCourseRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<StudyPlanCourse> _repository;

        public StudyPlanCourseRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<StudyPlanCourse>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(StudyPlanCourse item)
        {
            return await _repository.InsertAsync(item);
        }

        public async Task<bool> UpdateAsync(StudyPlanCourse item)
        {
            return await _repository.UpdateAsync(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> DeleteByNotIdsAsync(List<int> notIds, int planId)
        {
            return await _repository.DeleteAsync(Q.
                Where(nameof(StudyPlanCourse.PlanId), planId).
                WhereNotIn(nameof(StudyPlanCourse.Id), notIds)) > 0;
        }

        public async Task<StudyPlanCourse> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
        public async Task<StudyPlanCourse> GetAsync(int planId,int courseId)
        {
            var query = Q.Where(nameof(StudyPlanCourse.PlanId), planId).Where(nameof(StudyPlanCourse.CourseId), courseId);
            return await _repository.GetAsync(query);
        }

        public async Task<List<StudyPlanCourse>> GetListAsync(bool isSelect,int planId)
        {
            var query = Q.Where(nameof(StudyPlanCourse.PlanId), planId);

            if (isSelect)
            {
                query.WhereTrue(nameof(StudyPlanCourse.IsSelectCourse));
            }
            else
            {
                query.WhereFalse(nameof(StudyPlanCourse.IsSelectCourse));
            }

            query.OrderByDesc(nameof(StudyPlanCourse.Taxis));
            return await _repository.GetAllAsync(query);
        }


    }
}
