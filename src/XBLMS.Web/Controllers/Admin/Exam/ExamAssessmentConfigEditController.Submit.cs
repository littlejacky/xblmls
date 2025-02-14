using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamAssessmentConfigEditController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] GetSubmitRequest request)
        {
            var auth = await _authManager.GetAuthorityAuth();

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

            var item = request.Item;

            if (item.Id > 0)
            {
                await _examAssessmentConfigRepository.UpdateAsync(item);

                if (request.ItemList != null && request.ItemList.Count > 0)
                {
                    var itemList = await _examAssessmentConfigSetRepository.GetListAsync(item.Id);
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
                                await _examAssessmentConfigSetRepository.UpdateAsync(itemInfo);
                            }
                        }
                        else
                        {
                            itemInfo.ConfigId = item.Id;
                            itemInfo.CompanyId = auth.CompanyId;
                            itemInfo.DepartmentId = auth.DepartmentId;
                            itemInfo.CreatorId = auth.AdminId;
                            await _examAssessmentConfigSetRepository.InsertAsync(itemInfo);
                        }
                    }
                    if (itemIds.Count > 0)
                    {
                        foreach(var itemId in itemIds)
                        {
                            await _examAssessmentConfigSetRepository.DeleteAsync(itemId);
                        }
                    }
                }
                else
                {
                    var itemList = await _examAssessmentConfigSetRepository.GetListAsync(item.Id);
                    if (itemList != null && itemList.Count > 0)
                    {
                        foreach (var itemInfo in itemList)
                        {
                            await _examAssessmentConfigSetRepository.DeleteAsync(itemInfo.Id);
                        }
                    }
                }

                await _authManager.AddAdminLogAsync("修改测评类别", $"{item.Title}");
            }
            else
            {
                item.CompanyId = auth.CompanyId;
                item.DepartmentId = auth.DepartmentId;
                item.CreatorId = auth.AdminId;

                var itemId = await _examAssessmentConfigRepository.InsertAsync(item);
                if (request.ItemList != null && request.ItemList.Count > 0)
                {
                    foreach (var itemInfo in request.ItemList)
                    {
                        itemInfo.ConfigId = itemId;
                        itemInfo.CompanyId = auth.CompanyId;
                        itemInfo.DepartmentId = auth.DepartmentId;
                        itemInfo.CreatorId = auth.AdminId;
                        await _examAssessmentConfigSetRepository.InsertAsync(itemInfo);
                    }
                }

                await _authManager.AddAdminLogAsync("新增测评类别", $"{item.Title}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }


}
