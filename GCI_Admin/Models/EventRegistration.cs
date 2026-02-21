using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    public class EventRegistration
    {
        [Key]
        public int RegistrationId { get; set; }

        public int EventId { get; set; }

        public int MemberId { get; set; }

        public DateTime RegistrationDate { get; set; }

        public int PaymentStatusId { get; set; }

        public decimal AmountPaid { get; set; }

        public bool? HasAttended { get; set; }

        [ForeignKey("EventId")]
       public Event Event { get; set; }
        [ForeignKey("MemberId")]
        public Member Member { get; set; }



    }
}
