using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersGroupRangeController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResults>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasPermissionsAsync())
            {
                return this.NoAuth();
            }

            var auth = await _authManager.GetAuthorityAuth();

            var organIds = new List<int>();

            if (request.OrganId > 0)
            {
                if (request.OrganType == "company")
                {
                    organIds = await _organManager.GetCompanyIdsAsync(request.OrganId);
                }
                if (request.OrganType == "department")
                {
                    organIds = await _organManager.GetDepartmentIdsAsync(request.OrganId);
                }
                if (request.OrganType == "duty")
                {
                    organIds = await _organManager.GetDutyIdsAsync(request.OrganId);
                }
            }



            var group = await _userGroupRepository.GetAsync(request.GroupId);

            var count = await _userRepository.Group_RangeCountAsync(auth, group.UserIds, organIds, request.OrganType, request.Range, request.LastActivityDate, request.Keyword);
            var users = await _userRepository.Group_RangeUsersAsync(auth, group.UserIds, organIds, request.OrganType, request.Range, request.LastActivityDate, request.Keyword, request.Order, request.Offset, request.Limit);

            return new GetResults
            {
                Users = users,
                Count = count,
                GroupName = group.GroupName
            };
        }

        [HttpGet, Route(RouteOtherData)]
        public async Task<ActionResult<GetResults>> GetOtherData()
        {
            var auth = await _authManager.GetAuthorityAuth();

            var organs = await _organManager.GetOrganTreeTableDataAsync(auth);
            return new GetResults
            {
                Organs = organs,
            };
        }
    }
}
