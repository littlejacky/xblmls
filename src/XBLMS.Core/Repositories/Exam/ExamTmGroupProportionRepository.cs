using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Core.Repositories
{
    public class ExamTmGroupProportionRepository : IExamTmGroupProportionRepository
    {
        private readonly Repository<ExamTmGroupProportion> _repository;

        public ExamTmGroupProportionRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ExamTmGroupProportion>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;


        public async Task<List<ExamTmGroupProportion>> GetListAsync(int examPaperId)
        {
            var infoList = await _repository.GetAllAsync(
                Q.Where(nameof(ExamTmGroupProportion.ExamPaperId), examPaperId).
                OrderBy(nameof(ExamTmGroupProportion.Id)));
            return infoList;
        }
        public async Task<int> InsertAsync(ExamTmGroupProportion item)
        {
            return await _repository.InsertAsync(item);
        }
        public async Task<bool> UpdateAsync(ExamTmGroupProportion item)
        {
            return await _repository.UpdateAsync(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
        public async Task<int> DeleteByPaperAsync(int examPaperId)
        {
            return await _repository.DeleteAsync(Q.Where(nameof(ExamTmGroupProportion.ExamPaperId), examPaperId));
        }

    }
}
