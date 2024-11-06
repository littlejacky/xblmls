using DocumentFormat.OpenXml.Drawing.Charts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Configuration;
using XBLMS.Enums;

namespace XBLMS.Core.Services
{
    public partial class AuthManager
    {
        public async Task<List<Menu>> GetMenus()
        {
            var admin = await GetAdminAsync();
            if (admin.Auth == AuthorityType.Admin || admin.Auth == AuthorityType.AdminCompany)
            {
                var menus = _settingsManager.GetMenus(admin.Auth == AuthorityType.Admin, false, null);
                return menus;
            }
            else
            {
                var roles = await _databaseManager.AdministratorsInRolesRepository.GetRoleIdsForAdminAsync(admin.Id);
                var menuIds = new List<string>();
                foreach (var roleId in roles)
                {
                    var role = await _databaseManager.RoleRepository.GetRoleAsync(roleId);
                    if (role != null)
                    {
                        menuIds.AddRange(role.MenuIds);
                    }
                }
                menuIds = menuIds.Distinct().ToList();
                if (menuIds.Count > 0)
                {
                    return _settingsManager.GetMenus(false, false, menuIds);
                }
                return null;
            }
        }
    }
}
