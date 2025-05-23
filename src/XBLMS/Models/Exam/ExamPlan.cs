using Datory.Annotations;
using System;
using System.Collections.Generic;
using XBLMS.Enums;

namespace XBLMS.Models
{
    [DataTable("quiz_Plan")]

    public class ExamPlan : ExamPaper
    {
        [DataColumn]
        public FrequencyType FrequencyType { get; set; } = FrequencyType.PerWeekday; // 频率类型：Daily, Weekly, Monthly, Yearly

        [DataColumn]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [DataColumn]
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);

        [DataColumn]
        public bool RequirePass { get; set; } // 未通过需重新做题

        //[DataColumn(Text = true)]
        public List<ExamPaperRandomConfig> ConfigList { get; set; }

        [DataColumn]
        public PlanType PlanType { get; set; } = PlanType.Practice;

        [DataColumn]
        public int ExecutedTotal { get; set; }

        public List<ExamTmGroupProportion> TmGroupProportions { get; set; }
    }
}
