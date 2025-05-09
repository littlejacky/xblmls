using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSwag.Annotations;
using System.Net.Http;
using XBLMS.Configuration;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AnalysisController : ControllerBase
    {
        private const string Route = "analysis";

        private static string _adminToken;
        private static string _refreshToken;

        private readonly string _supersetUrl;
        private readonly string _username;
        private readonly string _password;
        private readonly string _dashboardId;

        private readonly IHttpClientFactory _httpClientFactory;

        public AnalysisController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;

            _supersetUrl = configuration["Superset:Url"];
            _username = configuration["Superset:Username"];
            _password = configuration["Superset:Password"];
            _dashboardId = configuration["Superset:DashboardId"];
        }

        public class GetResult
        {
            public string GuestToken { get; set; }
        }
    }
}
