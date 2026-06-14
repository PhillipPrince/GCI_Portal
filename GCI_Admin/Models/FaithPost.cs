using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("FaithPosts")]
    public class FaithPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? PostType { get; set; }

        public bool IsApproved { get; set; } = false;

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("MemberId")]
        public virtual Member? Member { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual Member? Approver { get; set; }

        public virtual ICollection<FaithPostComment> Comments { get; set; } = new List<FaithPostComment>();
    }
}
