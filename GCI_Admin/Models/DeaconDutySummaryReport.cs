using System;
using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models
{
    public class DeaconDutySummaryReport
    {
        [Key]
        public int DeaconDutySummaryReportId { get; set; }

        public int DeaconId { get; set; }

        public DateTime ReportDate { get; set; }

        public string TuesdayPrayersObservation { get; set; }

        public string ThursdayBibleStudyObservation { get; set; }

        public string FridayKeshaObservation { get; set; }

        public string SundayServicesObservation { get; set; }

        public string OtherWeeklyEventsObservation { get; set; }

        public string KeyIssuesForAttention { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

    }
}