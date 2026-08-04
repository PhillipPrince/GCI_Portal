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
        private readonly GCI_Admin.DBOperations.AppDbContext _appDbContext;

        public RegisterController(IEventsService eventsService, IAssembliesService assembliesService, GCI_Admin.DBOperations.AppDbContext appDbContext)
        {
            _eventsService = eventsService;
            _assembliesService = assembliesService;
            _appDbContext = appDbContext;
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

        [HttpGet("Register/CheckCollectionstatus/{registrationId}")]
        public async Task<IActionResult> CheckCollectionstatus(int registrationId)
        {
            var result = await _eventsService.CheckCollectionstatusAsync(registrationId);

            if (!result.IsSuccess)
                return NotFound();

            return Json(new { CollectionstatusId = result.Data });
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

        [HttpPost("Register/SubmitGroupRest")]
        public async Task<IActionResult> SubmitGroupRest([FromBody] SubmitGroupRestDto dto)
        {
            if (dto == null || dto.guests == null)
                return BadRequest(new { isSuccess = false, message = "Invalid data." });

            var result = await _eventsService.SubmitGroupRestAsync(dto);

            if (result.IsSuccess)
            {
                return Ok(new { isSuccess = true, message = result.Message, groupId = result.Data });
            }
            return BadRequest(new { isSuccess = false, message = result.Message });
        }

        [HttpPost("Register/SponsorSubmit")]
        public async Task<IActionResult> SponsorSubmit([FromBody] SponsorSubmitDto dto)
        {
            try
            {
                if (dto == null || dto.eventId <= 0 || string.IsNullOrWhiteSpace(dto.sponsorPhone) || string.IsNullOrWhiteSpace(dto.sponsorName))
                    return BadRequest(new { isSuccess = false, message = "Invalid input data." });

                var targetEventResult = await _eventsService.GetEventByIdAsync(dto.eventId);
                if (!targetEventResult.IsSuccess || targetEventResult.Data == null)
                    return NotFound(new { isSuccess = false, message = "Event not found." });

                var targetEvent = targetEventResult.Data;
                var accountRef = $"{targetEvent.Title} Sponsor";

                var normalizedPhone = global::Utils.PhoneHelper.NormalizeKenyanPhoneOrEmail(dto.sponsorPhone);
                var payload = new
                {
                    MemberId = 0,
                    PhoneNumber = normalizedPhone,
                    Amount = dto.amount,
                    Account = accountRef
                };

                string checkoutRequestId = null;
                try
                {
                    using (var httpClient = new System.Net.Http.HttpClient())
                    {
                        var content = new System.Net.Http.StringContent(
                            System.Text.Json.JsonSerializer.Serialize(payload),
                            System.Text.Encoding.UTF8,
                            "application/json"
                        );
                        var response = await httpClient.PostAsync("https://api.gospelcentresinternational.com/api/Payments/MakePayment", content);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonStr = await response.Content.ReadAsStringAsync();
                            if (!string.IsNullOrWhiteSpace(jsonStr))
                            {
                                using (var doc = System.Text.Json.JsonDocument.Parse(jsonStr))
                                {
                                    if (doc.RootElement.TryGetProperty("data", out var dataProp))
                                    {
                                        checkoutRequestId = dataProp.GetString();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    global::Utils.Loggers.DoLogs($"Error calling MakePayment API: {ex}");
                }

                var sponsorRecord = new EventSponsor
                {
                    EventId = dto.eventId,
                    SponsorName = dto.sponsorName.Trim(),
                    SponsorPhone = normalizedPhone,
                    NumberOfPeople = dto.numberOfPeople,
                    Amount = dto.amount,
                    CheckoutRequestID = checkoutRequestId,
                    PaymentStatusId = 2, // 2 = Pending / Success
                    CreatedAt = DateTime.UtcNow
                };

                try
                {
                    _appDbContext.EventSponsors.Add(sponsorRecord);
                    await _appDbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    global::Utils.Loggers.DoLogs($"Error saving EventSponsor record to DB: {ex}");
                }

                return Ok(new { isSuccess = true, message = "Sponsorship initiated successfully.", checkoutRequestId = checkoutRequestId });
            }
            catch (Exception ex)
            {
                global::Utils.Loggers.DoLogs($"SponsorSubmit Exception: {ex}");
                return StatusCode(500, new { isSuccess = false, message = $"Server error initiating sponsorship: {ex.Message}" });
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

    public class SubmitGroupRestDto
    {
        public int primaryRegistrationId { get; set; }
        public List<UsherRegistrationDto> guests { get; set; }
    }

    public class SponsorSubmitDto
    {
        public int eventId { get; set; }
        public string sponsorName { get; set; }
        public string sponsorPhone { get; set; }
        public int numberOfPeople { get; set; }
        public decimal amount { get; set; }
    }
}
