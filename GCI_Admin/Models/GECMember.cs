using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    public class GECMember
    {
        [Key]
        public int GECId { get; set; }
        public int MemberId { get; set; }
        public int GECPositionId { get; set; }
        
        [ForeignKey("GECPositionId")]
        public GECPosition? GECPosition { get; set; }
        public string Bio { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("MemberId")]
        public Member Member { get; set; }

       
       
    }
    public class GECMemberDetailsViewModel
    {
        public GECMember GECMember { get; set; }
    }
}
