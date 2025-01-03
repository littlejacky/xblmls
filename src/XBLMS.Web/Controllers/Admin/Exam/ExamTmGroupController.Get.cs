using Datory;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamTmGroupController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAuthorityAuth();

            var resultGroups = new List<ExamTmGroup>();
            var allGroups = await _examTmGroupRepository.GetListAsync(auth);

            foreach (var group in allGroups)
            {
                var creator = await _administratorRepository.GetByUserIdAsync(group.CreatorId);
                if (creator != null)
                {
                    group.Set("CreatorId", creator.Id);
                    group.Set("CreatorDisplayName", creator.DisplayName);
                }
                group.Set("TypeName", group.GroupType.GetDisplayName());

                group.TmTotal = await _examTmRepository.Group_CountAsync(auth, group);

                group.Set("UseCount", await _examPaperRepository.GetTmGroupCount(group.Id));

                var keyWord = request.Search;
                if (!string.IsNullOrEmpty(keyWord))
                {
                    if (StringUtils.Contains(group.GroupName, keyWord) || StringUtils.Contains(group.Description, keyWord))
                    {
                        resultGroups.Add(group);
                    }
                }
                else
                {
                    resultGroups.Add(group);
                }


            }
            return new GetResult
            {
                Groups = resultGroups
            };
        }
    }
}
