using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;

namespace XBLMS.Web.Controllers.Home.Study
{
    public partial class StudyCourseInfoController
    {
        [HttpPost, Route(RouteWareSetProgress)]
        public async Task<ActionResult<GetWareSetProgressResult>> SetWareProgress([FromBody] GetWareSetProgressRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var userware = await _studyCourseWareUserRepository.GetAsync(request.Id);
            if (!userware.StudyCurrent)
            {
                await _studyCourseWareUserRepository.ClearCureentAsync(user.Id, userware.PlanId, userware.CourseId);
                userware.StudyCurrent = true;
            }

            if (userware.State == StudyStatType.Yiwancheng)
            {
                userware.TotalDuration += request.Progress;
            }
            else
            {
                userware.State = StudyStatType.Xuexizhong;
                if (request.IsAdd)
                {
                    userware.TotalDuration += request.Progress;
                }
                else
                {
                    userware.TotalDuration = request.Progress;
                }
            }
            userware.CurrentDuration = request.CurrentDuration;

            await _studyCourseWareUserRepository.UpdateAsync(userware);

            var usercourse = await _studyCourseUserRepository.GetAsync(user.Id, userware.PlanId, userware.CourseId);
            var userWareList = await _studyCourseWareUserRepository.GetListAsync(user.Id, userware.PlanId, userware.CourseId);
            var totalDuraiont = userWareList.Sum(ware => ware.TotalDuration);

            usercourse.TotalDuration = totalDuraiont;
            usercourse.LastStudyDateTime = DateTime.Now;

            await _studyCourseUserRepository.UpdateAsync(usercourse);

            return new GetWareSetProgressResult
            {
                TotalDuration = totalDuraiont
            };
        }


        [HttpPost, Route(RouteWareSetOver)]
        public async Task<ActionResult<GetWareSetProgressResult>> SetWareOver([FromBody] IdRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var allOver = true;

            var userware = await _studyCourseWareUserRepository.GetAsync(request.Id);
            var usercourse = await _studyCourseUserRepository.GetAsync(user.Id, userware.PlanId, userware.CourseId);

            if (userware.State != StudyStatType.Yiwancheng)
            {
                var ware = await _studyCourseWareRepository.GetAsync(userware.CourseWareId);

                userware.State = StudyStatType.Yiwancheng;
                userware.TotalDuration = ware.Duration;
                await _studyCourseWareUserRepository.UpdateAsync(userware);

                var userWareList = await _studyCourseWareUserRepository.GetListAsync(user.Id, userware.PlanId, userware.CourseId);
                long totalDuraiont = 0;
            

                foreach (var item in userWareList)
                {
                    if (item.State != StudyStatType.Yiwancheng && item.Id != userware.Id) 
                    {
                        allOver = false;
                    }
                    totalDuraiont += item.TotalDuration;
                }

                usercourse.TotalDuration = totalDuraiont;
                await _studyCourseUserRepository.UpdateAsync(usercourse);
            }
            return new GetWareSetProgressResult
            {
                StudyOver = allOver,
                TotalDuration = usercourse.TotalDuration
            };

        }
    }
}
