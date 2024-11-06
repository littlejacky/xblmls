using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using XBLMS.Configuration;
using XBLMS.Repositories;
using XBLMS.Services;
namespace XBLMS.Web.Controllers.Admin.Common
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class StudyCourseFileLayerViewController : ControllerBase
    {
        private const string Route = "common/studyCourseFileLayerView";

        private readonly IAuthManager _authManager;
        private readonly IStudyCourseFilesRepository _studyCourseFilesRepository;

        public StudyCourseFileLayerViewController(IAuthManager authManager, IStudyCourseFilesRepository studyCourseFilesRepository)
        {
            _authManager = authManager;
            _studyCourseFilesRepository = studyCourseFilesRepository;
        }

        public class GetResult
        {
            public string FileName { get; set; }
            public string FileUrl { get; set; }
        }
    }
}
