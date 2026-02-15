namespace GCI_Admin.Models
{
    public class DashboardViewModel
    {
        public int TotalMembers{ get; set; }
        public int TotalActiveMembers { get; set; }
        public int UpcomingEvents { get; set; }
        public List<Member> ActiveMembers { get; set; } = new();
        public List<Member> MembershipClassMembers { get; set; } = new();
        public List<Member> NonMembers { get; set; } = new();
    }
}
