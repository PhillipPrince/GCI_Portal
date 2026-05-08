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
                var upcomingEvents = await _eventsService.GetAllEventsAsync();
                var events = await _eventsService.GetUpcomingEventsAsync();

                var members = allMembers?.Data ?? new List<Member>();

                if (dashboard.MemberStatus == null)
                {
                    dashboard.MemberStatus = new MemberStatusModel();
                }

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

                // Events
                dashboard.UpcomingEvents = upcomingEvents?.Data?.Count ?? 0;
                dashboard.UpcomingEvent = events?.Data ?? new List<Event>();

                return View(dashboard);
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"HomeController Index Error: {ex}");

                TempData["Error"] = "Unable to load dashboard.";

                return View(new DashboardViewModel
                {
                    UpcomingEvent = new List<Event>(),
                    MemberStatus = new MemberStatusModel() // Initialize MemberStatus to avoid null reference
                });
            }
        }// GET: HomeController1/Details/5
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
            var allMembers = await _membersService.GetAllMembersAsync();
            var members = allMembers.Data; // Adjust based on your service response structure

            var labels = new List<string>();
            var data = new List<int>();

            if (period == "weekly")
            {
                // Get last 7 days
                for (int i = 6; i >= 0; i--)
                {
                    var date = DateTime.Today.AddDays(-i);
                    labels.Add(date.ToString("ddd")); // Mon, Tue, Wed, etc.

                    var count = members.Count(m => m.CreatedAt.Date == date);
                    data.Add(count);
                }
            }
            else if (period == "monthly")
            {
                // Get current month's weeks
                var today = DateTime.Today;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

                // Group by week (4 weeks)
                var weekSize = daysInMonth / 4;

                for (int week = 0; week < 4; week++)
                {
                    var weekStart = firstDayOfMonth.AddDays(week * weekSize);
                    var weekEnd = (week == 3) ? new DateTime(today.Year, today.Month, daysInMonth) : weekStart.AddDays(weekSize - 1);

                    labels.Add($"Week {week + 1}");

                    var count = members.Count(m => m.CreatedAt.Date >= weekStart && m.CreatedAt.Date <= weekEnd);
                    data.Add(count);
                }
            }
            else // yearly
            {
                // Get last 12 months
                for (int i = 11; i >= 0; i--)
                {
                    var date = DateTime.Today.AddMonths(-i);
                    labels.Add(date.ToString("MMM")); // Jan, Feb, Mar, etc.

                    var count = members.Count(m => m.CreatedAt.Year == date.Year && m.CreatedAt.Month == date.Month);
                    data.Add(count);
                }
            }

            return Json(new { success = true, labels = labels, data = data });
        }
        [HttpGet]
        public async Task<IActionResult> GetFullMembershipData(string period = "weekly")
        {
            var allMembers = await _membersService.GetAllMembersAsync();
            var members = allMembers.Data;

            // Filter members who attained full membership (StatusId == 1 for Active Members or however you track full members)
            var fullMembers = members.Where(m => m.StatusId == 1).ToList(); // Adjust StatusId as needed

            var labels = new List<string>();
            var data = new List<int>();

            if (period == "weekly")
            {
                // Get last 7 days
                for (int i = 6; i >= 0; i--)
                {
                    var date = DateTime.Today.AddDays(-i);
                    labels.Add(date.ToString("ddd")); // Mon, Tue, Wed, etc.

                    var count = fullMembers.Count(m => m.CreatedAt.Date == date);
                    data.Add(count);
                }
            }
            else if (period == "monthly")
            {
                // Get current month's weeks
                var today = DateTime.Today;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

                // Group by week (4 weeks)
                var weekSize = daysInMonth / 4;

                for (int week = 0; week < 4; week++)
                {
                    var weekStart = firstDayOfMonth.AddDays(week * weekSize);
                    var weekEnd = (week == 3) ? new DateTime(today.Year, today.Month, daysInMonth) : weekStart.AddDays(weekSize - 1);

                    labels.Add($"Week {week + 1}");

                    var count = fullMembers.Count(m => m.CreatedAt.Date >= weekStart && m.CreatedAt.Date <= weekEnd);
                    data.Add(count);
                }
            }
            else // yearly
            {
                // Get last 12 months
                for (int i = 11; i >= 0; i--)
                {
                    var date = DateTime.Today.AddMonths(-i);
                    labels.Add(date.ToString("MMM")); // Jan, Feb, Mar, etc.

                    var count = fullMembers.Count(m => m.CreatedAt.Year == date.Year && m.CreatedAt.Month == date.Month);
                    data.Add(count);
                }
            }

            return Json(new { success = true, labels = labels, data = data });
        }
    }
}
