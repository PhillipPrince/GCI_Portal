using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    public class GrowthCenterLeader
    {
        [Key]
        public int GrowthCenterLeaderId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public int GrowthCenterId { get; set; }

        public string Bio { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MemberId")]
        public Member Member { get; set; }

        [ForeignKey("GrowthCenterId")]
        public GrowthCenter GrowthCenter { get; set; }
    }
}