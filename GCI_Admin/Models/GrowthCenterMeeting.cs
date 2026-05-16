using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models
{
    public class GrowthCenterMeeting
    {
        [Key]
        public int GrowthCenterMeetingId { get; set; }

        public int GrowthCenterId { get; set; }

        public DateTime MeetingDate { get; set; }

        public string BibleStudyTopic { get; set; }

        public TimeSpan StartingTime { get; set; }

        public TimeSpan FinishingTime { get; set; }

        public int TotalMembers { get; set; }

        public int TotalVisitors { get; set; }

        public int NumberOfChildren { get; set; }

        public decimal OfferingCollected { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual GrowthCenter GrowthCenter { get; set; }

        public virtual ICollection<GrowthCenterMeetingAttendee> Attendees { get; set; }

        public virtual ICollection<GrowthCenterMeetingVisitor> Visitors { get; set; }
    }

    public class GrowthCenterMeetingAttendee
    {
        public int GrowthCenterMeetingAttendeeId { get; set; }

        public int GrowthCenterMeetingId { get; set; }

        public string MemberName { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Property
        public virtual GrowthCenterMeeting GrowthCenterMeeting { get; set; }
    }
    public class GrowthCenterMeetingVisitor
    {
        public int GrowthCenterMeetingVisitorId { get; set; }

        public int GrowthCenterMeetingId { get; set; }

        public string VisitorName { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Property
        public virtual GrowthCenterMeeting GrowthCenterMeeting { get; set; }
    }

    public class ReportsDashboardViewModel
    {
        public object DashboardSummary { get; set; }
        public List<object> CurrentMonthReport { get; set; }
        public List<object> PreviousMonthReport { get; set; }
        public ChurchReportViewModel ChurchReports { get; set; }
        public MinistryReportViewModel MinistryReports { get; set; }
        public GrowthCenterReportViewModel GrowthCenterReports { get; set; }
        public EventsReportViewModel EventsReports { get; set; }

        // Statistics properties
        public int TotalGrowthCenters { get; set; }
        public int TotalMeetings { get; set; }
        public int TotalAttendees { get; set; }
        public decimal TotalOfferings { get; set; }
        public decimal AverageAttendance { get; set; }
        public int GrowthPercentage { get; set; }
    }

    public class ChurchReportViewModel
    {
        // Statistics
        public int TotalMembers { get; set; }
        public int NewMembersThisMonth { get; set; }
        public int TotalMinistries { get; set; }
        public int TotalEvents { get; set; }
        public int MonthlyEvents { get; set; }
        public decimal TotalOfferings { get; set; }
        public decimal OfferingGrowth { get; set; }

        // Highest Attendance
        public HighestAttendanceModel HighestAttendance { get; set; }

        // Demographics
        public GenderDistributionModel GenderDistribution { get; set; }
        public AgeDemographicsModel AgeDemographics { get; set; }
        public MaritalStatusModel MaritalStatus { get; set; }
        public EducationLevelModel EducationLevel { get; set; }
        public EmploymentStatusModel EmploymentStatus { get; set; }

        // Lists
        public List<TopMinistryModel> TopMinistries { get; set; }
        public List<TopEventModel> TopEvents { get; set; }
        public List<GrowthTrendModel> GrowthTrend { get; set; }
    }

    public class MinistryReportViewModel
    {
        // Statistics
        public int ActiveMinistries { get; set; }
        public int TotalMinistryMembers { get; set; }
        public double AverageMembersPerMinistry { get; set; }
        public double MalePercentage { get; set; }
        public double FemalePercentage { get; set; }

        // Highest Attendance
        public HighestAttendanceMinistryModel HighestAttendance { get; set; }

        // Distributions
        public GenderDistributionModel GenderDistribution { get; set; }
        public List<MinistryGenderDataModel> MinistryGenderData { get; set; }

        // Lists
        public List<MinistryDetailModel> Ministries { get; set; }
        public List<MinistryPerformanceModel> Performance { get; set; }
    }
    public class GrowthCenterReportViewModel
    {
        // Statistics
        public int TotalCenters { get; set; }
        public int TotalMeetings { get; set; }
        public int TotalAttendance { get; set; }
        public decimal TotalOfferings { get; set; }

        // Highest Attendance
        public HighestAttendanceCenterModel HighestAttendance { get; set; }

        // Distributions
        public AttendanceDistributionModel Distribution { get; set; }

        // Lists
        public List<GrowthCenterPerformanceModel> Centers { get; set; }
        public List<AttendanceTrendModel> Trends { get; set; }
    }

    public class EventsReportViewModel
    {
        // Statistics
        public int TotalEvents { get; set; }
        public int TotalAttendance { get; set; }
        public int UpcomingEvents { get; set; }
        public double AverageAttendance { get; set; }

        // Highest Attendance
        public HighestAttendanceEventModel HighestAttendance { get; set; }

        // Charts Data
        public List<TopEventChartModel> TopEvents { get; set; }
        public List<EventTrendModel> Trends { get; set; }
        public List<MonthlyEventAttendanceModel> MonthlyAttendance { get; set; }

        // All Events
        public List<EventDetailModel> AllEvents { get; set; }
    }

    // ================= CHURCH MODELS =================

    public class HighestAttendanceModel
    {
        public string Name { get; set; }
        public int Attendance { get; set; }
        public string Type { get; set; } // Ministry, Event, Growth Center
    }

    public class GenderDistributionModel
    {
        public int Male { get; set; }
        public int Female { get; set; }
        public int Total => Male + Female;
        public double MalePercentage => Total > 0 ? (Male / (double)Total) * 100 : 0;
        public double FemalePercentage => Total > 0 ? (Female / (double)Total) * 100 : 0;
    }

    public class AgeDemographicsModel
    {
        public int Children { get; set; } // 0-12 years
        public int Youth { get; set; }    // 13-25 years
        public int Adults { get; set; }   // 26-55 years
        public int Seniors { get; set; }  // 56+ years
        public int Total => Children + Youth + Adults + Seniors;
    }

    public class MaritalStatusModel
    {
        public int Single { get; set; }
        public int Married { get; set; }
        public int Divorced { get; set; }
        public int Widowed { get; set; }
        public int Total => Single + Married + Divorced + Widowed;
    }

    public class EducationLevelModel
    {
        public int None { get; set; }
        public int Primary { get; set; }
        public int Secondary { get; set; }
        public int Tertiary { get; set; }
        public int University { get; set; }
        public int Total => None + Primary + Secondary + Tertiary + University;
    }

    public class EmploymentStatusModel
    {
        public int Employed { get; set; }
        public int SelfEmployed { get; set; }
        public int Unemployed { get; set; }
        public int Student { get; set; }
        public int Retired { get; set; }
        public int Total => Employed + SelfEmployed + Unemployed + Student + Retired;
    }

    public class TopMinistryModel
    {
        public int MinistryId { get; set; }
        public string MinistryName { get; set; }
        public int TotalMembers { get; set; }
        public string LeaderName { get; set; }
        public double Growth { get; set; }
        public int Rank { get; set; }
    }

    public class TopEventModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public int Attendance { get; set; }
        public string Status { get; set; }
        public int Rank { get; set; }
    }

    public class GrowthTrendModel
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public int NewMembers { get; set; }
        public int TotalMembers { get; set; }
    }

    // ================= MINISTRY MODELS =================

    public class HighestAttendanceMinistryModel
    {
        public int MinistryId { get; set; }
        public string MinistryName { get; set; }
        public int Attendance { get; set; }
        public double AverageAttendance { get; set; }
    }

    public class MinistryGenderDataModel
    {
        public int MinistryId { get; set; }
        public string MinistryName { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Total => Male + Female;
        public double MalePercentage => Total > 0 ? (Male / (double)Total) * 100 : 0;
        public double FemalePercentage => Total > 0 ? (Female / (double)Total) * 100 : 0;
    }

    public class MinistryDetailModel
    {
        public int MinistryId { get; set; }
        public string MinistryName { get; set; }
        public string LeaderName { get; set; }
        public int TotalMembers { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int YouthCount { get; set; }
        public int WeeklyMeetings { get; set; }
        public double AverageAttendance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MinistryPerformanceModel
    {
        public int MinistryId { get; set; }
        public string MinistryName { get; set; }
        public int TotalMembers { get; set; }
        public double AverageAttendance { get; set; }
        public double Growth { get; set; }
    }

    // ================= GROWTH CENTER MODELS =================

    public class HighestAttendanceCenterModel
    {
        public int CenterId { get; set; }
        public string CenterName { get; set; }
        public int Attendance { get; set; }
        public int TotalMeetings { get; set; }
        public double AverageAttendance { get; set; }
    }

    public class AttendanceDistributionModel
    {
        public int Members { get; set; }
        public int Visitors { get; set; }
        public int Children { get; set; }
        public int Total => Members + Visitors + Children;
    }

    public class GrowthCenterPerformanceModel
    {
        public int CenterId { get; set; }
        public string CenterName { get; set; }
        public int TotalMeetings { get; set; }
        public int TotalAttendance { get; set; }
        public double AverageAttendance { get; set; }
        public int TotalMembers { get; set; }
        public int TotalVisitors { get; set; }
        public int TotalChildren { get; set; }
        public decimal TotalOfferings { get; set; }
        public double AverageOffering => TotalMeetings > 0 ? (double)(TotalOfferings / TotalMeetings) : 0;
        public string PerformanceLevel => AverageAttendance > 50 ? "High" : (AverageAttendance > 25 ? "Medium" : "Low");
    }

    public class AttendanceTrendModel
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public int Attendance { get; set; }
        public int NewVisitors { get; set; }
    }

    // ================= EVENT MODELS =================

    public class HighestAttendanceEventModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int Attendance { get; set; }
        public DateTime EventDate { get; set; }
    }

    public class TopEventChartModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int Attendance { get; set; }
    }

    public class EventTrendModel
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public int Attendance { get; set; }
        public int EventCount { get; set; }
    }

    public class EventCategoryModel
    {
        public string Category { get; set; }
        public int Count { get; set; }
        public int TotalAttendance { get; set; }
        public double Percentage { get; set; }
    }

    public class MonthlyEventAttendanceModel
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public int Attendance { get; set; }
        public int EventCount { get; set; }
    }

    public class EventDetailModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public int Attendance { get; set; }
        public string Status { get; set; } // Upcoming, Ongoing, Completed, Cancelled
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    // ================= DASHBOARD MODELS =================

    public class DashboardSummaryModel
    {
        // Church Overview
        public int TotalMembers { get; set; }
        public int TotalGrowthCenters { get; set; }
        public int TotalMinistries { get; set; }
        public int TotalEvents { get; set; }

        // Attendance & Offerings
        public int TotalAttendance { get; set; }
        public int TotalMeetings { get; set; }
        public decimal TotalOfferings { get; set; }

        // Averages
        public double AverageAttendance { get; set; }
        public int MonthlyMeetings { get; set; }
        public double GrowthPercentage { get; set; }

        // Demographics Summary
        public int TotalMembersMale { get; set; }
        public int TotalMembersFemale { get; set; }
        public int TotalChildren { get; set; }
        public int TotalVisitors { get; set; }

        // Trends Data
        public List<TrendDataPoint> TrendsData { get; set; }
    }

    public class TrendDataPoint
    {
        public string Month { get; set; }
        public int TotalAttendance { get; set; }
        public decimal TotalOfferings { get; set; }
    }
}