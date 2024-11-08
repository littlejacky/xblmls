using Datory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class StudyCourseRepository
    {
        public async Task<(int total, List<string>)> User_GetPublicMarkListAsync()
        {
            var query = Q.
                Select(nameof(StudyCourse.Mark)).
                WhereTrue(nameof(StudyCourse.Public)).
                WhereNullOrFalse(nameof(StudyCourse.Locked));
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
                        if (!list.Contains(mark))
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
        public async Task<(int total, List<StudyCourse> list)> User_GetPublicListAsync(string keyWords,string mark,string orderby, int pageIndex, int pageSize)
        {
            var query = Q.
                WhereTrue(nameof(StudyCourse.Public)).
                WhereNullOrFalse(nameof(StudyCourse.Locked));
            if (!string.IsNullOrEmpty(keyWords))
            {
                query.WhereLike(nameof(StudyCourse.Name), $"%{keyWords}%");
            }
            if (!string.IsNullOrEmpty(mark))
            {
                query.WhereLike(nameof(StudyCourse.Mark), $"%{mark}%");
            }
            if (!string.IsNullOrEmpty(orderby))
            {
                if (orderby == "orderbyEvaluation")
                {
                    query.OrderByDesc(nameof(StudyCourse.TotalEvaluation));
                }
                if (orderby == "orderbyCount")
                {
                    query.OrderByDesc(nameof(StudyCourse.TotalUser));
                }
            }
            else
            {
                query.OrderByDesc(nameof(StudyCourse.Id));
            }

  
            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }

    }
}
