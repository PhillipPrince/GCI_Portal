using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Utils;

namespace GCI_Admin.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly IMeetingsService _meetingsService;

        public MeetingsController(IMeetingsService meetingsService)
        {
            _meetingsService = meetingsService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _meetingsService.GetAllMeetingsAsync();

                if (response.IsSuccess && response.Data != null)
                {
                    var meetings = response.Data;
                    return View(meetings);
                }

                return View(new List<MeetingAttendance>());
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"MeetingController Index Error: {ex.Message}");
                TempData["Error"] = "Unable to load meetings.";
                return View(new List<MeetingAttendance>());
            }
        }
        public async Task<IActionResult> MeetingDetails(int id)
        {
            try
            {
                var response = await _meetingsService.GetMeetingDetailsByIdAsync(id);

                if (!response.IsSuccess || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Meeting not found";
                    return RedirectToAction(nameof(Index));
                }

                return View(response.Data);
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"MeetingController MeetingDetails Error: {ex.Message}");
                TempData["Error"] = "Unable to load meeting details.";
                return RedirectToAction(nameof(Index));
            }
        }



    }
}
