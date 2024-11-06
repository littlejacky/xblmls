using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Utils;
namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyPlanController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Delete))
            {
                return this.NoAuth();
            }
            var plan = await _studyPlanRepository.GetAsync(request.Id);
            if (plan != null)
            {
                await _studyPlanRepository.DeleteAsync(plan.Id);
                await _authManager.AddAdminLogAsync("删除计划", $"计划名称：{plan.PlanName}");
            }
            return new BoolResult
            {
                Value = true
            };
        }

    }
}
