using Datory.Annotations;
using System;

namespace XBLMS.Models
{
    [DataTable("quiz_PlanPractice")]

    public class ExamPlanPractice: ExamPractice
    {
        [DataColumn]
        public DateTime? BeginDateTime { get; set; }
        [DataColumn]
        public DateTime? EndDateTime { get; set; }
        /// <summary>
        /// 已用时 秒
        /// </summary>
        [DataColumn]
        public int ExamTimeSeconds { get; set; }
        [DataColumn]
        public int PlanRecordId { get; set; }

        [DataColumn]
        public decimal Score { get; set; }
    }
}
