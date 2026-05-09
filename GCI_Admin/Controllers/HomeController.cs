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
        public HomeController(IMembersService membersService, IEventsService eventsService)
        {
            _membersService = membersService;
            _eventsService = eventsService;
        }

        // GET: HomeController1
        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboard = new DashboardViewModel();

                var allMembers = await _membersService.GetAllMembersAsync();
                var events = await _eventsService.GetAllEventsAsync();
                var previousMonthMembers = await _membersService.GetMembersByDateRangeAsync(DateTime.Now.AddMonths(-1), DateTime.Now);
                var previousMonthActiveMembers = await _membersService.GetActiveMembersByDateRangeAsync(DateTime.Now.AddMonths(-1), DateTime.Now);
                var previousMonthEvents = await _eventsService.GetEventsByDateRangeAsync(DateTime.Now.AddMonths(-1), DateTime.Now);

                var members = allMembers?.Data ?? new List<Member>();

                if (dashboard.MemberStatus == null)
                {
                    dashboard.MemberStatus = new MemberStatusModel();
                }
                var upcomingEvents = events.Data.Where(e=>e.IsActive);
                

                // Assign members to respective status lists
                dashboard.MemberStatus.AllMembers = members;
                dashboard.MemberStatus.MembershipClassMembers = members.Where(x => x.StatusId == 2).ToList();
                dashboard.MemberStatus.ActiveMembers = members.Where(x => x.StatusId == 1).ToList();
                dashboard.MemberStatus.InactiveMembers = members.Where(x => x.StatusId == 3).ToList();
                dashboard.MemberStatus.TransferredMembers = members.Where(x => x.StatusId == 4).ToList();
                dashboard.MemberStatus.PromotedToGlory = members.Where(x => x.StatusId == 5).ToList();
                dashboard.MemberStatus.WithdrawnMembers = members.Where(x => x.StatusId == 6).ToList();

                // For backward compatibility - NonMembers (all except Active Members with StatusId 1)
                dashboard.MemberStatus.NonMembers = members.Where(x => x.StatusId != 1).ToList();

                // Total counts
                dashboard.TotalMembers = members.Count;
                dashboard.TotalActiveMembers = dashboard.MemberStatus.ActiveMembers.Count;

                // Calculate percentages for progress bars
                dashboard.TotalMembersPercentage = dashboard.TotalMembers > 0 ?
                    Math.Round((decimal)dashboard.TotalActiveMembers / dashboard.TotalMembers * 100, 2) : 0;

                dashboard.ActiveMembersPercentage = dashboard.TotalActiveMembers > 0 ?
                    Math.Round((decimal)dashboard.MemberStatus.MembershipClassMembers.Count / dashboard.TotalActiveMembers * 100, 2) : 0;

                dashboard.EventCompletionPercentage = dashboard.UpcomingEvents > 0 ?
                    Math.Round((decimal)events?.Data?.Count(e => e.EventDate >= DateTime.Now && e.EventDate <= DateTime.Now.AddDays(7)) / dashboard.UpcomingEvents * 100, 2) : 0;

                // Calculate growth percentages (compared to previous month)
                int previousTotalMembers = previousMonthMembers.Data.Count ;
                int previousActiveMembers = previousMonthActiveMembers.Data.Count ;
                int previousEvents = previousMonthEvents.Data.Count ;

                dashboard.MemberGrowthPercentage = previousTotalMembers > 0 ?
                    Math.Round((decimal)(dashboard.TotalMembers - previousTotalMembers) / previousTotalMembers * 100, 2) :
                    (dashboard.TotalMembers > 0 ? 100 : 0);

                dashboard.ActiveMemberGrowthPercentage = previousActiveMembers > 0 ?
                    Math.Round((decimal)(dashboard.TotalActiveMembers - previousActiveMembers) / previousActiveMembers * 100, 2) :
                    (dashboard.TotalActiveMembers > 0 ? 100 : 0);

                dashboard.EventChangePercentage = previousEvents > 0 ?
                    Math.Round((decimal)(dashboard.UpcomingEvents - previousEvents) / previousEvents * 100, 2) :
                    (dashboard.UpcomingEvents > 0 ? 100 : 0);

                // Events
                dashboard.UpcomingEvents = upcomingEvents.Count();
                dashboard.UpcomingEvent = upcomingEvents.ToList();

                return View(dashboard);
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"HomeController Index Error: {ex}");

                TempData["Error"] = "Unable to load dashboard.";

                return View(new DashboardViewModel
                {
                    UpcomingEvent = new List<Event>(),
                    MemberStatus = new MemberStatusModel(),
                    TotalMembersPercentage = 0,
                    ActiveMembersPercentage = 0,
                    EventCompletionPercentage = 0,
                    MemberGrowthPercentage = 0,
                    ActiveMemberGrowthPercentage = 0,
                    EventChangePercentage = 0
                });
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
                        data.Add(membersOnDate.Data.Count );
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
                        data.Add(fullMembersOnDate.Data.Count );
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
