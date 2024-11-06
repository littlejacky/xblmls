using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Core.Repositories
{
    public class ExamCerRepository : IExamCerRepository
    {
        private readonly Repository<ExamCer> _repository;
        private readonly string _cacheKey;

        public ExamCerRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ExamCer>(settingsManager.Database, settingsManager.Redis);
            _cacheKey = CacheUtils.GetEntityKey(TableName);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<bool> IsExistsAsync(string name)
        {
            var list = await GetListAsync();
            return list.Any(cer => cer.Name == name);
        }

        public async Task<ExamCer> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
        public async Task<List<ExamCer>> GetListAsync(AuthorityAuth auth)
        {
            var query = Q.NewQuery();
            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(ExamCer.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(ExamCer.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(ExamCer.CreatorId), auth.AdminId);
            }


            return await _repository.GetAllAsync(query
                .OrderByDesc(nameof(ExamCer.Id))
            );
        }
        public async Task<List<ExamCer>> GetListAsync()
        {
            return await _repository.GetAllAsync(Q
                .OrderBy(nameof(ExamCer.Id))
            );
        }

        public async Task<int> InsertAsync(ExamCer item)
        {
            return await _repository.InsertAsync(item);
        }

        public async Task UpdateAsync(ExamCer item)
        {
            await _repository.UpdateAsync(item);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
