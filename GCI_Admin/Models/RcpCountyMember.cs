using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("RcpCountyMembers")]
    public class RcpCountyMember
    {
        [Key]
        public int Id { get; set; }

        public int RcpsId { get; set; }

        public int MemberId { get; set; }

        public bool IsLeader { get; set; }

        public string? Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }

        [ForeignKey("RcpsId")]
        public virtual Rcps Rcps { get; set; }
    }
}
