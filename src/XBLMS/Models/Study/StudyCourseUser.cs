using Datory;
using Datory.Annotations;
using System;
using System.Collections.Generic;
using XBLMS.Enums;

namespace XBLMS.Models
{
    [DataTable("study_CourseUser")]
    public class StudyCourseUser : Entity
    {
        [DataColumn]
        public int PlanId { get; set; }
        [DataColumn]
        public int CourseId { get; set; }
        [DataColumn]
        public int UserId { get; set; }
        [DataColumn]
        public List<string> Mark { get; set; }
        [DataColumn]
        public int TotalEvaluation { get; set; }
        [DataColumn]
        public decimal AvgEvaluation { get; set; }
        [DataColumn]
        public bool Locked { get; set; }
        [DataColumn]
        public int TotalDuration { get; set; }
        [DataColumn]
        public decimal Credit { get; set; }
        [DataColumn]
        public DateTime? BeginStudyDateTime { get; set; }
        [DataColumn]
        public DateTime? LastStudyDateTime { get; set; }
        [DataColumn]
        public bool Collection { get; set; }
        [DataColumn]
        public StudyStatType State { get; set; }
    }
}
