using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Web.Controllers.Home.Study
{
    public partial class StudyPlanInfoController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ItemResult<StudyPlanUser>>> Get([FromQuery] IdRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var planUser = await _studyPlanUserRepository.GetAsync(request.Id);
            await _studyManager.User_GetPlanInfo(planUser, true);

            return new ItemResult<StudyPlanUser>
            {
                Item = planUser
            };
        }
    }
}
