namespace GCI_Admin.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime EventDate { get; set; }

        public string Location { get; set; }

        public bool IsPaid { get; set; }

        public decimal? Price { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public bool RequireRegistration { get; set; }

        public bool AllowWalkIns { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public int? GroupId { get; set; }
        public int? MinistryId { get; set; }
        public string? QrCode { get; set; }
        public string? AllowedAgeGroups { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public byte[]? EventImage { get; set; }
    }

    public class EventViewModel
    {
        public Event Event { get; set; }
        public List<EventRegistration> Registrations { get; set; }
        public List<EventFeedback> Feedbacks { get; set; } = new();
    }

}
