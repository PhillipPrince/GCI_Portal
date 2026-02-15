namespace GCI_Admin.Models.DTOs
{
    public class GECMemberDto
    {
        public int MemberId { get; set; }
        public string PositionTitle { get; set; }
        public string Bio { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
