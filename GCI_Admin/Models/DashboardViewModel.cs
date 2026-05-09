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

        public decimal MemberGrowthPercentage { get; set; }
        public decimal ActiveMemberGrowthPercentage { get; set; }
        public decimal EventChangePercentage { get; set; }
        public decimal TotalMembersPercentage { get; set; }
        public decimal ActiveMembersPercentage { get; set; } 
        public decimal EventCompletionPercentage { get; set; } 
    }
}
