using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using NPOI.POIFS.Properties;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class ExamPaperTreeRepository
    {
        public async Task<List<ExamPaperTree>> GetListAsync(AuthorityAuth auth)
        {
            var query = Q.NewQuery();
            if(auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(ExamPaperTree.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(ExamPaperTree.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(ExamPaperTree.CreatorId), auth.AdminId);
            }
            return await _repository.GetAllAsync(query.OrderBy(nameof(ExamPaperTree.Id)));

        }
        public async Task<List<ExamPaperTree>> GetListAsync()
        {
            return await GetAllAsync();
        }
        public async Task<List<ExamPaperTree>> GetAllAsync()
        {
            return await _repository.GetAllAsync(Q.OrderBy(nameof(ExamPaperTree.Id)));
        }

        public async Task<ExamPaperTree> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }


        public async Task<List<int>> GetIdsAsync(int id)
        {
            var all = await GetAllAsync();
            var ids = new List<int>();
            ids.Add(id);
            await GetIdsAsync(ids, all, id);
            return ids;
        }

        private async Task GetIdsAsync(List<int> ids, List<ExamPaperTree> all, int parentid)
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
            var result = new List<ExamPaperTree>();
            var info = await GetAsync(id);
            if (info != null)
            {
                result.Add(info);
                await GetPathNamesAsync(result, info.ParentId);
            }
            result = result.OrderBy(d => d.Id).ToList();
            var names = new List<string>();
            if(result.Count > 0)
            {
                foreach (var item in result)
                {
                    names.Add(item.Name);
                }

            }
            return ListUtils.ToString(names, ">"); ;
        }
        public async Task GetPathNamesAsync(List<ExamPaperTree> names, int parentId)
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
