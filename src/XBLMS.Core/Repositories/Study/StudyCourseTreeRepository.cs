using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class StudyCourseTreeRepository : IStudyCourseTreeRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<StudyCourseTree> _repository;

        public StudyCourseTreeRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<StudyCourseTree>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(StudyCourseTree item)
        {
            return await _repository.InsertAsync(item);
        }

        public async Task<bool> UpdateAsync(StudyCourseTree item)
        {
            return await _repository.UpdateAsync(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> DeleteAsync(List<int> ids)
        {
            return await _repository.DeleteAsync(Q.WhereIn(nameof(StudyCourseTree.Id), ids)) > 0;
        }

        public async Task<List<StudyCourseTree>> GetListAsync()
        {
            return await GetAllAsync();
        }
        public async Task<List<StudyCourseTree>> GetAllAsync()
        {
            return await _repository.GetAllAsync(Q.OrderBy(nameof(StudyCourseTree.Id)));
        }

        public async Task<StudyCourseTree> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
        public async Task<string> GetPathAsync(int id)
        {
            var path = "";
            var ids = await GetParentIdListAsync(id);
            if (ids != null && ids.Count > 0)
            {
                foreach (var treeid in ids)
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        path = $"'{treeid}'";
                    }
                    else
                    {
                        path += $",'{treeid}'";
                    }
                }
            }
            return path;
        }
        public async Task<List<int>> GetParentIdListAsync(int id)
        {
            var ids = new List<int>();
            ids.Insert(0, id);

            var tree = await GetAsync(id);
            await GetParentIdListAsync(ids, tree);

            return ids;
        }
        private async Task GetParentIdListAsync(List<int> parentIds, StudyCourseTree tree)
        {
            if (tree != null)
            {
                if (tree.ParentId > 0)
                {
                    var parent = await GetAsync(tree.ParentId);
                    if (parent != null)
                    {
                        parentIds.Insert(0, parent.Id);
                        await GetParentIdListAsync(parentIds, parent);
                    }
                }
            }

        }


        public async Task<List<int>> GetIdsAsync(int id)
        {
            var all = await GetAllAsync();
            var ids = new List<int>();
            ids.Add(id);
            await GetIdsAsync(ids, all, id);
            return ids;
        }

        private async Task GetIdsAsync(List<int> ids, List<StudyCourseTree> all, int parentid)
        {
            var children = all.Where(x => x.ParentId == parentid).ToList();
            foreach (var child in children)
            {
                ids.Add(child.Id);
                await GetIdsAsync(ids, all, child.Id);
            }
        }


        public async Task<string> GetPathNamesAsync(int id)
        {
            var result = new List<StudyCourseTree>();
            var info = await GetAsync(id);
            if (info != null)
            {
                result.Add(info);
                await GetPathNamesAsync(result, info.ParentId);
            }
            result = result.OrderBy(d => d.Id).ToList();
            var names = new List<string>();
            if (result.Count > 0)
            {
                foreach (var item in result)
                {
                    names.Add(item.Name);
                }

            }
            return ListUtils.ToString(names, ">"); ;
        }
        public async Task GetPathNamesAsync(List<StudyCourseTree> names, int parentId)
        {
            if (parentId > 0)
            {
                var info = await GetAsync(parentId);
                names.Add(info);
                await GetPathNamesAsync(names, info.ParentId);
            }
        }
    }
}
