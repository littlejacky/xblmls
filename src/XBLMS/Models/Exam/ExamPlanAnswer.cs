using Datory.Annotations;
using XBLMS.Enums;

namespace XBLMS.Models
{
    [DataTable("quiz_PlanAnswer")]
    public class ExamPlanAnswer : ExamPracticeAnswer
    {
        [DataColumn]
        public decimal Score { get; set; }
        [DataColumn]
        public ExamTmType ExamTmType { get; set; }
    }
}
