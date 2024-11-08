using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Dto;
using XBLMS.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using XBLMS.Models;
using System.Text.RegularExpressions;
using XBLMS.Enums;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersGroupRangeController
    {
        [HttpPost, Route(RouteRange)]
        public async Task<ActionResult<BoolResult>> range([FromBody] GetRequest request)
        {
            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Update))
            {
                return this.NoAuth();
            }


            var auth = await _authManager.GetAuthorityAuth();
            var group = await _userGroupRepository.GetAsync(request.GroupId);
            var userIds = new List<int>();

            userIds = request.RangeUserIds;

            if (group.UserIds == null) { group.UserIds = new List<int>(); }
            if (request.Range == 0)//安排
            {
                group.UserIds.AddRange(userIds);
                group.UserIds = group.UserIds.Distinct().ToList();
                await _userGroupRepository.UpdateAsync(group);
                await _authManager.AddAdminLogAsync("安排用户", $"{group.GroupName}");
            }
            else//移出
            {
                group.UserIds = group.UserIds.Where(id => !userIds.Contains(id)).ToList();
                await _userGroupRepository.UpdateAsync(group);
                await _authManager.AddAdminLogAsync("移出用户", $"{group.GroupName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
