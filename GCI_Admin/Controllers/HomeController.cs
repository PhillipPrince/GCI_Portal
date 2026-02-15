using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
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
            DashboardViewModel dashboard = new DashboardViewModel();
            var allMembers = await _membersService.GetAllMembersAsync();
            var upcomingEvents = await _eventsService.GetAllEventsAsync();


            dashboard.ActiveMembers = allMembers.Data.Where(m => m.StatusId == 1).ToList();
            dashboard.MembershipClassMembers = allMembers.Data.Where(m => m.StatusId == 2).ToList();
            dashboard.NonMembers = allMembers.Data.Where(m => m.StatusId == 3).ToList();
            dashboard.UpcomingEvents = upcomingEvents.Data.Count;
            dashboard.TotalActiveMembers = dashboard.ActiveMembers.Count;
            dashboard.TotalMembers = allMembers.Data.Count;

            return View(dashboard);
        }
        // GET: HomeController1/Details/5
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
    }
}
