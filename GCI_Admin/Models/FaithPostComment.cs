using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("FaithPostComments")]
    public class FaithPostComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("PostId")]
        public virtual FaithPost? Post { get; set; }

        [ForeignKey("MemberId")]
        public virtual Member? Member { get; set; }
    }
}
