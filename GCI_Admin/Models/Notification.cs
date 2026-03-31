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
        public int? NotificationGroupId { get; set; }
    }
   
        public class NotificationGroup
        {
        [Key]
        public int GroupId { get; set; }

            public string GroupName { get; set; }

            public string Description { get; set; }

            public bool IsActive { get; set; } = true;

            public DateTime CreatedAt { get; set; } = DateTime.Now;

            public DateTime? UpdatedAt { get; set; }
        }

    public class SpecialNotificationMember
    {
        [Key]
        public int Id { get; set; }

        public int NotificationId { get; set; }

        public int MemberId { get; set; }

        public bool IsNotified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Optional Navigation Properties
        //public Notification Notification { get; set; }

        // public Member Member { get; set; }
    }
}
    
