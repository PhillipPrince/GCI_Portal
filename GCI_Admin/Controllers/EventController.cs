using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.Services.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Utils;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]

    public class EventController : Controller
    {
        private readonly IEventsService _eventsService;

        public EventController(IEventsService eventsService)
        {
            _eventsService = eventsService;
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
        public IActionResult CreateEvent()
        {
            var dto = new EventDto();
            return View("_CreateEvent", dto);
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
                eventData = new EventViewModel
                {
                    Event = response.Data,
                    Registrations = registrationsResponse.Data
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
        public async Task<IActionResult> Update(int eventId, EventDto dto)
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
        public async Task<IActionResult> GetCurrentYearTheme()
        {
            var result = await _eventsService.GetCurrentYearThemeAsync();

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAnnualTheme(int id, [FromBody] AnnualThemeDto dto)
        {
            var result = await _eventsService.UpdateAnnualThemeAsync(id, dto);

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

       
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SendReminders(int id)
        //{
        //    try
        //    {
        //        var result = await _eventsService.SendRemindersToAllAsync(id);
        //        return Json(new { isSuccess = true, message = $"Reminders sent to {result.Count} participants" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { isSuccess = false, message = ex.Message });
        //    }
        //}
    }
}
