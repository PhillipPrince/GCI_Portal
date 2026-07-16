using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("RcpsFriendContributions")]
    public class RcpsFriendContribution
    {
        [Key]
        public int Id { get; set; }

        public int RcpsInviteId { get; set; }

        [Required]
        [StringLength(100)]
        public string FriendName { get; set; }

        [Required]
        [StringLength(20)]
        public string FriendPhone { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public bool IsAnonymous { get; set; }

        [StringLength(100)]
        public string? CheckoutRequestID { get; set; }

        public int PaymentStatusId { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [ForeignKey("RcpsInviteId")]
        public RcpsInvite? RcpsInvite { get; set; }
    }
}
