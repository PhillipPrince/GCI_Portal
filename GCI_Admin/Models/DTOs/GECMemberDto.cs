using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models.DTOs
{
    public class GECMemberDto
    {
        public int GECId { get; set; }
        [Required(ErrorMessage = "Member is required.")]
        public int MemberId { get; set; }
        [Required(ErrorMessage = "Position is required.")]
        public int GECPositionId { get; set; }
        [Required(ErrorMessage = "Title prefix is required.")]
        public int? TitlePrefixId { get; set; }
        public string? Bio { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? ProfileImageBase64 { get; set; } 

    }

    public class CreateGECMemberDto
    {
        public List<Member> Members { get; set; }
        public List<GECPosition> Positions { get; set; }
        public List<TitlePrefix> TitlePrefixes { get; set; } = new List<TitlePrefix>();
        public GECMemberDto GECMember { get; set; }
    }
}