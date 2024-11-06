using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Common
{
    public partial class StudyCourseEvaluationLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] IdRequest request)
        {
            var item = await _studyCourseEvaluationRepository.GetAsync(request.Id);
            var itemList = await _studyCourseEvaluationItemRepository.GetListAsync(request.Id);
            if (itemList != null && itemList.Count > 0) {
                foreach (var itemInfo in itemList)
                {
                    itemInfo.Set("Score", item.MaxStar);
                }
            }
            return new GetResult
            {
                Item = item,
                ItemList = itemList,
            };
        }



    }
}
