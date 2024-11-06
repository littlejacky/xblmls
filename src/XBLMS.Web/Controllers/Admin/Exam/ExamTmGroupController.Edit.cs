using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Core.Utils;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamTmGroupController
    {
        [HttpGet, Route(RouteEditGet)]
        public async Task<ActionResult<GetEditResult>> GetEdit([FromQuery] IdRequest request)
        {
            var auth = await _authManager.GetAuthorityAuth();
          
            var group = new ExamTmGroup();
            var selectOrganIds = new List<string>();
            if (request.Id > 0)
            {
                group = await _examTmGroupRepository.GetAsync(request.Id);
            }
            var tmTree = await _examManager.GetExamTmTreeCascadesAsync(auth);
            var groupTypeSelects = ListUtils.GetSelects<TmGroupType>();
            var txList = await _examTxRepository.GetListAsync();

            if (txList == null || txList.Count == 0)
            {
                await _examTxRepository.ResetAsync();
                txList = await _examTxRepository.GetListAsync();
            }

            return new GetEditResult
            {
                Group = group,
                GroupTypeSelects = groupTypeSelects,
                TmTree = tmTree,
                TxList = txList
            };
        }


        [HttpPost, Route(RouteEditPost)]
        public async Task<ActionResult<BoolResult>> PostEdit([FromBody] GetEditRequest request)
        {

            if (request.Group.Id > 0)
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


            if (request.Group.Id > 0)
            {
                var group = await _examTmGroupRepository.GetAsync(request.Group.Id);

                await _examTmGroupRepository.UpdateAsync(request.Group);
                await _authManager.AddAdminLogAsync("修改题目组", $"题目组名称：{group.GroupName}");
            }
            else
            {
                request.Group.CreatorId = auth.AdminId;
                request.Group.CompanyId = auth.CompanyId;
                request.Group.DepartmentId = auth.DepartmentId;

                await _examTmGroupRepository.InsertAsync(request.Group);
                await _authManager.AddAdminLogAsync("新增题目组", $"题目组名称：{request.Group.GroupName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
