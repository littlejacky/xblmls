using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseEditController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAuthorityAuth();

            var course = new StudyCourse();
            course.Name = "课程-" + StringUtils.PadZeroes(await _studyCourseRepository.MaxAsync(), 5);

            course.OfflineBeginDateTime = DateTime.Now;
            course.OfflineEndDateTime = DateTime.Now.AddDays(1);
            course.OfflineAddress = "无";
            course.OfflineTeacher = "无";

            var courseWareList = new List<StudyCourseWare>();

            if (request.Id > 0)
            {
                course = await _studyCourseRepository.GetAsync(request.Id);

                var wareList = await _studyCourseWareRepository.GetListAsync(course.Id);
                if (wareList != null && wareList.Count > 0)
                {
                    foreach (var item in wareList)
                    {
                        courseWareList.Add(item);
                    }
                }
            }
            if (request.Face)
            {
                course.OffLine = true;
                if (string.IsNullOrEmpty(course.CoverImg))
                {
                    course.CoverImg = _pathManager.DefaultCourseFaceCoverUrl;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(course.CoverImg))
                {
                    course.CoverImg = _pathManager.DefaultCourseCoverUrl;
                }
            }

            var tree = await _studyManager.GetStudyCourseTreeCascadesAsync(auth);
            var markList = await _studyCourseRepository.GetMarkListAsync();

            if (tree == null || tree.Count == 0)
            {
                var treeId = await _studyCourseTreeRepository.InsertAsync(new StudyCourseTree
                {
                    Name = "课程分类",
                    CompanyId = auth.CompanyId,
                    DepartmentId = auth.DepartmentId,
                    CreatorId = auth.AdminId
                });
                course.TreeId = treeId;
                tree = await _studyManager.GetStudyCourseTreeCascadesAsync(auth);
            }
            return new GetResult
            {
                Item = course,
                Tree = tree,
                WareList = courseWareList,
                MarkList = markList
            };

        }

    }
}
