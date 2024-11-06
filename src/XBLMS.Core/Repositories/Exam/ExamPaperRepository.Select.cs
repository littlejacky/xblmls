using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Core.Repositories
{
    public partial class ExamPaperRepository
    {
        public async Task<(int total, List<ExamPaper> list)> Select_GetListAsync(AuthorityAuth auth, string keyWords, int pageIndex, int pageSize)
        {
            var query = Q.
                WhereNullOrFalse(nameof(ExamPaper.Locked)).
                WhereTrue(nameof(ExamPaper.IsCourseUse)).
                WhereNullOrFalse(nameof(ExamPaper.Moni));

            if (auth.AuthType == Enums.AuthorityType.Admin || auth.AuthType == Enums.AuthorityType.AdminCompany)
            {
                query.Where(nameof(ExamPaper.CompanyId), auth.CurManageOrganId);
            }
            else if (auth.AuthType == Enums.AuthorityType.AdminDepartment)
            {
                query.Where(nameof(ExamPaper.DepartmentId), auth.CurManageOrganId);
            }
            else
            {
                query.Where(nameof(ExamPaper.CreatorId), auth.AdminId);
            }


            if (!string.IsNullOrWhiteSpace(keyWords))
            {
                keyWords = $"%{keyWords}%";
                query.WhereLike(nameof(ExamPaper.Title), keyWords);
            }
            query.OrderByDesc(nameof(ExamPaper.Id));

            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }

    }
}
