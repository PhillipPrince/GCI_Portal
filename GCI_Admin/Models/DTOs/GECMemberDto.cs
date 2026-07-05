namespace GCI_Admin.Models.DTOs
{
    public class GECMemberDto
    {
        public int GECId { get; set; }
        public int MemberId { get; set; }
        public int GECPositionId { get; set; }
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
        public GECMemberDto GECMember { get; set; }
    }
}