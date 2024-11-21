using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;

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
