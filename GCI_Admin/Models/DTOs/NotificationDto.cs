using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models.DTOs
{
    public class NotificationDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public int? CreatedById { get; set; }

        public bool IsChurchWide { get; set; }

        public int? MinistryId { get; set; }

        public DateTime NotificationTime { get; set; } = DateTime.Now;

        public DateTime? ExpiryTime { get; set; }

        public bool RequiresReminder { get; set; }

        public bool SendSMS { get; set; }

        public bool SendEmail { get; set; }

        public bool IsActive { get; set; } = true;

        public List<DropdownItem> NotificationGroups { get; set; }
        public int NotificationGroupId { get; set; }
        public List<DropdownItem> Members { get; set; }
        public List<int> SelectedMembers { get; set; }
    }
}   