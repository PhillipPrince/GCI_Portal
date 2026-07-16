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

                eventData = new EventViewModel
                {
                    Event = response.Data,
                    Registrations = registrationsResponse.Data,
                    Feedbacks = feedbacks
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
                var stats = await _eventsService.GetEventRegistrationsByEventIdAsync(id);

                return Json(new
                {
                    isSuccess = true,
                    attendeeCount = stats.Data.Count(),
                    attendedCount = stats.Data.Where(e=> e.HasAttended==true).Count(),
                    pendingCount = stats.Data.Where(e=> e.HasAttended==false).Count(),
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
        public async Task<IActionResult> SendPaymentReminder(int id)
        {
            try
            {
                var reg = await _context.EventRegistrations
                    .Include(r => r.Event)
                    .Include(r => r.Member)
                    .FirstOrDefaultAsync(r => r.RegistrationId == id);
                
                if (reg == null) return Json(new { isSuccess = false, message = "Registration not found." });
                if (reg.PaymentStatusId == 4) return Json(new { isSuccess = false, message = "User has already paid." });

                string phone = reg.MemberId != 0 ? reg.Member?.Phone : reg.GuestPhone;
                string email = reg.MemberId != 0 ? reg.Member?.Email : reg.GuestEmail;
                string name = !string.IsNullOrWhiteSpace(reg.GuestName) ? reg.GuestName : (reg.MemberId != 0 ? reg.Member?.FirstName : "Guest");
                string eventName = reg.Event?.Title ?? "the upcoming event";

                if (string.IsNullOrEmpty(phone) && string.IsNullOrEmpty(email))
                    return Json(new { isSuccess = false, message = "No contact information available for this user." });

                string message = $"Hello {name}, we hope you're doing well. This is a gentle reminder that your payment for {eventName} is still pending." +
                          $" \nPlease click the link to complete your payment." +
                          $"https://portal.gospelcentresinternational.com/Register/Event/{reg.EventId}" +
                          $" \nThank you and God bless!";

                bool sent = false;
                if (!string.IsNullOrEmpty(phone))
                {
                    await _communicationService.SendSmsAsync(phone, message);
                    sent = true;
                }

                if (!string.IsNullOrEmpty(email) && !sent)
                {
                    await _communicationService.SendEmailAsync(email, $"Payment Reminder: {eventName}", message);
                }

                return Json(new { isSuccess = true, message = "Payment reminder sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendBulkPaymentReminders(int eventId)
        {
            try
            {
                var unpaidRegistrations = await _context.EventRegistrations
                    .Include(r => r.Event)
                    .Include(r => r.Member)
                    .Where(r => r.EventId == eventId && r.PaymentStatusId != 4)
                    .ToListAsync();

                if (!unpaidRegistrations.Any())
                    return Json(new { isSuccess = false, message = "No unpaid registrations found for this event." });

                int count = 0;
                foreach (var reg in unpaidRegistrations)
                {
                    string phone = reg.MemberId != 0 ? reg.Member?.Phone : reg.GuestPhone;
                    string email = reg.MemberId != 0 ? reg.Member?.Email : reg.GuestEmail;
                    string name = !string.IsNullOrWhiteSpace(reg.GuestName) ? reg.GuestName : (reg.MemberId != 0 ? reg.Member?.FirstName : "Guest");
                    string eventName = reg.Event?.Title ?? "the upcoming event";

                    if (!string.IsNullOrEmpty(phone) || !string.IsNullOrEmpty(email))
                    {
                        string message = $"Hello {name}, we hope you're doing well. This is a gentle reminder that your payment for {eventName} is still pending." +
                            $" \nPlease click the link to complete your payment." +
                            $"https://portal.gospelcentresinternational.com/Register/Event/{reg.EventId}" +
                            $" \nThank you and God bless!";
                        if (!string.IsNullOrEmpty(phone))
                        {
                            await _communicationService.SendSmsAsync(phone, message);
                        }
                        else if (!string.IsNullOrEmpty(email))
                        {
                            await _communicationService.SendEmailAsync(email, $"Payment Reminder: {eventName}", message);
                        }
                        count++;
                    }
                }

                return Json(new { isSuccess = true, message = $"Successfully sent payment reminders to {count} registrants." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SendAttendanceReminder(int id)
        {
            try
            {
                var reg = await _context.EventRegistrations
                    .Include(r => r.Event)
                    .Include(r => r.Member)
                    .FirstOrDefaultAsync(r => r.RegistrationId == id);
                
                if (reg == null) return Json(new { isSuccess = false, message = "Registration not found." });

                string phone = reg.MemberId != 0 ? reg.Member?.Phone : reg.GuestPhone;
                string email = reg.MemberId != 0 ? reg.Member?.Email : reg.GuestEmail;
                string name = !string.IsNullOrWhiteSpace(reg.GuestName) ? reg.GuestName : (reg.MemberId != 0 ? reg.Member?.FirstName : "Guest");
                string eventName = reg.Event?.Title ?? "the upcoming event";

                if (string.IsNullOrEmpty(phone) && string.IsNullOrEmpty(email))
                    return Json(new { isSuccess = false, message = "No contact information available for this user." });

               
                string message = $"Hello {name}, we hope you're doing well. This is a Gentle reminder about your upcoming attendance for {eventName}. We are looking forward to welcoming you and sharing this special time together. We can't wait to see you. Thank you and God bless!";
                bool sent = false;
                if (!string.IsNullOrEmpty(phone))
                {
                    await _communicationService.SendSmsAsync(phone, message);
                    sent = true;
                }

                if (!string.IsNullOrEmpty(email) && !sent)
                {
                    await _communicationService.SendEmailAsync(email, $"Attendance Reminder: {eventName}", message);
                }

                return Json(new { isSuccess = true, message = "Attendance reminder sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendBulkAttendanceReminders(int eventId)
        {
            try
            {
                var registrations = await _context.EventRegistrations
                    .Include(r => r.Event)
                    .Include(r => r.Member)
                    .Where(r => r.EventId == eventId && r.HasAttended != true)
                    .ToListAsync();

                if (!registrations.Any())
                    return Json(new { isSuccess = false, message = "No registrations pending attendance found for this event." });

                int count = 0;
                foreach (var reg in registrations)
                {
                    string phone = reg.MemberId != 0 ? reg.Member?.Phone : reg.GuestPhone;
                    string email = reg.MemberId != 0 ? reg.Member?.Email : reg.GuestEmail;
                    string name = !string.IsNullOrWhiteSpace(reg.GuestName) ? reg.GuestName : (reg.MemberId != 0 ? reg.Member?.FirstName : "Guest");
                    string eventName = reg.Event?.Title ?? "the upcoming event";

                    if (!string.IsNullOrEmpty(phone) || !string.IsNullOrEmpty(email))
                    {
                        string message = $"Hello {name}, we hope you're doing well. This is a Gentle reminder about your upcoming attendance for {eventName}. We are looking forward to welcoming you and sharing this special time together. We can't wait to see you. Thank you and God bless!";
                        if (!string.IsNullOrEmpty(phone))
                        {
                            await _communicationService.SendSmsAsync(phone, message);
                        }
                        else if (!string.IsNullOrEmpty(email))
                        {
                            await _communicationService.SendEmailAsync(email, $"Attendance Reminder: {eventName}", message);
                        }
                        count++;
                    }
                }

                return Json(new { isSuccess = true, message = $"Successfully sent attendance reminders to {count} registrants." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }
    }
}


