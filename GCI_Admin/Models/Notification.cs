using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        [StringLength(250)]
        public string Title { get; set; }


        [Required]
        public string Message { get; set; }

        public int CreatedById { get; set; }

        public bool IsChurchWide { get; set; }

        public int? MinistryId { get; set; }

        public DateTime NotificationTime { get; set; }

        public DateTime? ExpiryTime { get; set; }

        public bool RequiresReminder { get; set; }

        public bool SendSMS { get; set; }

        public bool SendEmail { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}   