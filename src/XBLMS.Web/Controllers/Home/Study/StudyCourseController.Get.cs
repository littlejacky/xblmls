using Datory;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Web.Controllers.Home.Study
{
    public partial class StudyCourseController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var resultList = new List<StudyCourse>();
            var resultTotal = 0;
            var resultMarkTotal = 0;
            var resultMarkList = new List<string>();

            if (request.Collection || !string.IsNullOrEmpty(request.State))
            {
                var (markTotal, markList) = await _studyCourseUserRepository.GetMarkListAsync(user.Id);
                resultMarkTotal = markTotal;
                resultMarkList = markList;

                var (total, list) = await _studyCourseUserRepository.GetListAsync(user.Id, request.Collection, request.KeyWords, request.Mark, request.Orderby, request.State, request.PageIndex, request.PageSize);
                resultTotal = total;
                if (total > 0)
                {
                    foreach (var item in list)
                    {
                        var course = await _studyCourseRepository.GetAsync(item.CourseId);
                        GetCouresInfo(course, item, item.PlanId);
                        resultList.Add(course);
                    }
                }
            }
            else
            {
                var (markTotal, markList) = await _studyCourseRepository.User_GetPublicMarkListAsync();
                resultMarkTotal = markTotal;
                resultMarkList = markList;

                var (total, list) = await _studyCourseRepository.User_GetPublicListAsync(request.KeyWords, request.Mark, request.Orderby, request.PageIndex, request.PageSize);
                resultTotal = total;
                if (total > 0)
                {
                    foreach (var item in list)
                    {
                        var courseUser = await _studyCourseUserRepository.GetAsync(user.Id, 0, item.Id);
                        GetCouresInfo(item, courseUser, 0);

                        resultList.Add(item);
                    }
                }
            }


            return new GetResult
            {
                MarkTotal = resultMarkTotal,
                MarkList = resultMarkList,
                Total = resultTotal,
                List = resultList
            };
        }

        [HttpGet, Route(RouteItem)]
        public async Task<ActionResult<ItemResult<StudyCourse>>> GetItem([FromQuery] GetItemRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var course = await _studyCourseRepository.GetAsync(request.Id);
            var courseUser = await _studyCourseUserRepository.GetAsync(user.Id, request.PlanId, request.Id);

            GetCouresInfo(course, courseUser, request.PlanId);

            return new ItemResult<StudyCourse>
            {
                Item = course
            };
        }

        private async void GetCouresInfo(StudyCourse course, StudyCourseUser courseUser, int planId)
        {
            if (courseUser != null)
            {
                course.Set("State", courseUser.State);
                course.Set("StateStr", courseUser.State.GetDisplayName());
                if (planId > 0)
                {
                    var planCourse = await _studyPlanCourseRepository.GetAsync(planId, course.Id);
                    if (planCourse.IsSelectCourse)
                    {
                        course.Set("CourseType", "选修课");
                    }
                    else
                    {
                        course.Set("CourseType", "必修课");
                    }
                }
                else
                {
                    course.Set("CourseType", "公共课");
                }
            }
            else
            {
                course.Set("State", "");
                course.Set("StateStr", "未学");
                course.Set("CourseType", "公共课");
            }
            course.Set("PlanId", planId);
        }
    }
}
