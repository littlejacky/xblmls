using Datory;
using Datory.Annotations;

namespace XBLMS.Models
{
    [DataTable("exam_TmGroupProportion")]
    public class ExamTmGroupProportion : Entity
    {
        [DataColumn]
        public int ExamPaperId { get; set; }
        [DataColumn]
        public int TmGroupId { get; set; }
        [DataColumn]
        public double GroupRatio { get; set; }
    }
}
