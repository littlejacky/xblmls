using Datory;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace XBLMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerProfileController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAuthorityAuth();

            var authSelect = _authManager.AuthorityTypes(auth);
            var organs = await _organManager.GetOrganTreeTableDataAsync(auth);

            if (!string.IsNullOrEmpty(request.UserName))
            {
                var administrator = await _administratorRepository.GetByUserNameAsync(request.UserName);
                var organId = await _organManager.GetAdministratorOrganGuId(administrator);

                return new GetResult
                {
                    UserId = administrator.Id,
                    UserName = administrator.UserName,
                    DisplayName = administrator.DisplayName,
                    AvatarUrl = administrator.AvatarUrl,
                    Mobile = administrator.Mobile,
                    Email = administrator.Email,
                    Auth = administrator.Auth.GetValue(),
                    OrganId = organId,
                    Auths = authSelect,
                    Organs = organs
                };
            }

            return new GetResult()
            {
                Auths = authSelect,
                Organs = organs
            };

        }
    }
}
