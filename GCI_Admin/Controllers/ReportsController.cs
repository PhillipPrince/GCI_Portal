using GCI_Admin.DBOperations;
using GCI_Admin.Models;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GCI_Admin.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportsService _reportsService;
        private readonly IGrowthCentersService _growthCentersService;
        private readonly ILeadershipService _leadershipService;

        public ReportsController(
            IReportsService reportsService, 
            IGrowthCentersService growthCentersService,
            ILeadershipService leadershipService)
        {
            _reportsService = reportsService;
            _growthCentersService = growthCentersService;
            _leadershipService = leadershipService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get all report data for the dashboard
                var churchReports = await _reportsService.GetChurchReportsAsync();
                var ministryReports = await _reportsService.GetMinistryReportsAsync();
                var growthCenterReports = await _reportsService.GetGrowthCenterReportsAsync();
                var eventsReports = await _reportsService.GetEventsReportsAsync();

                var viewModel = new ReportsDashboardViewModel
                {
                    ChurchReports = churchReports?.IsSuccess == true ? churchReports.Data : null,
                    MinistryReports = ministryReports?.IsSuccess == true ? ministryReports.Data : null,
                    GrowthCenterReports = growthCenterReports?.IsSuccess == true ? growthCenterReports.Data : null,
                    EventsReports = eventsReports?.IsSuccess == true ? eventsReports.Data : null
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Dashboard Index: {ex.Message}");
                ViewBag.Error = "An error occurred while loading the dashboard";
                return View(new ReportsDashboardViewModel());
            }
        }

        #region Church Reports Actions

        [HttpGet]
        public async Task<JsonResult> GetChurchReports()
        {
            try
            {
                var response = await _reportsService.GetChurchReportsAsync();
                return Json(new { success = response?.IsSuccess ?? false, data = response?.Data, message = response?.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetGenderDistribution()
        {
            try
            {
                var response = await _reportsService.GetChurchReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.GenderDistribution });
                }
                return Json(new { success = false, data = new GenderDistributionModel() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAgeDemographics()
        {
            try
            {
                var response = await _reportsService.GetChurchReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.AgeDemographics });
                }
                return Json(new { success = false, data = new AgeDemographicsModel() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetMaritalStatus()
        {
            try
            {
                var response = await _reportsService.GetChurchReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.MaritalStatus });
                }
                return Json(new { success = false, data = new MaritalStatusModel() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTopMinistries()
        {
            try
            {
                var response = await _reportsService.GetChurchReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.TopMinistries });
                }
                return Json(new { success = false, data = new List<TopMinistryModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTopEvents()
        {
            try
            {
                var response = await _reportsService.GetChurchReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.TopEvents });
                }
                return Json(new { success = false, data = new List<TopEventModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetMemberGrowthTrend()
        {
            try
            {
                var response = await _reportsService.GetChurchReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.GrowthTrend });
                }
                return Json(new { success = false, data = new List<GrowthTrendModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Ministry Reports Actions

        [HttpGet]
        public async Task<JsonResult> GetMinistryReports()
        {
            try
            {
                var response = await _reportsService.GetMinistryReportsAsync();
                return Json(new { success = response?.IsSuccess ?? false, data = response?.Data, message = response?.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetMinistryGenderData()
        {
            try
            {
                var response = await _reportsService.GetMinistryReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.MinistryGenderData });
                }
                return Json(new { success = false, data = new List<MinistryGenderDataModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAllMinistries()
        {
            try
            {
                var response = await _reportsService.GetMinistryReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.Ministries });
                }
                return Json(new { success = false, data = new List<MinistryDetailModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetMinistryPerformance()
        {
            try
            {
                var response = await _reportsService.GetMinistryReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.Performance });
                }
                return Json(new { success = false, data = new List<MinistryPerformanceModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Growth Center Reports Actions

        [HttpGet]
        public async Task<JsonResult> GetGrowthCenterReports()
        {
            try
            {
                var response = await _reportsService.GetGrowthCenterReportsAsync();
                return Json(new { success = response?.IsSuccess ?? false, data = response?.Data, message = response?.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetGrowthCentersPerformance()
        {
            try
            {
                var response = await _reportsService.GetGrowthCenterReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.Centers });
                }
                return Json(new { success = false, data = new List<GrowthCenterPerformanceModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetGrowthCenterAttendanceTrends()
        {
            try
            {
                var response = await _reportsService.GetGrowthCenterReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.Trends });
                }
                return Json(new { success = false, data = new List<AttendanceTrendModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Events Reports Actions

        [HttpGet]
        public async Task<JsonResult> GetEventsReports()
        {
            try
            {
                var response = await _reportsService.GetEventsReportsAsync();
                return Json(new { success = response?.IsSuccess ?? false, data = response?.Data, message = response?.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTopEventsChart()
        {
            try
            {
                var response = await _reportsService.GetEventsReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.TopEvents });
                }
                return Json(new { success = false, data = new List<TopEventChartModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetEventTrends()
        {
            try
            {
                var response = await _reportsService.GetEventsReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.Trends });
                }
                return Json(new { success = false, data = new List<EventTrendModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

       

        [HttpGet]
        public async Task<JsonResult> GetAllEvents()
        {
            try
            {
                var response = await _reportsService.GetEventsReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return Json(new { success = true, data = response.Data.AllEvents });
                }
                return Json(new { success = false, data = new List<EventDetailModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Dashboard Summary Actions

        [HttpGet]
        public async Task<JsonResult> GetDashboardSummary()
        {
            try
            {
                var response = await _reportsService.GetDashboardSummaryAsync();
                return Json(new { success = response?.IsSuccess ?? false, data = response?.Data, message = response?.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAttendanceTrends(int months = 6)
        {
            try
            {
                var trends = new List<object>();
                for (int i = months - 1; i >= 0; i--)
                {
                    var date = DateTime.Now.AddMonths(-i);
                    var report = await _reportsService.GetMonthlyGrowthCenterReportAsync(date.Year, date.Month);

                    if (report?.IsSuccess == true && report.Data != null)
                    {
                        trends.Add(new
                        {
                            month = date.ToString("MMM yyyy"),
                            totalAttendance = GetTotalAttendanceFromReport(report.Data),
                            totalOfferings = GetTotalOfferingsFromReport(report.Data)
                        });
                    }
                }
                return Json(trends);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetRecentMeetings(int count = 5)
        {
            try
            {
                var meetings = await _reportsService.GetAllGrowthCenterMeetingsAsync();
                if (meetings?.IsSuccess == true && meetings.Data != null)
                {
                    var recentMeetings = meetings.Data
                        .OrderByDescending(m => m.MeetingDate)
                        .Take(count)
                        .Select(m => new
                        {
                            growthCenterName = m.GrowthCenter?.CenterName,
                            meetingDate = m.MeetingDate,
                            totalAttendance = m.TotalMembers + m.TotalVisitors,
                            offeringCollected = m.OfferingCollected
                        })
                        .ToList();

                    return Json(recentMeetings);
                }
                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTopPerformingCenters(int limit = 5)
        {
            try
            {
                var meetings = await _reportsService.GetAllGrowthCenterMeetingsAsync();
                if (meetings?.IsSuccess == true && meetings.Data != null)
                {
                    var topCenters = meetings.Data
                        .GroupBy(m => m.GrowthCenterId)
                        .Select(g => new
                        {
                            centerId = g.Key,
                            centerName = g.FirstOrDefault()?.GrowthCenter?.CenterName,
                            totalAttendance = g.Sum(m => m.TotalMembers + m.TotalVisitors),
                            averageAttendance = g.Average(m => m.TotalMembers + m.TotalVisitors),
                            meetingCount = g.Count()
                        })
                        .OrderByDescending(c => c.totalAttendance)
                        .Take(limit)
                        .ToList();

                    return Json(topCenters);
                }
                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        #endregion

        #region Original Actions (Preserved)

        // GET: Reports/GrowthCenters
        public async Task<IActionResult> GrowthCenters()
        {
            try
            {
                var response = await _reportsService.GetAllGrowthCenterMeetingsAsync();

                if (response == null || !response.IsSuccess || response.Data == null)
                {
                    TempData["Warning"] = response?.Message ?? "No growth center meetings found";
                    return View(new List<GrowthCenterMeeting>());
                }

                return View(response.Data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading growth center meetings";
                return View(new List<GrowthCenterMeeting>());
            }
        }

        // GET: Reports/GrowthCenterMeetingDetails/5
        public async Task<IActionResult> GrowthCenterMeetingDetails(int id)
        {
            try
            {
                var meetingResult = await _reportsService.GetGrowthCenterMeetingByIdAsync(id);
                if (!meetingResult.IsSuccess || meetingResult.Data == null)
                {
                    TempData["Error"] = meetingResult.Message ?? "Meeting not found";
                    return RedirectToAction("GrowthCenters");
                }

                var growthCenterMeeting = meetingResult.Data;

                var attendeesResult = await _reportsService.GetAttendanceByMeetingIdAsync(id);
                if (attendeesResult.IsSuccess && attendeesResult.Data != null)
                {
                    growthCenterMeeting.Attendees = attendeesResult.Data;
                }
                else
                {
                    growthCenterMeeting.Attendees = new List<GrowthCenterMeetingAttendee>();
                }

                var visitorsResult = await _reportsService.GetVisitorsByMeetingIdAsync(id);
                if (visitorsResult.IsSuccess && visitorsResult.Data != null)
                {
                    growthCenterMeeting.Visitors = visitorsResult.Data;
                }
                else
                {
                    growthCenterMeeting.Visitors = new List<GrowthCenterMeetingVisitor>();
                }


                return View(growthCenterMeeting);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GrowthCenterMeetingDetails: {ex.Message}");
                TempData["Error"] = "An error occurred while loading meeting details";
                return RedirectToAction("GrowthCenters");
            }
        }

        // ======================================
        // MINISTRY LEADER REPORTS
        // ======================================

        // GET: Reports/MinistryLeaderReports
        public async Task<IActionResult> MinistryLeaderReports()
        {
            try
            {
                var response = await _reportsService.GetAllMinistryLeaderReportsAsync();

                if (response == null || !response.IsSuccess || response.Data == null)
                {
                    TempData["Warning"] = response?.Message ?? "No ministry leader reports found";
                    return View(new List<MinistryLeaderReport>());
                }

                return View(response.Data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading ministry leader reports";
                return View(new List<MinistryLeaderReport>());
            }
        }

        // GET: Reports/MinistryLeaderReportDetails/5
        public async Task<IActionResult> MinistryLeaderReportDetails(int id)
        {
            try
            {
                var reportResult = await _reportsService.GetMinistryLeaderReportByIdAsync(id);

                if (!reportResult.IsSuccess || reportResult.Data == null)
                {
                    TempData["Error"] = reportResult.Message ?? "Ministry leader report not found";
                    return RedirectToAction("MinistryLeaderReports");
                }

                var report = reportResult.Data;



                return View(report);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MinistryLeaderReportDetails: {ex.Message}");

                TempData["Error"] = "An error occurred while loading ministry leader report details";

                return RedirectToAction("MinistryLeaderReports");
            }
        }

        // GET: Reports/MinistryLeaderReportsByMinistry/5
        public async Task<IActionResult> MinistryLeaderReportsByMinistry(int id)
        {
            try
            {
                var response = await _reportsService.GetMinistryLeaderReportsByMinistryIdAsync(id);

                if (response == null || !response.IsSuccess || response.Data == null)
                {
                    TempData["Warning"] = response?.Message ?? "No reports found for this ministry";
                    return View("MinistryLeaderReports", new List<MinistryLeaderReport>());
                }

                ViewBag.MinistryId = id;

                return View("MinistryLeaderReports", response.Data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading ministry reports";

                return View("MinistryLeaderReports", new List<MinistryLeaderReport>());
            }
        }

        // GET: Reports/MinistryLeaderReportsByLeader/5
        public async Task<IActionResult> MinistryLeaderReportsByLeader(int id)
        {
            try
            {
                var response = await _reportsService.GetMinistryLeaderReportsByLeaderIdAsync(id);

                if (response == null || !response.IsSuccess || response.Data == null)
                {
                    TempData["Warning"] = response?.Message ?? "No reports found for this leader";
                    return View("MinistryLeaderReports", new List<MinistryLeaderReport>());
                }

                ViewBag.LeaderId = id;

                return View("MinistryLeaderReports", response.Data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading leader reports";

                return View("MinistryLeaderReports", new List<MinistryLeaderReport>());
            }
        }

        // GET: Reports/Deacons
        public async Task<IActionResult> Deacons()
        {
            try
            {
                var response = await _reportsService.GetAllDeaconDutyReportsAsync();

                if (response == null || !response.IsSuccess || response.Data == null)
                {
                    TempData["Warning"] = response?.Message ?? "No deacon duty reports found";
                    return View(new List<DeaconDutySummaryReport>());
                }

                var reports = response.Data;
                var deaconIds = reports.Select(r => r.DeaconId).Distinct();
                var deaconNames = new Dictionary<int, string>();

                foreach (var id in deaconIds)
                {
                    var deaconResult = await _leadershipService.GetDeaconByIdAsync(id);
                    var deacon = deaconResult.IsSuccess ? deaconResult.Data : null;

                    if (deacon != null && deacon.Member != null)
                    {
                        deaconNames[id] = $"{deacon.Member.FirstName} {deacon.Member.OtherNames}";
                    }
                    else
                    {
                        deaconNames[id] = $"Deacon {id}";
                    }
                }

                ViewBag.DeaconNames = deaconNames;
                return View(reports);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Deacons: {ex.Message}");
                TempData["Error"] = "An error occurred while loading deacon reports";
                return View(new List<DeaconDutySummaryReport>());
            }
        }
        public async Task<IActionResult> DeaconReportDetails(int id)
        {
            try
            {
                var response = await _reportsService.GetDeaconDutyReportByIdAsync(id);
                if (response == null || !response.IsSuccess || response.Data == null)
                {
                    TempData["Warning"] = response?.Message ?? "Deacon report not found";
                    return RedirectToAction("Deacons");
                }
                var report = response.Data;
                return View(report);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeaconReportDetails: {ex.Message}");
                TempData["Error"] = "An error occurred while loading deacon report details";
                return RedirectToAction("Deacons");
            }
        }
        // GET: Reports/MinistryLeaders
        public async Task<IActionResult> MinistryLeaders()
        {
            try
            {
                var response = await _reportsService.GetMinistryReportsAsync();
                if (response?.IsSuccess == true && response.Data != null)
                {
                    return View(response.Data.Ministries);
                }
                TempData["Info"] = "No ministry reports available";
                return View(new List<MinistryDetailModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading ministry leader reports";
                return View(new List<MinistryDetailModel>());
            }
        }

        // GET: Reports/GetGrowthCentersList
        [HttpGet]
        public async Task<JsonResult> GetGrowthCentersList()
        {
            try
            {
                var centersResult = await _growthCentersService.GetAllGrowthCentersAsync();
                if (centersResult.IsSuccess && centersResult.Data != null)
                {
                    var centers = centersResult.Data.Where(g => g.IsActive).Select(g => new { id = g.GrowthCenterId, name = g.CenterName }).ToList();
                    return Json(centers);
                }
                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        // GET: Reports/GetDeaconsList
        [HttpGet]
        public async Task<JsonResult> GetDeaconsList()
        {
            try
            {
                var deaconsResult = await _leadershipService.GetAllDeaconsAsync();
                if (deaconsResult.IsSuccess && deaconsResult.Data != null)
                {
                    var deacons = deaconsResult.Data.Where(d => d.IsActive).Select(d => new {
                        id = d.DeaconId,
                        name = d.Member != null ? $"{d.Member.FirstName} {d.Member.OtherNames}" : $"Deacon {d.DeaconId}"
                    }).ToList();
                    return Json(deacons);
                }
                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        // GET: Reports/FilterGrowthCentersReport
        [HttpGet]
        public async Task<PartialViewResult> FilterGrowthCentersReport(DateTime? dateFrom, DateTime? dateTo, int? growthCenterId)
        {
            try
            {
                var allMeetings = await _reportsService.GetAllGrowthCenterMeetingsAsync();

                if (allMeetings == null || !allMeetings.IsSuccess || allMeetings.Data == null)
                {
                    return PartialView("_GrowthCentersReportTable", new List<GrowthCenterMeeting>());
                }

                var filteredMeetings = allMeetings.Data.AsQueryable();

                if (dateFrom.HasValue)
                    filteredMeetings = filteredMeetings.Where(m => m.MeetingDate >= dateFrom.Value);

                if (dateTo.HasValue)
                    filteredMeetings = filteredMeetings.Where(m => m.MeetingDate <= dateTo.Value);

                if (growthCenterId.HasValue && growthCenterId.Value > 0)
                    filteredMeetings = filteredMeetings.Where(m => m.GrowthCenterId == growthCenterId.Value);

                var result = filteredMeetings.ToList();

                return PartialView("_GrowthCentersReportTable", result);
            }
            catch (Exception ex)
            {
                return PartialView("_GrowthCentersReportTable", new List<GrowthCenterMeeting>());
            }
        }

        // GET: Reports/ExportGrowthCentersReport
        public async Task<FileResult> ExportGrowthCentersReport(DateTime? dateFrom, DateTime? dateTo, int? growthCenterId)
        {
            var allMeetings = await _reportsService.GetAllGrowthCenterMeetingsAsync();
            var meetings = allMeetings?.Data ?? new List<GrowthCenterMeeting>();

            if (dateFrom.HasValue)
                meetings = meetings.Where(m => m.MeetingDate >= dateFrom.Value).ToList();

            if (dateTo.HasValue)
                meetings = meetings.Where(m => m.MeetingDate <= dateTo.Value).ToList();

            if (growthCenterId.HasValue && growthCenterId.Value > 0)
                meetings = meetings.Where(m => m.GrowthCenterId == growthCenterId.Value).ToList();

            var csv = "Growth Center,Meeting Date,Start Time,End Time,Bible Study Topic,Members,Visitors,Total Attendance,Children,Offering\n";

            foreach (var meeting in meetings)
            {
                var centerName = meeting.GrowthCenter?.CenterName ?? await GetGrowthCenterName(meeting.GrowthCenterId);
                csv += $"\"{centerName}\",";
                csv += $"{meeting.MeetingDate:yyyy-MM-dd},";
                csv += $"{meeting.StartingTime:hh\\:mm},";
                csv += $"{meeting.FinishingTime:hh\\:mm},";
                csv += $"\"{meeting.BibleStudyTopic}\",";
                csv += $"{meeting.TotalMembers},";
                csv += $"{meeting.TotalVisitors},";
                csv += $"{meeting.TotalMembers + meeting.TotalVisitors},";
                csv += $"{meeting.NumberOfChildren},";
                csv += $"{meeting.OfferingCollected}\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"GrowthCentersReport_{DateTime.Now:yyyyMMdd}.csv");
        }

        #endregion

        #region Helper Methods

        private int GetTotalAttendanceFromReport(List<object> reportData)
        {
            try
            {
                if (reportData == null || !reportData.Any())
                    return 0;

                int totalAttendance = 0;
                foreach (var item in reportData)
                {
                    var props = item.GetType().GetProperties();
                    var membersProp = props.FirstOrDefault(p => p.Name == "TotalMembers");
                    var visitorsProp = props.FirstOrDefault(p => p.Name == "TotalVisitors");

                    if (membersProp != null && visitorsProp != null)
                    {
                        var members = membersProp.GetValue(item) as int? ?? 0;
                        var visitors = visitorsProp.GetValue(item) as int? ?? 0;
                        totalAttendance += members + visitors;
                    }
                }
                return totalAttendance;
            }
            catch
            {
                return 0;
            }
        }

        private decimal GetTotalOfferingsFromReport(List<object> reportData)
        {
            try
            {
                if (reportData == null || !reportData.Any())
                    return 0;

                decimal totalOfferings = 0;
                foreach (var item in reportData)
                {
                    var props = item.GetType().GetProperties();
                    var offeringProp = props.FirstOrDefault(p => p.Name == "OfferingCollected");

                    if (offeringProp != null)
                    {
                        totalOfferings += offeringProp.GetValue(item) as decimal? ?? 0;
                    }
                }
                return totalOfferings;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<string> GetGrowthCenterName(int centerId)
        {
            var centerResult = await _growthCentersService.GetGrowthCenterByIdAsync(centerId);
            var center = centerResult.IsSuccess ? centerResult.Data : null;
            return center?.CenterName ?? $"Center {centerId}";
        }

        #endregion
    }
}