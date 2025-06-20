﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace XBLMS.Web.Controllers.Home.Exam
{
    public partial class ExamPlanPracticeLogController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var (total, list) = await _examPlanPracticeRepository.GetListAsync(user.Id, request.KeyWords, request.DateFrom,request.DateTo, request.PageIndex, request.PageSize);
            if (total > 0)
            {
                foreach (var item in list)
                {
                    item.TmIds = null;
                    item.Zsds = null;
                }
            }
            return new GetResult
            {
                Total = total,
                List = list
            };
        }
    }
}
