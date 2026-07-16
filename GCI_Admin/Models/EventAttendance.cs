using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("EventAttendances")]
    public class EventAttendance
    {
        [Key]
        public int Id { get; set; }

        public int EventId { get; set; }

        public int MemberId { get; set; }

        public int DayNumber { get; set; }

        public DateTime AttendanceDate { get; set; }
    }
}
