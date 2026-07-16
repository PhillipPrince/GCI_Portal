using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("RcpsInvites")]
    public class RcpsInvite
    {
        [Key]
        public int Id { get; set; }

        public int RcpsPlanId { get; set; }

        public int MemberId { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomName { get; set; }

        [Required]
        [StringLength(100)]
        public string UniqueLinkCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRaised { get; set; }

        public int ContributorsCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("RcpsPlanId")]
        public RcpsPlan? RcpsPlan { get; set; }
        
        [ForeignKey("MemberId")]
        public Member? Member { get; set; }
    }
}
