using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GCI_Admin.DBOperations;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]

    public class GrowthCentersController : Controller
    {
        private readonly IGrowthCentersService _growthCentersService;
        private readonly MembersRepository _membersRepository;
        private readonly AppDbContext _context;

        public GrowthCentersController(IGrowthCentersService growthCentersService, MembersRepository membersRepository, AppDbContext context)
        {
            _growthCentersService = growthCentersService;
            _membersRepository = membersRepository;
            _context = context;
        }

        // GET: /GrowthCenters
        public async Task<IActionResult> Index()
        {
            try
            {
                GrowthCentersData data = new GrowthCentersData();

                // Get all growth centers
                var centerRes = await _growthCentersService.GetAllGrowthCentersAsync();
                if (centerRes != null && centerRes.Data != null)
                {
                    data.GrowthCenters = centerRes.Data;
                }

                // Get all growth center leaders
                var leaderRes = await _growthCentersService.GetAllGrowthCenterLeadersAsync();
                if (leaderRes != null && leaderRes.Data != null)
                {
                    data.GrowthCenterLeaders = leaderRes.Data;
                }

                return View(data);
            }
            catch (Exception ex)
            {
                // Optional: log exception
                Console.WriteLine(ex.Message);
                return View(new GrowthCentersData());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitNewCenter([FromBody] GrowthCenterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");
            var response = await _growthCentersService.CreateGrowthCenterAsync(dto);
            if (!response.IsSuccess)
                return BadRequest(response.Message);
            return Ok(response.Data);
        }

        // GET: /GrowthCenters/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var centerRes = await _growthCentersService.GetGrowthCenterByIdAsync(id);
            if (!centerRes.IsSuccess || centerRes.Data == null)
            {
                return NotFound("Growth Center not found.");
            }

            var leaders = await _context.GrowthCenterLeaders
                .Include(l => l.Member)
                .Where(l => l.GrowthCenterId == id)
                .ToListAsync();

            var members = await _context.GrowthCenterMembers
                .Include(m => m.Member)
                .Where(m => m.GrowthCenterId == id)
                .ToListAsync();

            var data = new GrowthCenterDetailsData
            {
                GrowthCenter = centerRes.Data,
                Leaders = leaders,
                Members = members
            };

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(int growthCenterId, int memberId)
        {
            try
            {
                // Check if already a member
                bool exists = await _context.GrowthCenterMembers
                    .AnyAsync(m => m.GrowthCenterId == growthCenterId && m.MemberId == memberId);

                if (exists)
                {
                    return Json(new { success = false, message = "Member is already in this Growth Center." });
                }

                var newMember = new GrowthCenterMember
                {
                    GrowthCenterId = growthCenterId,
                    MemberId = memberId,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.GrowthCenterMembers.Add(newMember);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Member added successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }



        public async Task<IActionResult> LoadCreateForm()
        {
            CreateGCLeaderDto dto = new CreateGCLeaderDto();

            // Get all members
            var membersResult = await _membersRepository.GetAllMembersAsync();
            if (membersResult.Success && membersResult.Data != null)
            {
                dto.Members = membersResult.Data.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = $"{m.FirstName} {m.OtherNames} ({m.Email})"
                }).ToList();
            }

            // Get all growth centers
            var centersResult = await _growthCentersService.GetAllGrowthCentersAsync();
            if (centersResult.IsSuccess && centersResult.Data != null)
            {
                dto.GrowthCenters = centersResult.Data.Select(c => new SelectListItem
                {
                    Value = c.GrowthCenterId.ToString(),
                    Text = c.CenterName
                }).ToList();
            }

            dto.GCLeader = new GCLeaderDto
            {
                StartDate = DateTime.Today,
                IsActive = true
            };

            ViewBag.IsEdit = false;
            return PartialView("_CreateGCLeaderPartial", dto);
        }

        // Action to load the edit form
        public async Task<IActionResult> LoadEditForm(int id)
        {
            CreateGCLeaderDto dto = new CreateGCLeaderDto();

            // Get all members
            var membersResult = await _membersRepository.GetAllMembersAsync();
            if (membersResult.Success && membersResult.Data != null)
            {
                dto.Members = membersResult.Data.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = $"{m.FirstName} {m.OtherNames} ({m.Email})"
                }).ToList();
            }

            // Get all growth centers
            var centersResult = await _growthCentersService.GetAllGrowthCentersAsync();
            if (centersResult.IsSuccess && centersResult.Data != null)
            {
                dto.GrowthCenters = centersResult.Data.Select(c => new SelectListItem
                {
                    Value = c.GrowthCenterId.ToString(),
                    Text = c.CenterName
                }).ToList();
            }

            // Get the existing GC leader
            var leaderResult = await _growthCentersService.GetGCLeaderByIdAsync(id);
            if (leaderResult.IsSuccess && leaderResult.Data != null)
            {
                dto.GCLeader = new GCLeaderDto
                {
                    GCLeaderId = leaderResult.Data.GrowthCenterLeaderId,
                    MemberId = leaderResult.Data.MemberId,
                    GrowthCenterId = leaderResult.Data.GrowthCenterId,
                    Bio = leaderResult.Data.Bio,
                    StartDate = leaderResult.Data.StartDate,
                    EndDate = leaderResult.Data.EndDate,
                    IsActive = leaderResult.Data.IsActive
                };
            }

            ViewBag.IsEdit = true;
            return PartialView("_CreateGCLeaderPartial", dto);
        }

        // Action to create a new GC leader
        [HttpPost]
        public async Task<IActionResult> CreateGCLeader([FromBody] GCLeaderDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Please correct the validation errors.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }

                // Check if member is already a leader in this center
                var existingLeader = await _growthCentersService.GetGCLeaderByMemberAndCenterAsync(model.MemberId, model.GrowthCenterId);
                if (existingLeader.IsSuccess && existingLeader.Data != null)
                {
                    return Json(new { success = false, message = "This member is already a leader in this growth center." });
                }

                // Create the GC leader
                var result = await _growthCentersService.CreateGCLeaderAsync(model);

                if (result.IsSuccess)
                {
                    return Json(new { success = true, message = "GC Leader assigned successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message ?? "Failed to assign GC leader." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        // Action to update a GC leader
        [HttpPost]
        public async Task<IActionResult> UpdateGCLeader([FromBody] GCLeaderDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Please correct the validation errors.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }

                // Update the GC leader
                var result = await _growthCentersService.UpdateGCLeaderAsync(model);

                if (result.IsSuccess)
                {
                    return Json(new { success = true, message = "GC Leader updated successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message ?? "Failed to update GC leader." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGCLeader(int id)
        {
            var result = await _growthCentersService.DeleteGCLeaderAsync(id);
            if (!result.IsSuccess)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, message = "GC Leader deleted successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var result = await _growthCentersService.ToggleGCLeaderStatusAsync(id, isActive);
            if (!result.IsSuccess)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetMembersByGrowthCenter(int growthCenterId)
        {
            try
            {
                var members = await _context.GrowthCenterMembers
                    .Where(m => m.GrowthCenterId == growthCenterId && m.Member.StatusId == 1)
                    .Include(m => m.Member)
                    .Select(m => new {
                        id = m.Member.Id,
                        firstName = m.Member.FirstName,
                        otherNames = m.Member.OtherNames,
                        email = m.Member.Email,
                        phone = m.Member.Phone,
                        gender = m.Member.Gender
                    })
                    .ToListAsync();
                return Json(new { success = true, data = members });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}