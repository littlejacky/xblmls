using System.Collections.Generic;
using XBLMS.Configuration;

namespace XBLMS.Services
{
    public partial interface ISettingsManager
    {
        string GetPermissionId(string menuId, string menuPermissionType);
        List<Menu> GetMenus(bool isAdmin = false, bool isAuth = false, List<string> menuIds = null);
    }
}
