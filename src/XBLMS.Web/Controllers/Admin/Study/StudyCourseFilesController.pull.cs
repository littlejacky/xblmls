namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseFilesController
    {
        //[HttpPost, Route(RoutePull)]
        //public async Task<ActionResult<BoolResult>> Pull()
        //{
        //    var admin = await _authManager.GetAdminAsync();
        //    if (admin == null || admin.AdminAuth > 1)
        //    {
        //        return Unauthorized();
        //    }

        //    var rootPath = _pathManager.Xlms_GetUploadKejianPath(admin.OrganId.ToString());
        //    await PullFiles(rootPath, 0, admin.OrganId.ToString(), admin);
        //    return new BoolResult { Value = true };
        //}
        //public async Task PullFiles(string currentDirectory, int parentId, string groupName, Administrator admin)
        //{
        //    var groupId = parentId;

        //    var filepaths = DirectoryUtils.GetFilePaths(currentDirectory);
        //    foreach (var filepath in filepaths)
        //    {
        //        var realFileName = PathUtils.GetFileNameWithoutExtension(filepath);
        //        if (realFileName.Contains(Constants.UploadFileConvertFilePlaceHolder)) continue;
        //        if (!await _materialFileRepository.IsExistsAsync(realFileName, groupId, admin.OrganId))
        //        {
        //            var fileType = PathUtils.GetExtension(filepath);

        //            var material = new MaterialFile
        //            {
        //                GroupId = groupId,
        //                Title = realFileName,
        //                FileType = fileType.ToUpper().Remove(0, 1),
        //                Url = _pathManager.GetRootUrlByPath(filepath),
        //                FileSize = FileUtils.GetFileSizeByFilePathForLong(filepath),
        //                Duration = FileUtils.IsPlayer(fileType) ? FileUtils.GetVideoDuration(filepath) : 0,
        //                CompanyId = admin.CompanyId,
        //                OrganId = admin.OrganId,
        //                DepartmentId = admin.DepartmentId,
        //                CreatedUserId = admin.Id,
        //                CreatedUserName = admin.UserName,
        //                Converting = false,
        //                ConvertMsg = "",
        //                ConvertSuccess = true
        //            };
        //            if (FileUtils.IsConvert(material.FileType))
        //            {
        //                material.Converting = true;
        //            }
        //            await _materialFileRepository.InsertAsync(material);

        //            if (FileUtils.IsConvert(material.FileType))
        //            {
        //                _createManager.CreateFileToContentAsync(admin.OrganId, admin.Id, material.Id, material.Title);
        //            }

        //            await _statRepository.AddCountAsync(StatType.FileAdd, admin.DepartmentId, admin.CompanyId, admin.OrganId, admin.Id);
        //            await _authManager.AddAdminLogAsync("同步文件", realFileName);
        //        }
        //    }


        //    var paths = DirectoryUtils.GetDirectoryPaths(currentDirectory);
        //    foreach (var path in paths)
        //    {
        //        var pathName = PathUtils.GetDirectoryName(path, false);
        //        if (pathName.Contains(Constants.UploadFileConvertFilePlaceHolder)) continue;

        //        var group = await _materialGroupRepository.GetAsync(parentId, pathName, admin.OrganId);
        //        if (group == null)
        //        {
        //            groupId = await _materialGroupRepository.InsertAsync(new MaterialGroup
        //            {
        //                MaterialType = MaterialType.Folder,
        //                GroupName = pathName,
        //                ParentId = parentId,
        //                CompanyId = admin.CompanyId,
        //                DepartmentId = admin.DepartmentId,
        //                OrganId = admin.OrganId,
        //                CreatedUserId = admin.Id,
        //                CreatedUserName = admin.UserName
        //            });
        //            await _authManager.AddAdminLogAsync("同步文件夹", groupName);
        //        }
        //        else
        //        {
        //            groupId = group.Id;
        //        }

        //        await PullFiles(path, groupId, pathName, admin);
        //    }

        //}

    }
}
