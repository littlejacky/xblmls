using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Configuration;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseFilesGroupEditController
    {
        [HttpPost, Route(RouteAdd)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] CreateRequest request)
        {
            if (_settingsManager.IsSafeMode)
            {
                return this.Error(Constants.ErrorSafe);
            }

            var admin = await _authManager.GetAdminAsync();

            if (!DirectoryUtils.IsDirectoryNameCompliant(request.GroupName)) return this.Error("文件夹名称不合法");

            if (await _studyCourseFilesGroupRepository.IsExistsAsync(request.ParentId, request.GroupName))
            {
                return this.Error("文件夹已存在");
            }

            await _studyCourseFilesGroupRepository.InsertAsync(new StudyCourseFilesGroup
            {
                GroupName = request.GroupName,
                ParentId = request.ParentId,
                CompanyId = admin.CompanyId,
                DepartmentId = admin.DepartmentId,
                CreatorId = admin.Id
            });

            var path = _pathManager.GetCourseFilesUploadPath(admin.CompanyId.ToString());
            if (request.ParentId > 0)
            {
                var group = await _studyCourseFilesGroupRepository.GetAsync(request.ParentId);
                path = PathUtils.Combine(path, group.GroupName);
            }

            path = PathUtils.Combine(path, request.GroupName);
            DirectoryUtils.CreateDirectoryIfNotExists(path);

            await _authManager.AddAdminLogAsync("新建文件夹", request.GroupName);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
