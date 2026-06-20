namespace GCI_Admin.Models
{
    public class DashboardViewModel
    {
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

        public int TotalMeetings { get; set; }
        public int TotalAttendees { get; set; }
        public decimal AverageAttendance { get; set; }
        public int TotalMale { get; set; }
        public int TotalFemale { get; set; }
        public int TotalChildren { get; set; }
        public int MeetingsLast30Days { get; set; }
        public int AttendeesLast30Days { get; set; }
        public decimal AttendanceGrowthPercentage { get; set; }
        public int MeetingTypesCount { get; set; }

        public List<RecentMeetingStats> RecentMeetings { get; set; }
        public List<MonthlyTrendStats> MonthlyAttendanceTrend { get; set; }
    }

    public class RecentMeetingStats
    {
        public int MeetingId { get; set; }
        public string MeetingType { get; set; }
        public DateTime MeetingDate { get; set; }
        public int TotalAttendees { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int ChildrenCount { get; set; }
    }

    public class MonthlyTrendStats
    {
        public string MonthName { get; set; }
        public int Year { get; set; }
        public int TotalMeetings { get; set; }
        public int TotalAttendees { get; set; }
        public double AverageAttendance { get; set; }
    }
}