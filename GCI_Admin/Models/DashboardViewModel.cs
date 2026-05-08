namespace GCI_Admin.Models
{
    public class DashboardViewModel
    {
        // Existing properties
        public int TotalMembers { get; set; }
        public int TotalActiveMembers { get; set; }
        public int UpcomingEvents { get; set; }
        public List<Event> UpcomingEvent { get; set; }
        public MemberStatusModel MemberStatus { get; set; }

    }
}
