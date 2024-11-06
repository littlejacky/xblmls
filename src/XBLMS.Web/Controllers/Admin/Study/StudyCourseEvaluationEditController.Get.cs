using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using XBLMS.Configuration;
using XBLMS.Core.Utils;
using XBLMS.Core.Utils.Office;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseEvaluationEditController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var item = new StudyCourseEvaluation();
            item.Title = "课程评价-" + StringUtils.PadZeroes(await _studyCourseEvaluationRepository.MaxAsync(), 5);

            var itemList = new List<StudyCourseEvaluationItem>();

            if (request.Id > 0)
            {
                item = await _studyCourseEvaluationRepository.GetAsync(request.Id);

                var items = await _studyCourseEvaluationItemRepository.GetListAsync(item.Id);
                if (items != null && items.Count > 0)
                {
                    foreach (var iteminfo in items)
                    {
                        itemList.Add(iteminfo);
                    }
                }
            }
            return new GetResult
            {
                Item = item,
                ItemList = itemList
            };

        }

    }
}
