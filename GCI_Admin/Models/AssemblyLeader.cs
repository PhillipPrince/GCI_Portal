using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("AssemblyLeaders")]
    public class AssemblyLeader
    {
        [Key]
        public int AssemblyLeaderId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public int AssemblyId { get; set; }

        public int? TitlePrefixId { get; set; }
        [ForeignKey("TitlePrefixId")]
        public TitlePrefix? TitlePrefix { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("AssemblyId")]
        public Assembly Assembly { get; set; }

        [ForeignKey("MemberId")]
        public Member Member { get; set; }
    }
}