using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models.DTOs
{
    public class EventDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public bool IsPaid { get; set; }
        public bool IsActive { get; set; }

        [Range(0, 1000000)]
        public decimal? Price { get; set; }
        public bool RequireRegistration { get; set; }
        public bool AllowWalkIns { get;set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public int? GroupId { get; set; }
        public int? MinistryId { get; set; }
        public List<int>? SpecificMemberIds { get; set; }
        public string? ImageBase64 { get; set; }
        public string? AllowedAgeGroups { get; set; }
    }
}
