﻿using Datory;
using Datory.Annotations;
using System;
using System.Collections.Generic;
using XBLMS.Enums;

namespace XBLMS.Models
{
    [DataTable("quiz_Plan")]

    public class ExamPlan : Entity
    {
        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public FrequencyType FrequencyType { get; set; } = FrequencyType.Weekly; // 频率类型：Daily, Weekly, Monthly, Immediately

        [DataColumn]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [DataColumn]
        public DateTime EndDate { get; set; } = DateTime.Now.AddYears(1);

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

        [DataColumn]
        public int ExecutedTotal { get; set; }

        public List<ExamTmGroupProportion> TmGroupProportions { get; set; }

        [DataColumn]
        public int TmCount { get; set; } = 100;

        /// <summary>
        /// 是否显示退出考试按钮
        /// </summary>
        [DataColumn]
        public bool OpenExist { get; set; } = true;

        [DataColumn]
        public bool Locked { get; set; } = false;
    }
}
