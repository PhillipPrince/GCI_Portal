using Microsoft.AspNetCore.Mvc;
using GCI_Admin.Models;
using GCI_Admin.Services.IService;
using System.Threading.Tasks;

namespace GCI_Admin.Controllers
{
    public class NotificationGroupsController : Controller
    {
        private readonly IAnnouncementsService _announcementsService;

        public NotificationGroupsController(IAnnouncementsService announcementsService)
        {
            _announcementsService = announcementsService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _announcementsService.GetAllNotificationGroupsAsync();
            var groups = result.IsSuccess ? result.Data : new List<NotificationGroup>();
            return View(groups);
        }

        [HttpGet]
        public async Task<IActionResult> GetGroup(int id)
        {
            var result = await _announcementsService.GetNotificationGroupByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound(new { message = "Notification Group not found" });
            }
            return Json(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] NotificationGroup model)
        {
            var result = await _announcementsService.CreateOrUpdateNotificationGroupAsync(model);
            
            if (result.IsSuccess)
            {
                return Ok(new { success = true, message = result.Message });
            }
            else
            {
                if (result.Message == "Notification Group not found")
                {
                    return NotFound(new { message = result.Message });
                }
                return StatusCode(500, new { success = false, message = "An error occurred while saving the group", error = result.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _announcementsService.DeleteNotificationGroupAsync(id);
            
            if (result.IsSuccess)
            {
                return Ok(new { success = true, message = result.Message });
            }
            else
            {
                if (result.Message == "Notification Group not found")
                {
                    return NotFound(new { success = false, message = result.Message });
                }
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the group", error = result.Message });
            }
        }
    }
}
