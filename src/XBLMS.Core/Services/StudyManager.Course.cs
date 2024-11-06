using Datory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;

namespace XBLMS.Core.Services
{
    public partial class StudyManager
    {
        public async Task User_GetCourseInfo(int userId, int planId, int courseId)
        {
            var courseInfo = await _studyCourseRepository.GetAsync(courseId);
            var userCourse = await _studyCourseUserRepository.GetAsync(userId, planId, courseId);
            if(userCourse != null)
            {

            }
            else
            {
                await _studyCourseUserRepository.InsertAsync(new StudyCourseUser { UserId = userId, PlanId = planId, CourseId = courseId });
            }
        }
        public async Task User_GetCourseInfo(int userId, StudyCourse course)
        {
            var userCourse = await _studyCourseUserRepository.GetAsync(userId);
        }
    }
}
