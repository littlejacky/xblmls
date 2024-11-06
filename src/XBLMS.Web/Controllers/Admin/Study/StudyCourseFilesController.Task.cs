using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Dto;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseFilesController
    {
        //[HttpPost, Route(RouteActionsTask)]
        //public async Task<ActionResult<BoolResult>> CreateTask()
        //{
        //    var admin = await _authManager.GetAdminAsync();
        //    if (admin == null || admin.AdminAuth > 1)
        //    {
        //        return Unauthorized();
        //    }
        //    var organId = admin.OrganId;
        //    var list = await _materialFileRepository.GetAllForTaskAsync(organId);
        //    var result = false;
        //    if (list != null && list.Count > 0)
        //    {
        //        foreach (var file in list)
        //        {
        //            if (file.FileType == "TXT" || file.FileType == "DOCX" || file.FileType == "XLSX" || file.FileType == "PDF")
        //            {
        //                _createManager.CreateFileToContentAsync(organId, admin.Id, file.Id, file.Title);
        //                result = true;
        //            }
        //        }
        //    }
        //    return new BoolResult
        //    {
        //        Value = result
        //    };
        //}

        //[HttpPost, Route(RouteActionsTaskConverting)]
        //public async Task<ActionResult<BoolResult>> TaskConverting()
        //{
        //    var admin = await _authManager.GetAdminAsync();
        //    if (admin == null || admin.AdminAuth > 1)
        //    {
        //        return Unauthorized();
        //    }
        //    var organId = admin.OrganId;
        //    var list = await _materialFileRepository.GetAllForTaskAsync(organId, admin.Id);
        //    var result = false;
        //    if (list != null && list.Count > 0)
        //    {
        //        foreach (var file in list)
        //        {
        //            _createManager.CreateFileToContentAsync(organId, admin.Id, file.Id, file.Title);
        //        }
        //    }
        //    return new BoolResult
        //    {
        //        Value = result
        //    };
        //}
    }
}
