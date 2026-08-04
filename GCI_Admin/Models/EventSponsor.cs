using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    public class EventSponsor
    {
        [Key]
        public int SponsorId { get; set; }

        public int EventId { get; set; }

        public string SponsorName { get; set; }

        public string SponsorPhone { get; set; }

        public int NumberOfPeople { get; set; }

        public decimal Amount { get; set; }

        public string? CheckoutRequestID { get; set; }

        public int PaymentStatusId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("EventId")]
        public Event? Event { get; set; }
    }
}
