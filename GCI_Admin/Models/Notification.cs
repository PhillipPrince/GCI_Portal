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
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public int CreatedById { get; set; }



        public bool IsChurchWide { get; set; } = false;

        public int? MinistryId { get; set; }


        public int? RcpsId { get; set; }
        public int? GrowthCenterId { get; set; }

        public DateTime? NotificationTime { get; set; }

        public DateTime? ExpiryTime { get; set; }

        public bool RequiresReminder { get; set; } = false;

        public bool SendSMS { get; set; } = false;
        public bool SendPushNotification { get; set; } = false;

        public bool SendEmail { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
        public int? NotificationGroupId { get; set; }
        public bool IsSent { get; set; } = false;
        public int? SmsSentCount { get; set; } = 0;
        public int? PushSentCount { get; set; } = 0;
        public string PushNotificationType { get; set; } = "general";
        public string DeepLinkScreen { get; set; } = "notifications";
        public string? DeepLinkId { get; set; }

        [NotMapped]
        public bool IsCareRequestNotification { get; set; } = false;
        [NotMapped]
        public int CareRequestId { get; set; }


        [NotMapped]
        public byte[]? ImageBase64 { get; set; }
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
    
