using Datory;
using Datory.Annotations;
using System.Collections.Generic;

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
