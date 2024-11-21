using Datory;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersGroupController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasPermissionsAsync())
            {
                return this.NoAuth();
            }

            var auth = await _authManager.GetAuthorityAuth();

            var resultGroups = new List<UserGroup>();
            var allGroups = await _userGroupRepository.GetListAsync(auth);

            foreach (var group in allGroups)
            {
                var creator = await _administratorRepository.GetByUserIdAsync(group.CreatorId);
                group.Set("TypeName", group.GroupType.GetDisplayName());
                if (creator != null)
                {
                    group.Set("CreatorId", creator.Id);
                    group.Set("CreatorDisplayName", creator.DisplayName);
                }

                group.UserTotal = await _userRepository.Group_CountWithoutLockedAsync(auth, group);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    if (StringUtils.Contains(group.GroupName,request.Search) || StringUtils.Contains(group.Description,request.Search))
                    {
                        resultGroups.Add(group);
                    }
                }
                else
                {
                    resultGroups.Add(group);
                }


            }

            return new GetResult
            {
                Groups = resultGroups
            };
        }
    }
}
