using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseFilesController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<ItemResult<StudyCourseFiles>>> Update([FromBody] UpdateReques request)
        {
            var admin = await _authManager.GetAdminAsync();

            var file = await _studyCourseFilesRepository.GetAsync(request.Id);
            file.FileName = request.Title;
            file.GroupId = request.GroupId;
            await _studyCourseFilesRepository.UpdateAsync(file);
            await _authManager.AddAdminLogAsync("修改文件", file.FileName);

            return new ItemResult<StudyCourseFiles> { Item = file };
        }
    }
}
