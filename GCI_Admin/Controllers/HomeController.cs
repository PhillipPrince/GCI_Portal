using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utils;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]
    public class HomeController : Controller
    {
        private readonly IMembersService _membersService;
        private readonly IEventsService _eventsService;
        private readonly MeetingsRepository _meetingsRepository;
        public HomeController(IMembersService membersService, IEventsService eventsService, MeetingsRepository meetingsRepository)
        {
            _membersService = membersService;
            _eventsService = eventsService;
            _meetingsRepository = meetingsRepository;
        }

        // GET: HomeController1
        public IActionResult Index()
        {
            return View(new DashboardViewModel()); // Return empty model immediately
        }

        [HttpGet]
        public async Task<IActionResult> GetMembershipStats()
        {
            try
            {
                var dashboard = new DashboardViewModel();
                var allMembers = await _membersService.GetAllMembersAsync();
                var previousMonthMembers = allMembers.Data.Where(m => m.CreatedAt >= DateTime.Now.AddMonths(-1) && m.CreatedAt <= DateTime.Now).ToList();
                var previousMonthActiveMembers = allMembers.Data.Where(m => m.StatusId == 1 && m.CreatedAt >= DateTime.Now.AddMonths(-1) && m.CreatedAt <= DateTime.Now).ToList();

                var members = allMembers?.Data ?? new List<Member>();

                if (dashboard.MemberStatus == null)
                {
                    dashboard.MemberStatus = new MemberStatusModel();
                }

                dashboard.MemberStatus.AllMembers = members;
                dashboard.MemberStatus.MembershipClassMembers = members.Where(x => x.StatusId == 2).ToList();
                dashboard.MemberStatus.ActiveMembers = members.Where(x => x.StatusId == 1).ToList();
                dashboard.MemberStatus.InactiveMembers = members.Where(x => x.StatusId == 3).ToList();
                dashboard.MemberStatus.AwaitingConfirmationMembers = members.Where(x => x.StatusId == 4).ToList();
                dashboard.MemberStatus.TransferredMembers = members.Where(x => x.StatusId == 7).ToList();
                dashboard.MemberStatus.PromotedToGlory = members.Where(x => x.StatusId == 5).ToList();
                dashboard.MemberStatus.WithdrawnMembers = members.Where(x => x.StatusId == 6).ToList();
                
                // For backward compatibility
                dashboard.MemberStatus.NonMembers = members.Where(x => x.StatusId != 1).ToList();

                dashboard.TotalMembers = members.Count;
                dashboard.TotalActiveMembers = dashboard.MemberStatus.ActiveMembers.Count;

                dashboard.TotalMembersPercentage = dashboard.TotalMembers > 0 ?
                    Math.Round((decimal)dashboard.TotalActiveMembers / dashboard.TotalMembers * 100, 2) : 0;
                dashboard.ActiveMembersPercentage = dashboard.TotalActiveMembers > 0 ?
                    Math.Round((decimal)dashboard.MemberStatus.MembershipClassMembers.Count / dashboard.TotalActiveMembers * 100, 2) : 0;

                int previousTotalMembers = previousMonthMembers?.Count ?? 0;
                int previousActiveMembers = previousMonthActiveMembers?.Count ?? 0;

                dashboard.MemberGrowthPercentage = previousTotalMembers > 0
                    ? Math.Round((decimal)(dashboard.TotalMembers - previousTotalMembers) / previousTotalMembers * 100, 2)
                    : (dashboard.TotalMembers > 0 ? 100 : 0);

                dashboard.ActiveMemberGrowthPercentage = previousActiveMembers > 0
                    ? Math.Round((decimal)(dashboard.TotalActiveMembers - previousActiveMembers) / previousActiveMembers * 100, 2)
                    : (dashboard.TotalActiveMembers > 0 ? 100 : 0);

                return PartialView("_MembershipStats", dashboard);
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"HomeController GetMembershipStats Error: {ex}");
                return PartialView("_MembershipStats", new DashboardViewModel { MemberStatus = new MemberStatusModel() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEventStats()
        {
            try
            {
                var dashboard = new DashboardViewModel();
                var events = await _eventsService.GetAllEventsAsync();
                var previousMonthEvents = await _eventsService.GetEventsByDateRangeAsync(DateTime.Now.AddMonths(-1), DateTime.Now);

                var upcomingEvents = events.Data.Where(e => e.IsActive);
                int previousEvents = previousMonthEvents?.Data?.Count ?? 0;

                dashboard.UpcomingEvents = upcomingEvents.Count();
                dashboard.UpcomingEvent = upcomingEvents.ToList();

                dashboard.EventCompletionPercentage = dashboard.UpcomingEvents > 0 ?
                    Math.Round((decimal)events?.Data?.Count(e => e.EventDate >= DateTime.Now && e.EventDate <= DateTime.Now.AddDays(7)) / dashboard.UpcomingEvents * 100, 2) : 0;

                dashboard.EventChangePercentage = previousEvents > 0
                    ? Math.Round((decimal)(dashboard.UpcomingEvents - previousEvents) / previousEvents * 100, 2)
                    : (dashboard.UpcomingEvents > 0 ? 100 : 0);

                return PartialView("_EventStats", dashboard);
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"HomeController GetEventStats Error: {ex}");
                return PartialView("_EventStats", new DashboardViewModel { UpcomingEvent = new List<Event>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMeetingStats()
        {
            try
            {
                var dashboard = new DashboardViewModel();
                
                // Get gender counts using the efficient endpoint from MembersService if possible,
                // but since we only have membersService.GetAllMembersAsync() here, we might have to use that.
                // Wait, maybe we can fetch all members just for this or it's slow?
                // The gender breakdown requires counts. 
                var allMembers = await _membersService.GetAllMembersAsync();
                var members = allMembers?.Data ?? new List<Member>();
                dashboard.TotalMale = members.Count(x => x.Gender == "Male");
                dashboard.TotalFemale = members.Count(x => x.Gender == "Female");
                // dashboard.TotalChildren = members.Count(x => x.Age < 18);
                
                var meetingsStats = await _meetingsRepository.GetDashboardStatsAsync();
                var monthlyTrend = await _meetingsRepository.GetMonthlyAttendanceTrendAsync(6);
                var recentMeetings = await _meetingsRepository.GetAllMeetingsAsync(1, 5);

                if (meetingsStats.Success && meetingsStats.Data != null)
                {
                    dashboard.TotalMeetings = meetingsStats.Data.TotalMeetings;
                    dashboard.TotalAttendees = meetingsStats.Data.TotalAttendees;
                    dashboard.AverageAttendance = Math.Round(meetingsStats.Data.AverageAttendance, 2);
                    dashboard.MeetingsLast30Days = meetingsStats.Data.MeetingsLast30Days;
                    dashboard.AttendeesLast30Days = meetingsStats.Data.AttendeesLast30Days;
                    dashboard.MeetingTypesCount = meetingsStats.Data.MeetingTypesCount;

                    var previousMonthMeetings = await _meetingsRepository.GetMeetingsByDateRangeAsync(DateTime.Now.AddMonths(-2), DateTime.Now.AddMonths(-1));
                    int previousMonthAttendees = previousMonthMeetings.Success ? previousMonthMeetings.Data.Sum(m => m.TotalAttendees) : 0;

                    dashboard.AttendanceGrowthPercentage = previousMonthAttendees > 0
                        ? Math.Round((decimal)(dashboard.TotalAttendees - previousMonthAttendees) / previousMonthAttendees * 100, 2)
                        : (dashboard.TotalAttendees > 0 ? 100 : 0);
                }

                if (monthlyTrend.Success && monthlyTrend.Data != null)
                {
                    dashboard.MonthlyAttendanceTrend = monthlyTrend.Data.Select(m => new MonthlyTrendStats
                    {
                        MonthName = m.MonthName,
                        Year = m.Year,
                        TotalMeetings = m.TotalMeetings,
                        TotalAttendees = m.TotalAttendees,
                        AverageAttendance = m.AverageAttendance
                    }).ToList();
                }
                else
                {
                    dashboard.MonthlyAttendanceTrend = new List<MonthlyTrendStats>();
                }

                if (recentMeetings.Success && recentMeetings.Data != null && recentMeetings.Data.Items != null)
                {
                    dashboard.RecentMeetings = recentMeetings.Data.Items.Select(m => new RecentMeetingStats
                    {
                        MeetingId = m.MeetingAttendancesId,
                        MeetingType = m.MeetingType,
                        MeetingDate = m.MeetingDate,
                        TotalAttendees = m.TotalAttendees,
                        MaleCount = m.MaleCount ?? 0,
                        FemaleCount = m.FemaleCount ?? 0,
                        ChildrenCount = m.ChildrenCount ?? 0
                    }).ToList();
                }
                else
                {
                    dashboard.RecentMeetings = new List<RecentMeetingStats>();
                }

                return PartialView("_MeetingStats", dashboard);
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"HomeController GetMeetingStats Error: {ex}");
                return PartialView("_MeetingStats", new DashboardViewModel { RecentMeetings = new List<RecentMeetingStats>(), MonthlyAttendanceTrend = new List<MonthlyTrendStats>() });
            }
        }
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
       
        [HttpGet]
        public async Task<IActionResult> GetMemberGrowthData(string period = "weekly")
        {
            try
            {
                var labels = new List<string>();
                var data = new List<int>();
                var now = DateTime.Now;

                if (period == "weekly")
                {
                    // Weekly: Sunday to Saturday of current week
                    var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
                    for (int i = 0; i < 7; i++)
                    {
                        var currentDate = startOfWeek.AddDays(i);
                        labels.Add(currentDate.ToString("ddd, MMM dd"));

                        var membersOnDate = await _membersService.GetMembersByDateRangeAsync(
                            currentDate.Date,
                            currentDate.Date.AddDays(1).AddSeconds(-1));
                        data.Add(membersOnDate?.Data?.Count ?? 0);
                    }
                }
                else if (period == "monthly")
                {
                    // Monthly: Weeks 1-4 of current month
                    var startOfMonth = new DateTime(now.Year, now.Month, 1);
                    var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

                    for (int week = 0; week < 4; week++)
                    {
                        var weekStart = startOfMonth.AddDays(week * 7);
                        var weekEnd = weekStart.AddDays(6);

                        if (weekStart > startOfMonth.AddDays(daysInMonth - 1))
                            break;

                        if (weekEnd > startOfMonth.AddDays(daysInMonth - 1))
                            weekEnd = startOfMonth.AddDays(daysInMonth - 1);

                        labels.Add($"Week {week + 1}");

                        var membersInWeek = await _membersService.GetMembersByDateRangeAsync(
                            weekStart.Date,
                            weekEnd.Date.AddDays(1).AddSeconds(-1));
                        data.Add(membersInWeek.Data.Count );
                    }
                }
                else if (period == "yearly")
                {
                    // Yearly: January to December
                    for (int month = 1; month <= 12; month++)
                    {
                        labels.Add(new DateTime(now.Year, month, 1).ToString("MMM"));

                        var startDate = new DateTime(now.Year, month, 1);
                        var endDate = startDate.AddMonths(1).AddDays(-1);

                        var membersInMonth = await _membersService.GetMembersByDateRangeAsync(
                            startDate.Date,
                            endDate.Date.AddDays(1).AddSeconds(-1));
                        data.Add(membersInMonth.Data.Count );
                    }
                }

                return Json(new { success = true, labels = labels, data = data });
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetMemberGrowthData Error: {ex}");
                return Json(new { success = false, message = "Error loading data" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFullMembershipData(string period = "weekly")
        {
            try
            {
                var labels = new List<string>();
                var data = new List<int>();
                var now = DateTime.Now;

                if (period == "weekly")
                {
                    // Weekly: Sunday to Saturday of current week
                    var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
                    for (int i = 0; i < 7; i++)
                    {
                        var currentDate = startOfWeek.AddDays(i);
                        labels.Add(currentDate.ToString("ddd, MMM dd"));

                        var fullMembersOnDate = await _membersService.GetFullMembersByDateRangeAsync(
                            currentDate.Date,
                            currentDate.Date.AddDays(1).AddSeconds(-1));
                        data.Add(fullMembersOnDate?.Data?.Count ?? 0);
                    }
                }
                else if (period == "monthly")
                {
                    // Monthly: Weeks 1-4 of current month
                    var startOfMonth = new DateTime(now.Year, now.Month, 1);
                    var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

                    for (int week = 0; week < 4; week++)
                    {
                        var weekStart = startOfMonth.AddDays(week * 7);
                        var weekEnd = weekStart.AddDays(6);

                        if (weekStart > startOfMonth.AddDays(daysInMonth - 1))
                            break;

                        if (weekEnd > startOfMonth.AddDays(daysInMonth - 1))
                            weekEnd = startOfMonth.AddDays(daysInMonth - 1);

                        labels.Add($"Week {week + 1}");

                        var fullMembersInWeek = await _membersService.GetFullMembersByDateRangeAsync(
                            weekStart.Date,
                            weekEnd.Date.AddDays(1).AddSeconds(-1));
                        data.Add(fullMembersInWeek.Data.Count );
                    }
                }
                else if (period == "yearly")
                {
                    // Yearly: January to December
                    for (int month = 1; month <= 12; month++)
                    {
                        labels.Add(new DateTime(now.Year, month, 1).ToString("MMM"));

                        var startDate = new DateTime(now.Year, month, 1);
                        var endDate = startDate.AddMonths(1).AddDays(-1);

                        var fullMembersInMonth = await _membersService.GetFullMembersByDateRangeAsync(
                            startDate.Date,
                            endDate.Date.AddDays(1).AddSeconds(-1));
                        data.Add(fullMembersInMonth.Data.Count );
                    }
                }

                return Json(new { success = true, labels = labels, data = data });
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetFullMembershipData Error: {ex}");
                return Json(new { success = false, message = "Error loading data" });
            }
        }
    }
}
