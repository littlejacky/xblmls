using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using XBLMS.Configuration;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ProfileViewController : ControllerBase
    {
        private const string Route = "profileView";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICacheManager _cacheManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrganManager _organManager;

        public ProfileViewController(IAuthManager authManager,
            IPathManager pathManager,
            ICacheManager cacheManager,
            IConfigRepository configRepository,
            IOrganManager organManager,
            IUserRepository userRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _cacheManager = cacheManager;
            _configRepository = configRepository;
            _userRepository = userRepository;
            _organManager = organManager;
        }


        public class GetResult
        {
            public User User { get; set; }
        }
    }
}
