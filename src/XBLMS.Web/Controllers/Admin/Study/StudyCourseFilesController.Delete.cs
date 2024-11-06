using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Dto;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseFilesController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] IdRequest request)
        {
            var admin = await _authManager.GetAdminAsync();

            var info = await _studyCourseFilesRepository.GetAsync(request.Id);
            if (info == null) return NotFound();
            await _studyCourseFilesRepository.DeleteAsync(info.Id);
            await _authManager.AddAdminLogAsync("删除文件", info.FileName);

            FileUtils.DeleteFileIfExists(info.Url);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
