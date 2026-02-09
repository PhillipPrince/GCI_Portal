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
        public IActionResult Create()
        {
            var dto = new EventDto();
            return PartialView("_CreateEvent", dto);
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
