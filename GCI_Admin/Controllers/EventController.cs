using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.Services.Service;
using GCI_Admin.DBOperations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Utils;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]

    public class EventController : Controller
    {
        private readonly IEventsService _eventsService;
        private readonly AppDbContext _context;
        private readonly SessionManager _sessionManager;
        private readonly IAnnouncementsService _announcementsService;
        private readonly IMinistriesService _ministriesService;
        private readonly IMembersService _membersService;
        private readonly CommunicationService _communicationService;

        public EventController(IEventsService eventsService, AppDbContext context, SessionManager sessionManager,
            IAnnouncementsService announcementsService, IMinistriesService ministriesService, IMembersService membersService, CommunicationService communicationService)
        {
            _eventsService = eventsService;
            _context = context;
            _sessionManager = sessionManager;
            _announcementsService = announcementsService;
            _ministriesService = ministriesService;
            _membersService = membersService;
            _communicationService = communicationService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ApiResponse<List<Event>> response = await _eventsService.GetAllEventsAsync();

                if (!response.IsSuccess)
                    return View(new List<Event>());

                return View(response.Data);
            }
            catch
            {
                return View(new List<Event>());
            }
        }

      

        [HttpGet]
        public async Task<IActionResult> EventsTable()
        {
            try
            {
                ApiResponse<List<Event>> response = await _eventsService.GetAllEventsAsync();
                return PartialView("_EventsTable", response.Data ?? new List<Event>());
            }
            catch
            {
                return PartialView("_EventsTable", new List<Event>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateEvent()
        {
            var dto = new EventDto();
            try
            {
                ViewBag.NotificationGroups = await _context.NotificationGroups
                    .Where(g => g.IsActive)
                    .Select(g => new DropdownItem { Value = g.GroupId.ToString(), Text = g.GroupName })
                    .ToListAsync();
            }
            catch
            {
                ViewBag.NotificationGroups = new List<DropdownItem>();
            }
            try
            {
                var ministriesRes = await _ministriesService.GetAllMinistriesAsync();
                ViewBag.Ministries = ministriesRes.Data?.Where(m => m.IsActive)
                    .Select(m => new DropdownItem { Value = m.MinistryId.ToString(), Text = m.MinistryName })
                    .ToList() ?? new List<DropdownItem>();
            }
            catch { ViewBag.Ministries = new List<DropdownItem>(); }
            try
            {
                var membersRes = await _membersService.GetAllMembersAsync();
                ViewBag.Members = membersRes.Data ?? new List<Member>();
            }
            catch { ViewBag.Members = new List<Member>(); }
            return View("_CreateEvent", dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                ApiResponse<Event> response = await _eventsService.GetEventByIdAsync(id);
                if (response.Data == null)
                {
                    TempData["Error"] = "Event not found";
                    return RedirectToAction("Index");
                }

                var dto = new EventDto
                {
                    Title = response.Data.Title,
                    Description = response.Data.Description,
                    EventDate = response.Data.EventDate,
                    Location = response.Data.Location,
                    IsPaid = response.Data.IsPaid,
                    Price = response.Data.Price,
                    IsActive = response.Data.IsActive,
                    RequireRegistration = response.Data.RequireRegistration,
                    AllowWalkIns = response.Data.AllowWalkIns,
                    StartDateTime = response.Data.StartDateTime,
                    EndDateTime = response.Data.EndDateTime,
                    GroupId = response.Data.GroupId,
                    MinistryId = response.Data.MinistryId
                };

                ViewBag.EventId = id;
                try
                {
                    ViewBag.NotificationGroups = await _context.NotificationGroups
                        .Where(g => g.IsActive)
                        .Select(g => new DropdownItem { Value = g.GroupId.ToString(), Text = g.GroupName })
                        .ToListAsync();
                }
                catch
                {
                    ViewBag.NotificationGroups = new List<DropdownItem>();
                }
                try
                {
                    var ministriesRes = await _ministriesService.GetAllMinistriesAsync();
                    ViewBag.Ministries = ministriesRes.Data?.Where(m => m.IsActive)
                        .Select(m => new DropdownItem { Value = m.MinistryId.ToString(), Text = m.MinistryName })
                        .ToList() ?? new List<DropdownItem>();
                }
                catch { ViewBag.Ministries = new List<DropdownItem>(); }
                try
                {
                    var membersRes = await _membersService.GetAllMembersAsync();
                    ViewBag.Members = membersRes.Data ?? new List<Member>();
                }
                catch { ViewBag.Members = new List<Member>(); }

                return View("EditEvent", dto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading event edit view: " + ex.Message;
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public async Task<IActionResult> SubmitNewEvent([FromBody] EventDto dto)
        {
            try
            {
                ApiResponse<Event> response = await _eventsService.CreateEventAsync(dto);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Event>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }

       

        [HttpGet]
        public async Task<IActionResult> EventDetails(int id)
        {
            try
            {
                EventViewModel eventData = null;
                ApiResponse<Event> response = await _eventsService.GetEventByIdAsync(id);
                if (response.Data == null)
                {
                    TempData["Error"] = "Event not found";
                    return RedirectToAction("Index");
                }
                var registrationsResponse = await _eventsService.GetEventRegistrationsByEventIdAsync(id);
                var feedbacks = await _context.EventFeedbacks
                    .Include(f => f.Member)
                    .Where(f => f.EventId == id)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();
                var sponsors = await _context.EventSponsors
                    .Where(s => s.EventId == id)
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                eventData = new EventViewModel
                {
                    Event = response.Data,
                    Registrations = registrationsResponse.Data,
                    Feedbacks = feedbacks,
                    Sponsors = sponsors ?? new List<EventSponsor>()
                };  

                return View(eventData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading event details: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int eventId, [FromBody] EventDto dto)
        {
            try
            {
                ApiResponse<Event> response = await _eventsService.UpdateEventAsync(eventId, dto);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Event>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAgeGroups(int eventId, string ageGroups)
        {
            try
            {
                var response = await _eventsService.UpdateEventAgeGroupsAsync(eventId, ageGroups);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Event>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int eventId)
        {
            try
            {
                ApiResponse<bool> response = await _eventsService.DeleteEventAsync(eventId);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }

        // add method for         Task<ApiResponse<List<EventRegistration>>> GetEventRegistrationsAsync();
      
        //create view for the above method and add a link to it in the index view
         [HttpGet]
         public async Task<IActionResult> EventRegistrations()
         {
            ApiResponse<List<EventRegistration>> response = await _eventsService.GetEventRegistrationsAsync();
            if (!response.IsSuccess)
                return BadRequest(response);
            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> UploadEventsExcel(IFormFile file, string uploadOption)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Code = "400",
                        Message = "Please select a file to upload."
                    });
                }

                string createdBy = User?.Identity?.Name ?? "System";

                var response = await _eventsService.ProcessEventExcelUploadAsync(file, createdBy, uploadOption);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int eventId, bool isActive)
        {
            try
            {
                var response = await _eventsService.ToggleEventStatusAsync(eventId, isActive);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }
        //add endpoint to delete
        [HttpDelete]
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            try
            {
                var response = await _eventsService.DeleteEventAsync(eventId);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }


        [HttpGet]
                public IActionResult Themes()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAnnualThemes()
        {
            var user = _sessionManager.GetUserSession<Member>();
            string? assemblyName = (user != null && user.UserRole == 2) ? user.Assembly : null;
            var result = await _eventsService.GetAllAnnualThemesAsync(assemblyName);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAnnualTheme(int id)
        {
            var result = await _eventsService.DeleteAnnualThemeAsync(id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMonthlyThemes()
        {
            var user = _sessionManager.GetUserSession<Member>();
            string? assemblyName = (user != null && user.UserRole == 2) ? user.Assembly : null;
            var result = await _eventsService.GetAllMonthlyThemesAsync(assemblyName);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMonthlyTheme(int id)
        {
            var result = await _eventsService.DeleteMonthlyThemeAsync(id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentYearTheme()
        {
            var user = _sessionManager.GetUserSession<Member>();
            string? assemblyName = (user != null && user.UserRole == 2) ? user.Assembly : null;

            var result = await _eventsService.GetCurrentYearThemeAsync(assemblyName);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAnnualTheme(int id, [FromBody] AnnualThemeDto dto)
        {
            var user = _sessionManager.GetUserSession<Member>();
            string? assemblyName = (user != null && user.UserRole == 2) ? user.Assembly : null;

            var result = await _eventsService.UpdateAnnualThemeAsync(id, dto, assemblyName);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentMonthlyTheme()
        {
            var user = _sessionManager.GetUserSession<Member>();
            string? assemblyName = (user != null && user.UserRole == 2) ? user.Assembly : null;

            var result = await _eventsService.GetCurrentMonthlyThemeAsync(assemblyName);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMonthlyTheme(int id, [FromBody] MonthlyThemeDto dto)
        {
            var user = _sessionManager.GetUserSession<Member>();
            string? assemblyName = (user != null && user.UserRole == 2) ? user.Assembly : null;

            var result = await _eventsService.UpdateMonthlyThemeAsync(id, dto, assemblyName);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRegistrations(int id)
        {
            try
            {
                var registrations = await _eventsService.GetEventRegistrationsByEventIdAsync(id);
                return PartialView("_EventRegistrationsTable", registrations);
            }
            catch (Exception ex)
            {
                return Content($"<div class='alert alert-danger'>{ex.Message}</div>");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStats(int id)
        {
            try
            {
                var targetEventResult = await _eventsService.GetEventByIdAsync(id);
                var eventObj = targetEventResult?.Data;

                var regsResponse = await _eventsService.GetEventRegistrationsByEventIdAsync(id);
                var regs = regsResponse?.Data ?? new List<EventRegistration>();

                var sponsors = await _context.EventSponsors
                    .Where(s => s.EventId == id)
                    .ToListAsync();

                int attendeeCount = regs.Count;
                int attendedCount = regs.Count(r => r.HasAttended == true);

                int regPendingCount = regs.Count(r => r.PaymentStatusId != 4);
                int sponsorPendingCount = sponsors.Count(s => s.PaymentStatusId != 4);
                int totalPendingCount = regPendingCount + sponsorPendingCount;

                decimal regSuccessRev = regs.Where(r => r.PaymentStatusId == 4).Sum(r => r.AmountPaid);
                decimal sponsorSuccessRev = sponsors.Where(s => s.PaymentStatusId == 4).Sum(s => s.Amount);
                decimal totalRevenue = regSuccessRev + sponsorSuccessRev;

                return Json(new
                {
                    isSuccess = true,
                    attendeeCount = attendeeCount,
                    attendedCount = attendedCount,
                    pendingCount = totalPendingCount,
                    revenue = totalRevenue
                });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAttended([FromBody] MarkAttendedRequest request)
        {
            try
            {
                var registration = await _context.EventRegistrations
                    .Include(r => r.Event)
                    .FirstOrDefaultAsync(r => r.RegistrationId == request.Id);

                if (registration == null)
                {
                    return Json(new { success = false, message = "Registration not found." });
                }

                int dayNumber = 1;
                if (registration.Event.StartDateTime.HasValue && registration.Event.EndDateTime.HasValue)
                {
                    var now = DateTime.Now.Date;
                    var startDate = registration.Event.StartDateTime.Value.Date;
                    var endDate = registration.Event.EndDateTime.Value.Date;

                    if (now < startDate)
                    {
                        return Json(new { success = false, message = "Event has not started yet." });
                    }
                    if (now > endDate)
                    {
                        dayNumber = (int)(now - startDate).TotalDays + 1;
                    }
                    else
                    {
                        dayNumber = (int)(now - startDate).TotalDays + 1;
                    }
                }

                var existingAttendance = await _context.EventAttendances
                    .FirstOrDefaultAsync(a => a.EventId == registration.EventId && a.MemberId == registration.MemberId && a.DayNumber == dayNumber);

                if (existingAttendance != null)
                {
                    return Json(new { success = false, message = $"Attendance for Day {dayNumber} is already marked." });
                }

                var newAttendance = new EventAttendance
                {
                    EventId = registration.EventId,
                    MemberId = registration.MemberId,
                    DayNumber = dayNumber,
                    AttendanceDate = DateTime.Now
                };

                _context.EventAttendances.Add(newAttendance);

                registration.HasAttended = true;
                _context.EventRegistrations.Update(registration);

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Attendance for Day {dayNumber} marked successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error marking attendance: {ex.Message}" });
            }
        }

        public class MarkAttendedRequest
        {
            public int Id { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCollectionstatus([FromBody] UpdateCollectionRequest request)
        {
            try
            {
                var registration = await _context.EventRegistrations
                    .FirstOrDefaultAsync(r => r.RegistrationId == request.Id);

                if (registration == null)
                {
                    return Json(new { success = false, message = "Registration not found." });
                }

                registration.PaymentStatusId = request.PaymentStatusId;
                _context.EventRegistrations.Update(registration);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Collection status updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error updating Collection status: {ex.Message}" });
            }
        }

        public class UpdateCollectionRequest
        {
            public int Id { get; set; }
            public int PaymentStatusId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SendEventNotification(int eventId, [FromBody] NotificationDto dto)
        {
            try
            {
                var eventResponse = await _eventsService.GetEventByIdAsync(eventId);
                if (eventResponse.Data == null)
                    return BadRequest(new { isSuccess = false, message = "Event not found." });

                var currentUser = _sessionManager.GetUserSession<GCI_Admin.Models.Member>();
                int createdById = currentUser?.Id ?? 0;

                // Override targeting from the event record
                dto.CreatedById = createdById;
                dto.NotificationGroupId = eventResponse.Data.GroupId ?? 1;
                dto.MinistryId = eventResponse.Data.MinistryId;
                dto.IsChurchWide = !eventResponse.Data.GroupId.HasValue;
                dto.PushNotificationType = "event";
                dto.DeepLinkScreen = "events";
                dto.DeepLinkId = eventId.ToString();

                var savedNotification = await _announcementsService.CreateAnnouncementAsync(dto);
                if (savedNotification != null && savedNotification.IsSuccess)
                {
                    return Ok(new { isSuccess = true, message = "Notification scheduled successfully.", notificationId = savedNotification.Data?.NotificationId });
                }
                else
                {
                    return BadRequest(new { isSuccess = false, message = savedNotification?.Message ?? "Failed to schedule notification." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRegistration(int id)
        {
            try
            {
                var reg = await _context.EventRegistrations.FindAsync(id);
                if (reg != null)
                {
                    _context.EventRegistrations.Remove(reg);
                    await _context.SaveChangesAsync();
                    return Json(new { isSuccess = true, message = "Registration deleted successfully." });
                }
                return Json(new { isSuccess = false, message = "Registration not found." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendCollectionReminder(int id)
        {
            var response = await _eventsService.SendCollectionReminderAsync(id);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> SendBulkCollectionReminders(int eventId)
        {
            var response = await _eventsService.SendBulkCollectionRemindersAsync(eventId);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> SendAttendanceReminder(int id)
        {
            var response = await _eventsService.SendAttendanceReminderAsync(id);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> SendBulkAttendanceReminders(int eventId)
        {
            var response = await _eventsService.SendBulkAttendanceRemindersAsync(eventId);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddSponsor([FromBody] EventSponsor sponsor)
        {
            try
            {
                if (sponsor == null || sponsor.EventId <= 0 || string.IsNullOrWhiteSpace(sponsor.SponsorName) || string.IsNullOrWhiteSpace(sponsor.SponsorPhone))
                {
                    return BadRequest(new { isSuccess = false, message = "Invalid sponsor details provided." });
                }

                sponsor.SponsorName = sponsor.SponsorName.Trim();
                sponsor.SponsorPhone = global::Utils.PhoneHelper.NormalizeKenyanPhoneOrEmail(sponsor.SponsorPhone);
                sponsor.CreatedAt = DateTime.UtcNow;

                _context.EventSponsors.Add(sponsor);
                await _context.SaveChangesAsync();

                return Ok(new { isSuccess = true, message = "Sponsor added successfully." });
            }
            catch (Exception ex)
            {
                global::Utils.Loggers.DoLogs($"AddSponsor Error: {ex}");
                return StatusCode(500, new { isSuccess = false, message = $"Error adding sponsor: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSponsorStatus([FromBody] UpdateSponsorStatusDto dto)
        {
            try
            {
                var sponsor = await _context.EventSponsors.FindAsync(dto.SponsorId);
                if (sponsor == null)
                {
                    return NotFound(new { isSuccess = false, message = "Sponsor not found." });
                }

                sponsor.PaymentStatusId = dto.PaymentStatusId;
                await _context.SaveChangesAsync();

                return Ok(new { isSuccess = true, message = "Sponsor payment status updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = ex.Message });
            }
        }
    }

    public class UpdateSponsorStatusDto
    {
        public int SponsorId { get; set; }
        public int PaymentStatusId { get; set; }
    }
}
