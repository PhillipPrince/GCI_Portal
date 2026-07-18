using Microsoft.AspNetCore.Mvc;
using GCI_Admin.Models;
using GCI_Admin.Services.IService;
using System.Threading.Tasks;

namespace GCI_Admin.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IEventsService _eventsService;
        private readonly IAssembliesService _assembliesService;

        public RegisterController(IEventsService eventsService, IAssembliesService assembliesService)
        {
            _eventsService = eventsService;
            _assembliesService = assembliesService;
        }

        [HttpGet("Register/Event/{eventId}")]
        public async Task<IActionResult> Event(int eventId)
        {
            var eventResult = await _eventsService.GetEventByIdAsync(eventId);
            var targetEvent = eventResult.IsSuccess ? eventResult.Data : null;

            if (targetEvent == null || targetEvent.IsActive != true || !targetEvent.IsPaid)
            {
                return NotFound("Active paid event not found or has concluded.");
            }

            ViewBag.Event = targetEvent;
            
            var assembliesResult = await _assembliesService.GetAllAssembliesAsync();
            ViewBag.Assemblies = assembliesResult.IsSuccess ? assembliesResult.Data : new List<Assembly>();

            return View();
        }

        [HttpGet("Register/CheckRegistration")]
        public async Task<IActionResult> CheckRegistration(string phone, int eventId)
        {
            if (string.IsNullOrWhiteSpace(phone) || eventId <= 0)
                return BadRequest("Invalid input");

            phone = global::Utils.PhoneHelper.NormalizeKenyanPhoneOrEmail(phone);

            var result = await _eventsService.CheckEventRegistrationAsync(phone, eventId);
            
            if (result.IsSuccess)
            {
                return Json(result.Data);
            }

            return Json(new { isRegistered = false });
        }

        [HttpGet("Register/CheckPaymentStatus/{registrationId}")]
        public async Task<IActionResult> CheckPaymentStatus(int registrationId)
        {
            var result = await _eventsService.CheckPaymentStatusAsync(registrationId);

            if (!result.IsSuccess)
                return NotFound();

            return Json(new { paymentStatusId = result.Data });
        }

        [HttpGet("Register/Usher/{eventId}")]
        public async Task<IActionResult> Usher(int eventId)
        {
            var eventResult = await _eventsService.GetEventByIdAsync(eventId);
            var targetEvent = eventResult.IsSuccess ? eventResult.Data : null;

            if (targetEvent == null || targetEvent.IsActive != true)
            {
                return NotFound("Active event not found or has concluded.");
            }

            ViewBag.Event = targetEvent;
            
            var assembliesResult = await _assembliesService.GetAllAssembliesAsync();
            ViewBag.Assemblies = assembliesResult.IsSuccess ? assembliesResult.Data : new List<Assembly>();

            return View("Usher");
        }

        [HttpPost("Register/UsherSubmit")]
        public async Task<IActionResult> UsherSubmit([FromBody] UsherRegistrationDto dto)
        {
            if (dto == null || dto.eventId <= 0)
                return BadRequest(new { isSuccess = false, message = "Invalid data." });

            dto.guestPhone = global::Utils.PhoneHelper.NormalizeKenyanPhoneOrEmail(dto.guestPhone);

            var result = await _eventsService.UsherSubmitRegistrationAsync(dto);

            if (result.IsSuccess)
            {
                return Ok(new { isSuccess = true, message = result.Message });
            }
            else
            {
                // To keep backward compatibility with the existing JS which might check the HTTP status, 
                // if it's "Event not found." or similar, we might return NotFound or BadRequest.
                if (result.Message == "Event not found.")
                {
                    return NotFound(new { isSuccess = false, message = result.Message });
                }
                return BadRequest(new { isSuccess = false, message = result.Message });
            }
        }
    }

    public class UsherRegistrationDto
    {
        public int eventId { get; set; }
        public string guestName { get; set; }
        public string guestPhone { get; set; }
        public string guestEmail { get; set; }
        public string guestAssembly { get; set; }
        public string guestAgeGroup { get; set; }
        public decimal amountPaid { get; set; }
        public bool isPaid { get; set; }
    }
}
