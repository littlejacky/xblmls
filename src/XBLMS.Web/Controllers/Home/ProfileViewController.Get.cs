using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace XBLMS.Web.Controllers.Home
{
    public partial class ProfileViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            user = await _organManager.GetUser(user.Id);

            return new GetResult
            {
                User = user
            };
        }
    }
}
