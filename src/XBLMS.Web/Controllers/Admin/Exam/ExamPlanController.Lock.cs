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
        [HttpPost, Route(RouteLock)]
        public async Task<ActionResult<BoolResult>> Lock([FromBody] IdRequest request)
        {
            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Update))
            {
                return this.NoAuth();
            }

            var plan = await _examPlanRepository.GetAsync(request.Id);
            if (plan == null) return NotFound();
            plan.Locked = true;

            await _examPlanRepository.UpdateAsync(plan);
            //await _examPaperUserRepository.UpdateLockedAsync(plan.Id, plan.Locked);
            //await _examPaperStartRepository.UpdateLockedAsync(plan.Id, plan.Locked);

            await _authManager.AddAdminLogAsync("锁定考试计划", $"{plan.Title}");
            await _authManager.AddStatLogAsync(StatType.ExamPlanUpdate, "禁用考试计划", plan.Id, plan.Title);

            return new BoolResult
            {
                Value = true
            };
        }

    }
}
