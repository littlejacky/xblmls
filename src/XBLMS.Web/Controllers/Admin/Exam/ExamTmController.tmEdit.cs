using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Configuration;
using XBLMS.Core.Utils.Office;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamTmController
    {

        [HttpGet, Route(RouteEdit)]
        public async Task<ActionResult<GetEditResult>> Get([FromQuery] IdRequest request)
        {
            var auth = await _authManager.GetAuthorityAuth();

            var tm = new ExamTm();
            if (request.Id > 0)
            {
                tm = await _examManager.GetTmInfo(request.Id);
            }
            var txList = await _examTxRepository.GetListAsync();
            var tmTree = await _examManager.GetExamTmTreeCascadesAsync(auth);

            return new GetEditResult
            {
                Item = tm,
                TxList = txList,
                TmTree = tmTree,
            };
        }

        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteEditSubmit)]
        public async Task<ActionResult<BoolResult>> SubmitTm([FromBody] ItemRequest<ExamTm> request)
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
            var info = request.Item;
            if (info.Id > 0)
            {
                var txInfo = await _examTxRepository.GetAsync(info.TxId);
                if (txInfo.ExamTxBase == ExamTxBase.Duoxuanti)
                {
                    info.Answer = info.Answer.Replace(",", "").Trim();
                }
                await _examTmRepository.UpdateAsync(info);
                await _authManager.AddAdminLogAsync("修改题目", $"{info.Title}");
            }
            else
            {
                if (await _examTmRepository.ExistsAsync(info.Title, info.TxId, auth.CompanyId))
                {
                    return this.Error("已存在相同的题目");
                }
                var txInfo = await _examTxRepository.GetAsync(info.TxId);
                if (txInfo.ExamTxBase == ExamTxBase.Duoxuanti)
                {
                    info.Answer = info.Answer.Replace(",", "").Trim();
                }

                info.CompanyId = auth.CompanyId;
                info.DepartmentId = auth.DepartmentId;
                info.CreatorId = auth.AdminId;

                info.Id = await _examTmRepository.InsertAsync(info);

                await _statRepository.AddCountAsync(StatType.ExamTmAdd);
                await _authManager.AddAdminLogAsync("添加题目", $"{info.Title}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
