using GCI_Admin.Models;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IReportsService
    {
        // =========================
        // Growth Center Meetings
        // =========================

        Task<ApiResponse<List<GrowthCenterMeeting>>> GetAllGrowthCenterMeetingsAsync();
        Task<ApiResponse<GrowthCenterMeeting>> GetGrowthCenterMeetingByIdAsync(int meetingId);
        Task<ApiResponse<List<GrowthCenterMeeting>>> GetGrowthCenterMeetingsByCenterIdAsync(int centerId);

        Task<ApiResponse<List<GrowthCenterMeetingAttendee>>> GetAttendanceByMeetingIdAsync(int meetingId);

        Task<ApiResponse<List<GrowthCenterMeetingVisitor>>> GetVisitorsByMeetingIdAsync(int meetingId);

        // =========================
        // Ministry Leader Reports
        // =========================

        Task<ApiResponse<List<MinistryLeaderReport>>> GetAllMinistryLeaderReportsAsync();

        Task<ApiResponse<MinistryLeaderReport>> GetMinistryLeaderReportByIdAsync(int reportId);

        Task<ApiResponse<List<MinistryLeaderReport>>> GetMinistryLeaderReportsByMinistryIdAsync(int ministryId);

        Task<ApiResponse<List<MinistryLeaderReport>>> GetMinistryLeaderReportsByLeaderIdAsync(int leaderId);

        Task<ApiResponse<List<MinistryLeaderReport>>> GetMinistryLeaderReportsByDateRangeAsync(DateTime from, DateTime to);

        // =========================
        // Deacon Duty Reports
        // =========================
        Task<ApiResponse<List<DeaconDutySummaryReport>>> GetAllDeaconDutyReportsAsync();
        Task<ApiResponse<List<DeaconDutySummaryReport>>> GetDeaconDutyReportsByDateRangeAsync(DateTime from, DateTime to);

        Task<ApiResponse<List<DeaconDutySummaryReport>>> GetDeaconDutyReportsByDeaconNameAsync(string deaconName);
        Task<ApiResponse<DeaconDutySummaryReport>> GetDeaconDutyReportByIdAsync(int reportId);

        // =========================
        // Growth Center Reports
        // =========================

        Task<ApiResponse<List<object>>> GetMonthlyGrowthCenterReportAsync(int year, int month);

        Task<ApiResponse<List<object>>> GetAttendanceReportAsync(int meetingId);

        Task<ApiResponse<List<object>>> GetVisitorReportAsync(int meetingId);

        // =========================
        // Church Reports (New)
        // =========================
        Task<ApiResponse<ChurchReportViewModel>> GetChurchReportsAsync();

        // =========================
        // Ministry Reports (New)
        // =========================
        Task<ApiResponse<MinistryReportViewModel>> GetMinistryReportsAsync();

        // =========================
        // Growth Center Reports (New - Enhanced)
        // =========================
        Task<ApiResponse<GrowthCenterReportViewModel>> GetGrowthCenterReportsAsync();

        // =========================
        // Events Reports (New)
        // =========================
        Task<ApiResponse<EventsReportViewModel>> GetEventsReportsAsync();

        // =========================
        // Dashboard (Enhanced)
        // =========================
        new Task<ApiResponse<DashboardSummaryModel>> GetDashboardSummaryAsync();
    }
}