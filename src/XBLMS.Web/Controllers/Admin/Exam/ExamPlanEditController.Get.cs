using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamPlanEditController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] IdRequest request)
        {
            var auth = await _authManager.GetAuthorityAuth();

            var plan = new ExamPlan();
            plan.Title = "计划-" + StringUtils.PadZeroes(await _examPlanRepository.MaxAsync() + 1, 5);
            if (request.Id > 0)
            {
                plan = await _examPlanRepository.GetAsync(request.Id);
            }

            var tree = await _examManager.GetExamPaperTreeCascadesAsync(auth);
            var txs = await _examTxRepository.GetListAsync();
            if (txs == null || txs.Count == 0)
            {
                await _examTxRepository.ResetAsync();
                txs = await _examTxRepository.GetListAsync();
            }
            var cers = await _examCerRepository.GetListAsync(auth);
            var tmGroups = await _examTmGroupRepository.GetListWithoutLockedAsync(auth);
            var userGroups = await _userGroupRepository.GetListWithoutLockedAsync(auth);
            var configList = await _examPaperRandomConfigRepository.GetListAsync(request.Id);
            var fixedGroups = new List<ExamTmGroup>();
            if (tmGroups != null && tmGroups.Count > 0)
            {
                fixedGroups.AddRange(tmGroups.Where(group => group.GroupType == TmGroupType.Fixed).ToList());
            }

            if (tree == null || tree.Count == 0)
            {
                var treeId = await _examPaperTreeRepository.InsertAsync(new ExamPaperTree
                {
                    Name = "试卷分类",
                    CompanyId = auth.CompanyId,
                    DepartmentId = auth.DepartmentId,
                    CreatorId = auth.AdminId
                });
                plan.TreeId = treeId;
                tree = await _examManager.GetExamPaperTreeCascadesAsync(auth);
            }
            return new GetResult
            {
                Item = plan,
                PaperTree = tree,
                TxList = txs,
                CerList = cers,
                TmGroupList = tmGroups,
                TmFixedGroupList = fixedGroups,
                UserGroupList = userGroups,
                ConfigList = configList
            };

        }

    }
}
