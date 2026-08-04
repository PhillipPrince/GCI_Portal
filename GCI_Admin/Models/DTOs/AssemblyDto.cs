namespace GCI_Admin.Models.DTOs
{
    public class AssemblyDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public int? LeaderMemberId { get; set; }
        public string? ProfileImageBase64 { get; set; }
    }

    public class AssemblyLeaderDto
    {
        public int AssemblyLeaderId { get; set; }
        public int MemberId { get; set; }
        public int AssemblyId { get; set; }
        public int? TitlePrefixId { get; set; }
        public string? Bio { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? ProfileImageBase64 { get; set; }
    }

    public class CreateAssemblyLeaderDto
    {
        public List<Member> Members { get; set; }
        public List<Assembly> Assemblies { get; set; }
        public List<TitlePrefix> TitlePrefixes { get; set; } = new List<TitlePrefix>();
        public AssemblyLeaderDto AssemblyLeader { get; set; }
    }
}
