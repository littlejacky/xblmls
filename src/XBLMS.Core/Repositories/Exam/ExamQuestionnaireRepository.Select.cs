using Datory;
using SqlKata;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Core.Repositories
{
    public partial class ExamQuestionnaireRepository
    {
        public async Task<(int total, List<ExamQuestionnaire> list)> Select_GetListAsync(AuthorityAuth auth, string keyword, int pageIndex, int pageSize)
        {
            var query = Q.WhereNullOrFalse(nameof(ExamPaper.Locked)).WhereTrue(nameof(ExamPaper.IsCourseUse));

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

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(ExamQuestionnaire.Title), like)
                );
            }
            query.OrderByDesc(nameof(ExamQuestionnaire.Id));

            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }

    }
}
