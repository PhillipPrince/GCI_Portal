using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]
    [PermissionAuthorize("VIEW_EVENTS")] // Adjust permission if needed, using VIEW_EVENTS as a likely proxy
    public class ChurchDailyActivitiesController : Controller
    {
        private readonly IChurchDailyActivitiesService _service;
        private readonly IEventsService _eventsService;

        public ChurchDailyActivitiesController(IChurchDailyActivitiesService service, IEventsService eventsService)
        {
            _service = service;
            _eventsService = eventsService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _service.GetAllAsync();
            
            var todayDayOfWeek = System.DateTime.Today.DayOfWeek.ToString();
            var todayActivities = response.Data?.Where(a => a.IsActive && (a.DayOfWeek.Equals(todayDayOfWeek, System.StringComparison.OrdinalIgnoreCase) || a.DayOfWeek.Equals("Daily", System.StringComparison.OrdinalIgnoreCase))).ToList() ?? new System.Collections.Generic.List<GCI_Admin.Models.ChurchDailyActivity>();
            
            var eventsResponse = await _eventsService.GetEventsByDateRangeAsync(System.DateTime.Today, System.DateTime.Today.AddDays(1).AddTicks(-1));
            var todayEvents = eventsResponse.Data ?? new System.Collections.Generic.List<GCI_Admin.Models.Event>();

            ViewBag.TodayDayOfWeek = todayDayOfWeek;
            ViewBag.TodayDate = System.DateTime.Today.ToString("dddd, MMM dd yyyy");
            ViewBag.TodayActivities = todayActivities;
            ViewBag.TodayEvents = todayEvents;

            return View(response.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetActivities()
        {
            var response = await _service.GetAllAsync();
            return PartialView("_ActivitiesTable", response.Data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreateActivity", new ChurchDailyActivityDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChurchDailyActivityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Invalid data submitted" });
            }

            var response = await _service.CreateAsync(dto);
            if (!response.IsSuccess)
                return StatusCode(500, response);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _service.GetByIdAsync(id);
            if (!response.IsSuccess || response.Data == null)
            {
                return NotFound("Activity not found.");
            }

            var dto = new ChurchDailyActivityDto
            {
                Id = response.Data.Id,
                DayOfWeek = response.Data.DayOfWeek,
                ActivityName = response.Data.ActivityName,
                Description = response.Data.Description,
                StartTime = response.Data.StartTime,
                EndTime = response.Data.EndTime,
                IsActive = response.Data.IsActive
            };

            return PartialView("_EditActivity", dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromBody] ChurchDailyActivityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Invalid data submitted" });
            }

            var response = await _service.UpdateAsync(id, dto);
            if (!response.IsSuccess)
                return StatusCode(500, response);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ViewDetails(int id)
        {
            var response = await _service.GetByIdAsync(id);
            if (!response.IsSuccess || response.Data == null)
            {
                return NotFound("Activity not found.");
            }

            return PartialView("_ViewActivity", response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var response = await _service.ToggleStatusAsync(id, isActive);
            if (!response.IsSuccess)
                return StatusCode(500, response);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _service.DeleteAsync(id);
            if (!response.IsSuccess)
                return StatusCode(500, response);

            return Ok(response);
        }
    }
}
