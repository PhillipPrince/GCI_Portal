using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("EventFeedback")]
    public class EventFeedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        [MaxLength(50)]
        public string AttendanceType { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? NonAttendanceReason { get; set; }

        public int? SpeakerRating { get; set; }
        public int? ProgramRating { get; set; }
        public int? FacilitiesRating { get; set; }
        public int? MessageRating { get; set; }
        public int? MealsRating { get; set; }
        public int? ScheduleRating { get; set; }

        public string? LikedMost { get; set; }
        public string? Improvements { get; set; }
        public string? Insights { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("EventId")]
        public virtual Event? Event { get; set; }

        [ForeignKey("MemberId")]
        public virtual Member? Member { get; set; }
    }
}
