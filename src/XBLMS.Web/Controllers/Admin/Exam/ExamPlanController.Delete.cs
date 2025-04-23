using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Utils;
namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamPlanController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Delete))
            {
                return this.NoAuth();
            }
            var plan = await _examPlanRepository.GetAsync(request.Id);
            if (plan != null)
            {
                await _examPlanRepository.DeleteAsync(plan.Id);
                //await _examManager.ClearRandom(paper.Id, true);

                await _authManager.AddAdminLogAsync("删除考试计划", $"{plan.Title}");
                await _authManager.AddStatLogAsync(StatType.ExamPlanDelete, "删除考试计划", plan.Id, plan.Title);
                await _authManager.AddStatCount(StatType.ExamPlanDelete);

            }
            return new BoolResult
            {
                Value = true
            };
        }

    }
}
