using Datory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Core.Services
{
    public partial class AuthManager
    {
        public async Task<bool> HasPermissionsAsync(MenuPermissionType menuPermissionType = MenuPermissionType.Select)
        {
            var admin = await GetAdminAsync();

            if (admin.Auth == AuthorityType.Admin) return true;//超级管理员
            if (admin.Auth == AuthorityType.AdminCompany) return true;//单位管理员

            var menuId = MenuId;
            if (string.IsNullOrEmpty(menuId))
            {
                return false;
            }
            var myMenuIds = new List<string>();
            var myPermissionIds = new List<string>();
            var roleIds = await _databaseManager.AdministratorsInRolesRepository.GetRoleIdsForAdminAsync(admin.Id);
            if (roleIds == null || roleIds.Count == 0)
            {
                return false;
            }
            foreach (var roleId in roleIds)
            {
                var role = await _databaseManager.RoleRepository.GetRoleAsync(roleId);
                if (role != null)
                {
                    myMenuIds.AddRange(role.MenuIds);
                    myPermissionIds.AddRange(role.PermissionIds);
                }
            }
            if (myMenuIds.Count == 0 || !myMenuIds.Contains(menuId))
            {
                return false;
            }

            var permissionName = _settingsManager.GetPermissionId(menuId, menuPermissionType.GetValue());
            if (menuPermissionType != MenuPermissionType.Select && !myPermissionIds.Contains(permissionName))
            {
                return false;
            }
            return true; ;
        }


        public List<Select<string>> AuthorityTypes()
        {
            return ListUtils.GetSelects<AuthorityType>();
        }
        public List<Select<string>> AuthorityTypes(AuthorityAuth auth)
        {
            var list = ListUtils.GetSelects<AuthorityType>();

            if (auth.AuthType == AuthorityType.AdminCompany)
            {
                list = list.Where(s => s.Value != AuthorityType.Admin.GetValue()).ToList();
            }
            if (auth.AuthType == AuthorityType.AdminDepartment)
            {
                list = list.Where(s => s.Value != AuthorityType.Admin.GetValue() && s.Value != AuthorityType.AdminCompany.GetValue()).ToList();
            }
            if (auth.AuthType == AuthorityType.AdminSelf)
            {
                list = list.Where(s => s.Value == AuthorityType.AdminSelf.GetValue()).ToList();
            }

            return list;
        }

        public async Task<AuthorityAuth> GetAuthorityAuth(int adminId = 0)
        {
            var admin = new Administrator();
            if (adminId > 0)
            {
                admin = await _databaseManager.AdministratorRepository.GetByUserIdAsync(adminId);
                if (admin == null)
                {
                    admin = await _databaseManager.AdministratorRepository.GetByUserIdAsync(1);
                }
            }
            else
            {
                admin = await GetAdminAsync();
            }
            var authResult = new AuthorityAuth()
            {
                AuthType = admin.Auth,
                AdminId = admin.Id,
                CurManageOrganId = admin.AuthCurManageOrganId,
                CompanyId = admin.CompanyId,
                DepartmentId = admin.DepartmentId
            };
            if (admin.Auth == AuthorityType.AdminCompany || admin.Auth == AuthorityType.Admin)
            {
                var companyIds = await _databaseManager.OrganCompanyRepository.GetIdsAsync(admin.AuthCurManageOrganId);
                authResult.CurManageOrganIds = companyIds;
                authResult.CompanyId = admin.AuthCurManageOrganId;
                authResult.DepartmentId = admin.CompanyId == admin.AuthCurManageOrganId ? admin.DepartmentId : 0;
            }
            else if (admin.Auth == AuthorityType.AdminDepartment)
            {
                var departmentIds = await _databaseManager.OrganDepartmentRepository.GetIdsAsync(admin.AuthCurManageOrganId);
                authResult.CurManageOrganIds = departmentIds;
                authResult.CompanyId = admin.CompanyId;
                authResult.DepartmentId = admin.DepartmentId;
            }
            return authResult;
        }
    }
}
