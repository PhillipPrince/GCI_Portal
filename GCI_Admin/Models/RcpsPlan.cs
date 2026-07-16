using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("RcpsPlans")]
    public class RcpsPlan
    {
        [Key]
        public int Id { get; set; }

        public int RcpsId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TargetAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountRaised { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("RcpsId")]
        public Rcps? Rcps { get; set; }

        public virtual ICollection<RcpsInvite> Invites { get; set; } = new List<RcpsInvite>();
    }
}
