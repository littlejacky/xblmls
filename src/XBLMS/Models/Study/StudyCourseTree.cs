using System;
using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace XBLMS.Models
{
    [DataTable("study_CourseTree")]
    public class StudyCourseTree : Entity
    {
        [DataColumn]
        public string Name { get; set; }
        [DataColumn]
        public int ParentId { get; set; }
        public List<StudyCourseTree> Children { get; set; }

    }
}
