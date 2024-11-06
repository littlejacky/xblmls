using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Dto;

namespace XBLMS.Web.Controllers.Admin.Common
{
    public partial class StudyCourseFileLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] IdRequest request)
        {
            var file = await _studyCourseFilesRepository.GetAsync(request.Id);
            if (file == null) return NotFound();
            return new GetResult
            {
                FileName = file.FileName,
                FileUrl = file.Url,
            };
        }
    }
}
