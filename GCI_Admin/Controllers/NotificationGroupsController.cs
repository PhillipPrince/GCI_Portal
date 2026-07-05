using Microsoft.AspNetCore.Mvc;
using GCI_Admin.DBOperations;
using GCI_Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace GCI_Admin.Controllers
{
    public class NotificationGroupsController : Controller
    {
        private readonly AppDbContext _context;

        public NotificationGroupsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var groups = await _context.NotificationGroups.ToListAsync();
            return View(groups);
        }

        [HttpGet]
        public async Task<IActionResult> GetGroup(int id)
        {
            var group = await _context.NotificationGroups.FindAsync(id);
            if (group == null)
            {
                return NotFound(new { message = "Notification Group not found" });
            }
            return Json(group);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] NotificationGroup model)
        {
            try
            {
                if (model.GroupId == 0)
                {
                    model.CreatedAt = DateTime.Now;
                    _context.NotificationGroups.Add(model);
                }
                else
                {
                    var existingGroup = await _context.NotificationGroups.FindAsync(model.GroupId);
                    if (existingGroup == null)
                    {
                        return NotFound(new { message = "Notification Group not found" });
                    }

                    existingGroup.GroupName = model.GroupName;
                    existingGroup.Description = model.Description;
                    existingGroup.IsActive = model.IsActive;
                    existingGroup.UpdatedAt = DateTime.Now;
                    _context.NotificationGroups.Update(existingGroup);
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Notification Group saved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while saving the group", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var group = await _context.NotificationGroups.FindAsync(id);
                if (group == null)
                {
                    return NotFound(new { success = false, message = "Notification Group not found" });
                }

                _context.NotificationGroups.Remove(group);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Notification Group deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the group", error = ex.Message });
            }
        }
    }
}
