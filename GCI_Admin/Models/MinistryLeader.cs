using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    public class MinistryLeader
    {
        [Key]
        public int MinistryLeaderId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        [ForeignKey("Ministry")]
        public int MinistryId { get; set; }

        public string PositionTitle { get; set; }

        public string Bio { get; set; }


        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public  Ministry Ministry { get; set; }
        public Member Member { get; set; }
    }
}