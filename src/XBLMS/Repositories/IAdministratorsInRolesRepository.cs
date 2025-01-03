using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IAdministratorsInRolesRepository : IRepository
    {
        Task<List<int>> GetRoleIdsForAdminAsync(int adminId);

        Task<List<int>> GetUserIdsInRoleAsync(int roleId);

        Task InsertAsync(AdministratorsInRoles info);

        Task DeleteByRoleIdAsync(int roleId);
        Task DeleteUserAsync(int adminId);
    }
}
