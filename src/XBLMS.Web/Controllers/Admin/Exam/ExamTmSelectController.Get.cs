using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Configuration;
using XBLMS.Core.Utils;
using XBLMS.Core.Utils.Office;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamTmSelectController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetSearchResults>> Get([FromQuery] GetSearchRequest request)
        {
            var auth = await _authManager.GetAuthorityAuth();
            var treeIds = new List<int>();
            if (request.TreeId > 0)
            {
                if (request.TreeIsChildren)
                {
                    treeIds = await _examTmTreeRepository.GetIdsAsync(request.TreeId);
                }
                else
                {
                    treeIds.Add(request.TreeId);
                }
            }
            var group = await _examTmGroupRepository.GetAsync(request.Id);

            var (total, list) = await _examTmRepository.Group_SelectListAsync(auth, group, treeIds, request.TxId, request.Nandu, request.Keyword, request.Order, request.OrderType, request.PageIndex, request.PageSize);
            if (total > 0)
            {
                foreach (var tm in list)
                {
                    await _examManager.GetTmInfo(tm);
                }
            }

            return new GetSearchResults
            {
                Items = list,
                Total = total,
            };
        }
        [RequestSizeLimit(long.MaxValue)]
        [HttpGet, Route(RouteGetIn)]
        public async Task<ActionResult<GetSearchResults>> GetSelect([FromQuery] IdRequest request)
        {
            var group = await _examTmGroupRepository.GetAsync(request.Id);

            var list = new List<ExamTm>();

            if (group.TmIds != null && group.TmIds.Count > 0)
            {
                foreach (var id in group.TmIds)
                {
                    var tm = await _examTmRepository.GetAsync(id);
                    await _examManager.GetTmInfo(tm);
                    list.Add(tm);
                }
                list = list.ToList().OrderBy(tm => tm.Get("TxTaxis")).ToList();
            }

            return new GetSearchResults
            {
                Items = list
            };
        }
    }
}
