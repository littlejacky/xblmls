using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SqlKata;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public class StudyCourseFilesRepository : IStudyCourseFilesRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<StudyCourseFiles> _repository;

        public StudyCourseFilesRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<StudyCourseFiles>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(StudyCourseFiles file)
        {
            return await _repository.InsertAsync(file);
        }

        public async Task<bool> UpdateAsync(StudyCourseFiles file)
        {
            return await _repository.UpdateAsync(file);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var file = await GetAsync(id);
            if (file != null && !string.IsNullOrEmpty(file.Url))
            {
                var filePath = PathUtils.Combine(_settingsManager.WebRootPath, file.Url);
                FileUtils.DeleteFileIfExists(filePath);
            }

            return await _repository.DeleteAsync(id);
        }
        private static Query GetQ(AuthorityAuth auth, int groupId, string keyword, int organId)
        {
            var query = Q.NewQuery();

            query.Where(nameof(StudyCourseFiles.GroupId), groupId);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query.WhereLike(nameof(StudyCourseFiles.FileName), $"%{keyword}%");
            }
            return query;
        }
        public async Task<List<StudyCourseFiles>> GetAllAsync(string keyWords)
        {
            var query = Q.
                WhereLike(nameof(StudyCourseFiles.FileName), $"%{keyWords}%").
                OrderBy(nameof(StudyCourseFiles.CreatedDate));
            return await _repository.GetAllAsync(query);
        }
        public async Task<List<StudyCourseFiles>> GetAllAsync(int groupId)
        {
            var query = Q.Where(nameof(StudyCourseFiles.GroupId), groupId).OrderBy(nameof(StudyCourseFiles.CreatedDate));
            return await _repository.GetAllAsync(query);
        }

        public async Task<List<StudyCourseFiles>> GetAllAsync(AuthorityAuth auth, int groupId, string keyword, int organId)
        {
            return await _repository.GetAllAsync(GetQ(auth, groupId, keyword, organId));
        }


        public async Task<List<int>> GetIdsAsync(int groupId, string keyword, int organId)
        {
            var query = Q.Select(nameof(StudyCourseFiles.Id));
            query.Where(nameof(StudyCourseFiles.GroupId), groupId);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query.WhereLike(nameof(StudyCourseFiles.FileName), $"%{keyword}%");
            }

            return await _repository.GetAllAsync<int>(query);
        }

        public async Task<StudyCourseFiles> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }

        public async Task<bool> IsExistsAsync(string fileName, int companyId, int groupId)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(StudyCourseFiles.CompanyId), companyId)
                .Where(nameof(StudyCourseFiles.GroupId), groupId)
                .Where(nameof(StudyCourseFiles.FileName), fileName));
        }



        public async Task<int> CountAsync()
        {
            var query = Q.NewQuery();
            var total = await _repository.CountAsync(query);
            return total;
        }

        public async Task<long> SumFileSizeAsync(List<int> groupIds)
        {
            long result = 0;
            if (groupIds != null && groupIds.Count > 0)
            {
                var list = await _repository.GetAllAsync(Q.WhereIn(nameof(StudyCourseFiles.GroupId), groupIds));
                if (list != null && list.Count > 0)
                {
                    list.ForEach((file) =>
                    {
                        result += file.FileSize;
                    });
                }
            }

            return result;
        }

    }
}
