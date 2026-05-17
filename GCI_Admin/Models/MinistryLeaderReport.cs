using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    public class MinistryLeaderReport
    {
        public int MinistryLeaderReportId { get; set; }

        public int MinistryId { get; set; }

        public int SubmittedByMinistryLeaderId { get; set; }
        [NotMapped]
        public string SubmittedByMinistryLeaderName { get; set; } = string.Empty;

        public DateTime ReportingMonth { get; set; }

        public bool HadCalendarActivity { get; set; }

        public string? CalendarActivity { get; set; }

        public string SupportedPillar { get; set; }

        public string? PillarSupportDescription { get; set; }

        public int? CalendarActivityAttendance { get; set; }

        public bool HadOtherMeetings { get; set; }

        public string OtherMeetingDescription { get; set; }

        public int? OtherMeetingAttendance { get; set; }

        public int TotalRegisteredMembers { get; set; }

        public string LeadershipSupportComments { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual Ministry Ministry { get; set; }

        public virtual MinistryLeader SubmittedByMinistryLeader { get; set; }
    }
}