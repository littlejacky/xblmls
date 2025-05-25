using Datory.Annotations;
using System.Collections.Generic;
using System;
using XBLMS.Enums;
using Datory;

namespace XBLMS.Models
{
    [DataTable("quiz_PlanRecord")]

    public class ExamPlanRecord : Entity
    {
        public ExamPlanRecord() { }
        public ExamPlanRecord(ExamPlan plan)
        {
            Title = plan.Title;
            UserGroupIds = plan.UserGroupIds;
            TmGroupIds = plan.TmGroupIds;
            IsTiming = plan.IsTiming;
            TimingMinute = plan.TimingMinute;
            TotalScore = plan.TotalScore;
            PassScore = plan.PassScore;
            RequirePass = plan.RequirePass;
            ConfigList = plan.ConfigList;
            TmScoreType = plan.TmScoreType;
            TxIds = plan.TxIds;
            TmGroupProportions = plan.TmGroupProportions;
            TmCount = plan.TmCount;
            OpenExist = plan.OpenExist;
            PlanId = plan.Id;
            CompanyId = plan.CompanyId;
            CreatorId = plan.CreatorId;
            DepartmentId = plan.DepartmentId;
        }

        [DataColumn]
        public int PlanId { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn(Text = true)]
        public List<int> UserGroupIds { get; set; }

        [DataColumn(Text = true)]
        public List<int> TmGroupIds { get; set; }

        /// <summary>
        /// 是否计时考试
        /// </summary>
        [DataColumn]
        public bool IsTiming { get; set; } = true;
        /// <summary>
        /// 计时分钟
        /// </summary>
        [DataColumn]
        public int TimingMinute { get; set; } = 60;
        [DataColumn]
        public int TotalScore { get; set; } = 100;
        [DataColumn]
        public int PassScore { get; set; } = 60;

        [DataColumn]
        public bool RequirePass { get; set; } // 未通过需重新做题

        //[DataColumn(Text = true)]
        public List<ExamPaperRandomConfig> ConfigList { get; set; }

        /// <summary>
        /// 题目分数计算类型
        /// </summary>
        [DataColumn]
        public ExamPaperTmScoreType TmScoreType { get; set; } = ExamPaperTmScoreType.ScoreTypeRate;

        [DataColumn(Text = true)]
        public List<int> TxIds { get; set; }

        public List<ExamTmGroupProportion> TmGroupProportions { get; set; }

        [DataColumn]
        public int TmCount { get; set; } = 100;

        /// <summary>
        /// 是否显示退出考试按钮
        /// </summary>
        [DataColumn]
        public bool OpenExist { get; set; } = true;

    }
}
