using Datory.Annotations;

namespace XBLMS.Models
{
    [DataTable("quiz_PlanPractice")]

    public class ExamPlanPractice: ExamPractice
    {
        [DataColumn]
        public int PlanRecordId { get; set; }
    }
}
