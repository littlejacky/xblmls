﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamTmController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetSearchResults>> GetSearch([FromQuery] GetSearchRequest request)
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

            var group = await _examTmGroupRepository.GetAsync(request.TmGroupId);

            var (total, list) = await _examTmRepository.Group_ListAsync(auth, group, treeIds, request.TxId, request.Nandu, request.Keyword, request.Order, request.OrderType, request.IsStop, request.PageIndex, request.PageSize);
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

        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Delete))
            {
                return this.NoAuth();
            }

            var tm = await _examTmRepository.GetAsync(request.Id);
            if (tm == null) return this.NotFound();
            await _examTmRepository.DeleteAsync(request.Id);

            await _authManager.AddAdminLogAsync("删除题目", StringUtils.StripTags(tm.Title));
            await _authManager.AddStatLogAsync(StatType.ExamTmDelete, "删除题目", tm.Id, StringUtils.StripTags(tm.Title), tm);
            await _authManager.AddStatCount(StatType.ExamTmDelete);

            return new BoolResult
            {
                Value = true
            };
        }
        [HttpPost, Route(RouteDeleteSearch)]
        public async Task<ActionResult<BoolResult>> DeleteSearch([FromBody] GetDeletesRequest request)
        {
            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Delete))
            {
                return this.NoAuth();
            }

            foreach (var id in request.Ids)
            {
                var info = await _examTmRepository.GetAsync(id);
                if (info == null) continue;
                await _examTmRepository.DeleteAsync(info.Id);

                await _authManager.AddAdminLogAsync("删除题目", StringUtils.StripTags(info.Title));
                await _authManager.AddStatLogAsync(StatType.ExamTmDelete, "删除题目", info.Id, StringUtils.StripTags(info.Title), info);
                await _authManager.AddStatCount(StatType.ExamTmDelete);
            }
            return new BoolResult
            {
                Value = true
            };
        }

    }
}
