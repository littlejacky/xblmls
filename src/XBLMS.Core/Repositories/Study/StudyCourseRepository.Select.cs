using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;
using XBLMS.Utils;

namespace XBLMS.Core.Repositories
{
    public partial class StudyCourseRepository
    {
        public async Task<(int total, List<StudyCourse> list)> Select_GetListAsync(AuthorityAuth auth, string keyWords, string type, int pageIndex, int pageSize)
        {
            var query = Q.WhereNullOrFalse(nameof(StudyCourse.Locked));


            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(StudyCourse.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(StudyCourse.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(StudyCourse.CreatorId), auth.AdminId);
            }


            if (!string.IsNullOrEmpty(type))
            {
                if (type == "online")
                {
                    query.WhereNullOrFalse(nameof(StudyCourse.OffLine));
                }
                if (type == "offline")
                {
                    query.WhereTrue(nameof(StudyCourse.OffLine));
                }
                if (type == "public")
                {
                    query.WhereTrue(nameof(StudyCourse.Public));
                }
            }
            if (!string.IsNullOrEmpty(keyWords))
            {
                query.Where(q => q.WhereLike(nameof(StudyCourse.Name), $"%{keyWords}%").OrWhereLike(nameof(StudyCourse.Mark), $"%{keyWords}%"));
            }

            query.OrderByDesc(nameof(StudyCourse.Id));
            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }

    }
}
