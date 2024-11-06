using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Web.Controllers.Admin.Common
{
    public partial class SelectAuthOrganController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var admin = await _authManager.GetAdminAsync();

            var listOrgans = new List<OrganTree>();

            if (admin.Auth == Enums.AuthorityType.AdminDepartment)
            {
                listOrgans = await _organManager.GetOrganTreeAuthDepartmentTableDataAsync(admin.DepartmentId);
            }
            else
            {
                listOrgans = await _organManager.GetOrganTreeAuthCompanyTableDataAsync(admin.CompanyId);
            }

            return new GetResult
            {
                Organs = listOrgans,
            };

        }
    }
}
