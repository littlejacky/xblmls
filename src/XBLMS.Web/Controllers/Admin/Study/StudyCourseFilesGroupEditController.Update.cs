using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Configuration;
using XBLMS.Enums;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseFilesGroupEditController
    {
        [HttpPost, Route(RouteUpdate)]
        public async Task<ActionResult<UpdateResult>> Update([FromBody] UpdateRequest request)
        {
            if (_settingsManager.IsSafeMode)
            {
                return this.Error(Constants.ErrorSafe);
            }

            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Update))
            {
                return this.NoAuth();
            }

            var admin = await _authManager.GetAdminAsync();


            var group = await _studyCourseFilesGroupRepository.GetAsync(request.Id);
            if (!DirectoryUtils.IsDirectoryNameCompliant(request.GroupName)) return this.Error("文件夹名称不合法");
            if (group.GroupName != request.GroupName)
            {
                if (await _studyCourseFilesGroupRepository.IsExistsAsync(request.ParentId, request.GroupName))
                {
                    return this.Error("文件夹已存在");
                }

                group.GroupName = request.GroupName;
                await _studyCourseFilesGroupRepository.UpdateAsync(group);
            }
            await _authManager.AddAdminLogAsync("修改文件夹", request.GroupName);


            return new UpdateResult
            {
                Groups = null
            };
        }
    }
}
