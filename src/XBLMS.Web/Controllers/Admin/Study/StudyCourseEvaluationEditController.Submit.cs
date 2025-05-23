﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseEvaluationEditController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] GetSubmitRequest request)
        {
            if (request.Item.Id > 0)
            {
                if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Update))
                {
                    return this.NoAuth();
                }
            }
            else
            {
                if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Add))
                {
                    return this.NoAuth();
                }
            }


            var auth = await _authManager.GetAuthorityAuth();
            var item = request.Item;


            if (item.Id > 0)
            {
                await _studyCourseEvaluationRepository.UpdateAsync(item);

                if (request.ItemList != null && request.ItemList.Count > 0)
                {
                    var itemList = await _studyCourseEvaluationItemRepository.GetListAsync(item.Id);
                    var itemIds = new List<int>();
                    if (itemList != null && itemList.Count > 0)
                    {
                        foreach (var itemInfo in itemList)
                        {
                            itemIds.Add(itemInfo.Id);
                        }
                    }

                    foreach (var itemInfo in request.ItemList)
                    {
                        if (itemInfo.Id > 0)
                        {
                            if (itemIds.Contains(itemInfo.Id))
                            {
                                itemIds.Remove(itemInfo.Id);
                                await _studyCourseEvaluationItemRepository.UpdateAsync(itemInfo);
                            }
                        }
                        else
                        {
                            itemInfo.EvaluationId = item.Id;
                            itemInfo.CompanyId = auth.CompanyId;
                            itemInfo.DepartmentId = auth.DepartmentId;
                            itemInfo.CreatorId = auth.AdminId;
                            await _studyCourseEvaluationItemRepository.InsertAsync(itemInfo);
                        }
                    }
                    if (itemIds.Count > 0)
                    {
                        foreach (var itemId in itemIds)
                        {
                            await _studyCourseEvaluationItemRepository.DeleteAsync(itemId);
                        }
                    }
                }
                else
                {
                    var itemList = await _studyCourseEvaluationItemRepository.GetListAsync(item.Id);
                    if (itemList != null && itemList.Count > 0)
                    {
                        foreach (var itemInfo in itemList)
                        {
                            await _studyCourseEvaluationItemRepository.DeleteAsync(itemInfo.Id);
                        }
                    }
                }

                await _authManager.AddAdminLogAsync("修改课程评价", item.Title);
                await _authManager.AddStatLogAsync(StatType.StudyEvaluationUpdate, "修改课程评价", item.Id, item.Title);
            }
            else
            {
                item.CompanyId = auth.CompanyId;
                item.DepartmentId = auth.DepartmentId;
                item.CreatorId = auth.AdminId;

                var itemId = await _studyCourseEvaluationRepository.InsertAsync(item);
                if (request.ItemList != null && request.ItemList.Count > 0)
                {
                    foreach (var itemInfo in request.ItemList)
                    {
                        itemInfo.EvaluationId = itemId;
                        itemInfo.CompanyId = auth.CompanyId;
                        itemInfo.DepartmentId = auth.DepartmentId;
                        itemInfo.CreatorId = auth.AdminId;
                        await _studyCourseEvaluationItemRepository.InsertAsync(itemInfo);
                    }
                }

                await _authManager.AddAdminLogAsync("新增课程评价", item.Title);
                await _authManager.AddStatLogAsync(StatType.StudyEvaluationAdd, "新增课程评价", itemId, item.Title);
                await _authManager.AddStatCount(StatType.StudyEvaluationAdd);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }


}
