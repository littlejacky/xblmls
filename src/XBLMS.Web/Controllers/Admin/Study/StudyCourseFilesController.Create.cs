using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using XBLMS.Configuration;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseFilesController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Create([FromForm] CreateRequest request, [FromForm] IFormFile file)
        {
            if (_settingsManager.IsSafeMode)
            {
                return this.Error(Constants.ErrorSafe);
            }

            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Add))
            {
                return this.NoAuth();
            }

            try
            {
                var auth = await _authManager.GetAuthorityAuth();

                var fileName = PathUtils.GetFileName(file.FileName);

                var fileType = PathUtils.GetExtension(fileName);
                var config = await _configRepository.GetAsync();

                if (!FileUtils.IsPlayer(fileType))
                {
                    return this.Error(Constants.ErrorUpload);
                }


                var realFileName = PathUtils.GetFileNameWithoutExtension(fileName);
                if (await _studyCourseFilesRepository.IsExistsAsync(realFileName, auth.CompanyId, request.GroupId))
                {
                    return this.Error("文件已存在，请勿重复上传");
                }

                var path = _pathManager.GetCourseFilesUploadPath(auth.CompanyId.ToString());
                if (request.GroupId > 0)
                {
                    var parentIds = await _studyCourseFilesGroupRepository.GetParentIdListAsync(request.GroupId);
                    if (parentIds != null && parentIds.Count > 0)
                    {
                        foreach (var id in parentIds)
                        {
                            var parentGroup = await _studyCourseFilesGroupRepository.GetAsync(id);
                            if (parentGroup != null)
                            {
                                path = PathUtils.Combine(path, parentGroup.GroupName);
                            }
                        }
                    }


                }
                var filePath = PathUtils.Combine(path, fileName);

                await _pathManager.UploadAsync(file, filePath);
                var url = _pathManager.GetRootUrlByPath(filePath);

                var courseFile = new StudyCourseFiles
                {
                    GroupId = request.GroupId,
                    FileName = realFileName,
                    FileType = fileType.ToUpper().Remove(0, 1),
                    Url = url,
                    FileSize = TranslateUtils.ToInt(file.Length.ToString()),
                    Duration = request.Duration,
                    CompanyId = auth.CompanyId,
                    DepartmentId = auth.DepartmentId,
                    CreatorId = auth.AdminId
                };
                await _studyCourseFilesRepository.InsertAsync(courseFile);

                await _authManager.AddAdminLogAsync("上传课件", realFileName);

                return new GetResult { Success = true };
            }
            catch (Exception ex)
            {
                return this.Error(ex.Message);
            }

        }
    }
}
