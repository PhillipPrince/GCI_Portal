using GCI_Admin.Models;
using GCI_Admin.DBOperations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]
    public class FaithSpaceController : Controller
    {
        private readonly AppDbContext _context;

        public FaithSpaceController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var posts = await _context.FaithPosts
                    .Include(p => p.Member)
                    .Include(p => p.Comments)
                        .ThenInclude(c => c.Member)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return View(posts);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading posts: " + ex.Message;
                return View(new System.Collections.Generic.List<FaithPost>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleApproval(int id)
        {
            try
            {
                var post = await _context.FaithPosts.FindAsync(id);
                if (post == null)
                {
                    return Json(new { isSuccess = false, message = "Post not found" });
                }

                // Get logged in admin member ID
                int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int memberId);

                post.IsApproved = !post.IsApproved;
                if (post.IsApproved)
                {
                    post.ApprovedBy = memberId > 0 ? memberId : (int?)null;
                    post.ApprovedAt = DateTime.Now;
                }
                else
                {
                    post.ApprovedBy = null;
                    post.ApprovedAt = null;
                }

                await _context.SaveChangesAsync();

                var statusText = post.IsApproved ? "approved" : "unapproved";
                return Json(new { isSuccess = true, message = $"Post has been successfully {statusText}." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var post = await _context.FaithPosts.FindAsync(id);
                if (post == null)
                {
                    return Json(new { isSuccess = false, message = "Post not found" });
                }

                _context.FaithPosts.Remove(post);
                await _context.SaveChangesAsync();

                return Json(new { isSuccess = true, message = "Post has been deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }
    }
}
