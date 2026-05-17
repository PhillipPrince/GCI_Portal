using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Services.IService;
using GCI_Admin.Utils;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class ReportsService : IReportsService
    {
        private readonly ReportsRepository _reportsRepository;
        private readonly MembersRepository _membersRepository;

        public ReportsService(ReportsRepository reportsRepository, MembersRepository membersRepository)
        {
            _reportsRepository = reportsRepository;
            _membersRepository = membersRepository;
        }

        // =========================
        // Growth Center Meetings
        // =========================

        public async Task<ApiResponse<List<GrowthCenterMeeting>>> GetAllGrowthCenterMeetingsAsync()
        {
            var response = new ApiResponse<List<GrowthCenterMeeting>>();

            try
            {
                var result = await _reportsRepository.GetAllGrowthCenterMeetingsAsync();

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetAllGrowthCenterMeetingsAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching growth center meetings";
            }

            return response;
        }

        public async Task<ApiResponse<List<GrowthCenterMeeting>>> GetGrowthCenterMeetingsByCenterIdAsync(int centerId)
        {
            var response = new ApiResponse<List<GrowthCenterMeeting>>();

            try
            {
                var result = await _reportsRepository.GetGrowthCenterMeetingsByCenterIdAsync(centerId);

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetGrowthCenterMeetingsByCenterIdAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching meetings by center";
            }

            return response;
        }

        public async Task<ApiResponse<GrowthCenterMeeting>> GetGrowthCenterMeetingByIdAsync(int meetingId)
        {
            var response = new ApiResponse<GrowthCenterMeeting>();
            try
            {
                var result = await _reportsRepository.GetGrowthCenterMeetingByIdAsync(meetingId);
                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetGrowthCenterMeetingByIdAsync Exception: {ex}");
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching meeting details";
            }
            return response;
        }

        public async Task<ApiResponse<List<GrowthCenterMeetingAttendee>>> GetAttendanceByMeetingIdAsync(int meetingId)
        {
            var response = new ApiResponse<List<GrowthCenterMeetingAttendee>>();

            try
            {
                var result = await _reportsRepository.GetAttendanceByMeetingIdAsync(meetingId);

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetAttendanceByMeetingIdAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching attendance";
            }

            return response;
        }

        public async Task<ApiResponse<List<GrowthCenterMeetingVisitor>>> GetVisitorsByMeetingIdAsync(int meetingId)
        {
            var response = new ApiResponse<List<GrowthCenterMeetingVisitor>>();

            try
            {
                var result = await _reportsRepository.GetVisitorsByMeetingIdAsync(meetingId);

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetVisitorsByMeetingIdAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching visitors";
            }

            return response;
        }

        // =========================
        // Ministry Leader Reports
        // =========================

        public async Task<ApiResponse<List<MinistryLeaderReport>>> GetAllMinistryLeaderReportsAsync()
        {
            var response = new ApiResponse<List<MinistryLeaderReport>>();

            try
            {
                var result = await _reportsRepository.GetAllMinistryLeaderReportsAsync();

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetAllMinistryLeaderReportsAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching ministry leader reports";
            }

            return response;
        }

        public async Task<ApiResponse<MinistryLeaderReport>> GetMinistryLeaderReportByIdAsync(int reportId)
        {
            var response = new ApiResponse<MinistryLeaderReport>();

            try
            {
                var result = await _reportsRepository.GetMinistryLeaderReportByIdAsync(reportId);
                
                if (result.Data != null)
                {
                    var leader = await _membersRepository.GetMemberByIdAsync(result.Data.SubmittedByMinistryLeader.MemberId);
                    result.Data.SubmittedByMinistryLeaderName = leader.Success ? leader.Data.FirstName + " " + leader.Data.OtherNames : "Unknown Leader";
                }

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetMinistryLeaderReportByIdAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching ministry leader report";
            }

            return response;
        }

        public async Task<ApiResponse<List<MinistryLeaderReport>>> GetMinistryLeaderReportsByMinistryIdAsync(int ministryId)
        {
            var response = new ApiResponse<List<MinistryLeaderReport>>();

            try
            {
                var result = await _reportsRepository.GetMinistryLeaderReportsByMinistryIdAsync(ministryId);

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetMinistryLeaderReportsByMinistryIdAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching ministry reports by ministry";
            }

            return response;
        }

        public async Task<ApiResponse<List<MinistryLeaderReport>>> GetMinistryLeaderReportsByLeaderIdAsync(int leaderId)
        {
            var response = new ApiResponse<List<MinistryLeaderReport>>();

            try
            {
                var result = await _reportsRepository.GetMinistryLeaderReportsByLeaderIdAsync(leaderId);

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetMinistryLeaderReportsByLeaderIdAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching ministry reports by leader";
            }

            return response;
        }

        public async Task<ApiResponse<List<MinistryLeaderReport>>> GetMinistryLeaderReportsByDateRangeAsync(DateTime from, DateTime to)
        {
            var response = new ApiResponse<List<MinistryLeaderReport>>();

            try
            {
                var result = await _reportsRepository.GetMinistryLeaderReportsByDateRangeAsync(from, to);

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetMinistryLeaderReportsByDateRangeAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching ministry reports by date range";
            }

            return response;
        }

        // =========================
        // Deacon Reports
        // =========================

        public async Task<ApiResponse<List<DeaconDutySummaryReport>>> GetAllDeaconDutyReportsAsync()
        {
            var response = new ApiResponse<List<DeaconDutySummaryReport>>();
            try
            {
                var result = await _reportsRepository.GetAllDeaconDutyReportsAsync();
                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetAllDeaconDutyReportsAsync Exception: {ex}");
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching deacon reports";
            }
            return response;
        }

        public async Task<ApiResponse<List<DeaconDutySummaryReport>>> GetDeaconDutyReportsByDateRangeAsync(DateTime from, DateTime to)
        {
            var response = new ApiResponse<List<DeaconDutySummaryReport>>();

            try
            {
                var result = await _reportsRepository.GetDeaconDutyReportsByDateRangeAsync(from, to);

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetDeaconDutyReportsByDateRangeAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching deacon reports";
            }

            return response;
        }
        //get deacon report by deacon id
        public async Task<ApiResponse<DeaconDutySummaryReport>> GetDeaconDutyReportByIdAsync(int reportId)
        {
            var response = new ApiResponse<DeaconDutySummaryReport>();
            try
            {
                var result = await _reportsRepository.GetDeaconDutyReportByIdAsync(reportId);
                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetDeaconDutyReportsByDeaconIdAsync Exception: {ex}");
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching deacon reports";
            }
            return response;
        }

        public async Task<ApiResponse<List<DeaconDutySummaryReport>>> GetDeaconDutyReportsByDeaconNameAsync(string deaconName)
        {
            var response = new ApiResponse<List<DeaconDutySummaryReport>>();

            try
            {
                var result = await _reportsRepository.GetDeaconDutyReportsByDeaconNameAsync(deaconName);

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetDeaconDutyReportsByDeaconNameAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error searching deacon reports";
            }

            return response;
        }

        // =========================
        // Growth Center Reports
        // =========================

        public async Task<ApiResponse<List<object>>> GetMonthlyGrowthCenterReportAsync(int year, int month)
        {
            var response = new ApiResponse<List<object>>();

            try
            {
                var result = await _reportsRepository.GetMonthlyGrowthCenterReportAsync(year, month);

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetMonthlyGrowthCenterReportAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error generating monthly report";
            }

            return response;
        }

        public async Task<ApiResponse<List<object>>> GetAttendanceReportAsync(int meetingId)
        {
            var response = new ApiResponse<List<object>>();

            try
            {
                var result = await _reportsRepository.GetAttendanceReportAsync(meetingId);

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetAttendanceReportAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching attendance report";
            }

            return response;
        }

        public async Task<ApiResponse<List<object>>> GetVisitorReportAsync(int meetingId)
        {
            var response = new ApiResponse<List<object>>();

            try
            {
                var result = await _reportsRepository.GetVisitorReportAsync(meetingId);

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetVisitorReportAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching visitor report";
            }

            return response;
        }

        // =========================
        // Church Reports (NEW)
        // =========================

        public async Task<ApiResponse<ChurchReportViewModel>> GetChurchReportsAsync()
        {
            var response = new ApiResponse<ChurchReportViewModel>();

            try
            {
                var result = await _reportsRepository.GetChurchReportsAsync();

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetChurchReportsAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching church reports";
            }

            return response;
        }

        // =========================
        // Ministry Reports (NEW)
        // =========================

        public async Task<ApiResponse<MinistryReportViewModel>> GetMinistryReportsAsync()
        {
            var response = new ApiResponse<MinistryReportViewModel>();

            try
            {
                var result = await _reportsRepository.GetMinistryReportsAsync();

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetMinistryReportsAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching ministry reports";
            }

            return response;
        }

        // =========================
        // Growth Center Reports Enhanced (NEW)
        // =========================

        public async Task<ApiResponse<GrowthCenterReportViewModel>> GetGrowthCenterReportsAsync()
        {
            var response = new ApiResponse<GrowthCenterReportViewModel>();

            try
            {
                var result = await _reportsRepository.GetGrowthCenterReportsAsync();

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetGrowthCenterReportsAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching growth center reports";
            }

            return response;
        }

        // =========================
        // Events Reports (NEW)
        // =========================

        public async Task<ApiResponse<EventsReportViewModel>> GetEventsReportsAsync()
        {
            var response = new ApiResponse<EventsReportViewModel>();

            try
            {
                var result = await _reportsRepository.GetEventsReportsAsync();

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetEventsReportsAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error fetching events reports";
            }

            return response;
        }

        // =========================
        // Dashboard (Enhanced - NEW method)
        // =========================

        public async Task<ApiResponse<DashboardSummaryModel>> GetDashboardSummaryModelAsync()
        {
            var response = new ApiResponse<DashboardSummaryModel>();

            try
            {
                var result = await _reportsRepository.GetDashboardSummaryAsync();

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetDashboardSummaryModelAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error loading dashboard summary";
            }

            return response;
        }

        // =========================
        // Dashboard (Original -保持向后兼容)
        // =========================

        public async Task<ApiResponse<DashboardSummaryModel>> GetDashboardSummaryAsync()
        {
            var response = new ApiResponse<DashboardSummaryModel>();

            try
            {
                var result = await _reportsRepository.GetDashboardSummaryAsync();

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetDashboardSummaryAsync Exception: {ex}");

                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "Error loading dashboard summary";
            }

            return response;
        }
    }
}