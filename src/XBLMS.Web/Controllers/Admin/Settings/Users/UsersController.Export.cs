using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Core.Utils.Office;
using XBLMS.Dto;
using XBLMS.Core.Utils;
using System.Collections.Generic;
using XBLMS.Enums;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] GetRequest request)
        {
            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Export))
            {
                return this.NoAuth();
            }
            var auth = await _authManager.GetAuthorityAuth();

            var organIds = new List<int>();

            if (request.OrganId > 0)
            {
                if (request.OrganType == "company")
                {
                    organIds = await _organManager.GetCompanyIdsAsync(request.OrganId);
                }
                if (request.OrganType == "department")
                {
                    organIds = await _organManager.GetDepartmentIdsAsync(request.OrganId);
                }
                if (request.OrganType == "duty")
                {
                    organIds = await _organManager.GetDutyIdsAsync(request.OrganId);
                }
            }

            var group = await _userGroupRepository.GetAsync(request.GroupId);

            var ids = await _userRepository.Group_UserIdsAsync(auth, group, organIds, request.OrganType, request.LastActivityDate, request.Keyword, request.Order);

            var fileName = "用户列表.xlsx";
            var filePath = _pathManager.GetDownloadFilesPath(fileName);

            var excelObject = new ExcelObject(_databaseManager, _pathManager, _organManager, _examManager);
            await excelObject.CreateExcelFileForUsersAsync(ids, filePath);

            var downloadUrl = _pathManager.GetDownloadFilesUrl(fileName);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
