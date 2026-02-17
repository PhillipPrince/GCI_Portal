using System.Diagnostics;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Utils;

namespace GCI_Admin.Controllers
{
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
        public async Task<IActionResult> SubmitNewEvent(EventDto dto)
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
        public async Task<IActionResult> GetById(int eventId)
        {
            try
            {
                ApiResponse<Event> response = await _eventsService.GetEventByIdAsync(eventId);

                if (!response.IsSuccess)
                    return NotFound(response);

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
        [HttpGet]
        public async Task<IActionResult> GetEventRegistrations()
        {
            try
            {
                ApiResponse<List<EventRegistration>> response = await _eventsService.GetEventRegistrationsAsync();

                if (!response.IsSuccess)
                    return BadRequest(response);

                return PartialView("_EventRegistrationsTable", new List<EventRegistration>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<EventRegistration>>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }
        //create view for the above method and add a link to it in the index view
         [HttpGet]
         public IActionResult EventRegistrations()
         {
            GetEventRegistrations();
             return View();
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



        }
}
