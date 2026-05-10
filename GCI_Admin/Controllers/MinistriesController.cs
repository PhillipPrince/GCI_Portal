using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]
    public class MinistriesController : Controller
    {
        private readonly IMinistriesService _ministriesService;
        private readonly IMembersService _membersService;
        private readonly AppDbContext _context;

        public MinistriesController(IMinistriesService ministriesService, IMembersService membersService, AppDbContext context)
        {
            _ministriesService = ministriesService;
            _membersService = membersService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                MinistriesData ministriesData = new MinistriesData();

                var ministryRes = await _ministriesService.GetAllMinistriesAsync();
                var leaderRes = await _ministriesService.GetAllMinistryLeadersAsync();

                if (ministryRes != null && ministryRes.IsSuccess)
                {
                    ministriesData.Ministries = ministryRes.Data;
                }

                if (leaderRes != null && leaderRes.IsSuccess)
                {
                    ministriesData.MinistryLeaders = leaderRes.Data;
                }

                return View(ministriesData);
            }
            catch (Exception ex)
            {
                // Optional: Log the exception
                Console.WriteLine(ex.Message);
                return View(new MinistriesData());
            }
        }

        // GET: Load Create Form
        public async Task<IActionResult> LoadCreateForm()
        {
            CreateMinistryLeaderDto dto = new CreateMinistryLeaderDto();

            // Get all members (active members only)
            var membersResult = await _membersService.GetAllMembersAsync();
            if (membersResult.IsSuccess && membersResult.Data != null)
            {
                dto.Members = membersResult.Data.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = $"{m.FirstName} {m.OtherNames} ({m.Email})"
                }).ToList();
            }

            // Get all active ministries
            var ministriesResult = await _ministriesService.GetAllMinistriesAsync();
            if (ministriesResult.IsSuccess && ministriesResult.Data != null)
            {
                dto.Ministries = ministriesResult.Data.Where(m => m.IsActive).Select(m => new SelectListItem
                {
                    Value = m.MinistryId.ToString(),
                    Text = m.MinistryName
                }).ToList();
            }

            dto.MinistryLeader = new MinistryLeaderDto
            {
                StartDate = DateTime.Today,
                IsActive = true
            };

            ViewBag.IsEdit = false;
            return PartialView("_CreateMinistryLeaderPartial", dto);
        }
        // POST: Create Ministry Leader
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMinistryLeader([FromForm] MinistryLeaderDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new
                    {
                        success = false,
                        message = "Please correct the validation errors.",
                        errors = errors
                    });
                }

                // Check if member is already an active leader in this ministry
                // Note: You need to add this method to your service interface
                var existingLeaders = await _ministriesService.GetMinistryLeadersByMinistryAsync(model.MinistryId);
                if (existingLeaders.IsSuccess && existingLeaders.Data != null)
                {
                    var isExisting = existingLeaders.Data.Any(l => l.MemberId == model.MemberId && l.IsActive);
                    if (isExisting)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "This member is already an active leader in this ministry."
                        });
                    }
                }

                // Create the ministry leader using the service
                var result = await _ministriesService.CreateMinistryLeaderAsync(model);

                if (result.IsSuccess)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Ministry Leader assigned successfully!",
                        data = result.Data
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = result.Message ?? "Failed to assign ministry leader."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"An error occurred: {ex.Message}"
                });
            }
        }

        // GET: Load Edit Form
        public async Task<IActionResult> LoadEditForm(int id)
        {
            try
            {
                // Get the ministry leader by ID
                var leaderResult = await _ministriesService.GetMinistryLeaderByIdAsync(id);

                if (!leaderResult.IsSuccess || leaderResult.Data == null)
                {
                    return Json(new { success = false, message = "Ministry leader not found." });
                }

                var leader = leaderResult.Data;
                CreateMinistryLeaderDto dto = new CreateMinistryLeaderDto();
                MinistryLeaderDto ministryLeaderDto = new MinistryLeaderDto
                {
                    MinistryLeaderId = leader.MinistryLeaderId,
                    MemberId = leader.MemberId,
                    MinistryId = leader.MinistryId,
                    PositionTitle = leader.PositionTitle,
                    Bio = leader.Bio,
                    StartDate = leader.StartDate,
                    EndDate = leader.EndDate,
                    IsActive = leader.IsActive
                };

                // Get all members (active members only)
                var membersResult = await _membersService.GetAllMembersAsync();
                if (membersResult.IsSuccess && membersResult.Data != null)
                {
                    dto.Members = membersResult.Data.Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = $"{m.FirstName} {m.OtherNames} ({m.Email})",
                        Selected = (m.Id == leader.MemberId)
                    }).ToList();
                }

                // Get all active ministries
                var ministriesResult = await _ministriesService.GetAllMinistriesAsync();
                if (ministriesResult.IsSuccess && ministriesResult.Data != null)
                {
                    dto.Ministries = ministriesResult.Data.Where(m => m.IsActive).Select(m => new SelectListItem
                    {
                        Value = m.MinistryId.ToString(),
                        Text = m.MinistryName,
                        Selected = (m.MinistryId == leader.MinistryId)
                    }).ToList();
                }

                dto.MinistryLeader = ministryLeaderDto;

                ViewBag.IsEdit = true;
                return PartialView("_CreateMinistryLeaderPartial", dto);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error loading form: {ex.Message}" });
            }
        }

        // POST: Update Ministry Leader
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMinistryLeader([FromForm] MinistryLeaderDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new
                    {
                        success = false,
                        message = "Please correct the validation errors.",
                        errors = errors
                    });
                }

                // Check if member is already an active leader in this ministry (excluding current)
                var existingLeaders = await _ministriesService.GetMinistryLeadersByMinistryAsync(model.MinistryId);
                if (existingLeaders.IsSuccess && existingLeaders.Data != null)
                {
                    var isExisting = existingLeaders.Data.Any(l => l.MemberId == model.MemberId
                                                                 && l.IsActive
                                                                 && l.MinistryLeaderId != model.MinistryLeaderId);
                    if (isExisting)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "This member is already an active leader in this ministry."
                        });
                    }
                }

                // Update the ministry leader using the service
                var result = await _ministriesService.UpdateMinistryLeaderAsync(model.MinistryLeaderId, model);

                if (result.IsSuccess)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Ministry Leader updated successfully!",
                        data = result.Data
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = result.Message ?? "Failed to update ministry leader."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"An error occurred: {ex.Message}"
                });
            }
        }

        // POST: Delete Ministry Leader
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMinistryLeader(int id)
        {
            try
            {
                var result = await _ministriesService.DeleteMinistryLeaderAsync(id);

                if (result.IsSuccess)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Ministry Leader deleted successfully!"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = result.Message ?? "Failed to delete ministry leader."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"An error occurred: {ex.Message}"
                });
            }
        }

        // GET: Get Ministry Leader Details
        public async Task<IActionResult> GetMinistryLeaderDetails(int id)
        {
            try
            {
                var result = await _ministriesService.GetMinistryLeaderByIdAsync(id);

                if (!result.IsSuccess || result.Data == null)
                {
                    return Json(new { success = false, message = "Ministry leader not found." });
                }

                var leader = result.Data;

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        leader.MinistryLeaderId,
                        MemberName = leader.Member != null ? $"{leader.Member.FirstName} {leader.Member.OtherNames}" : "N/A",
                        leader.MemberId,
                        MinistryName = leader.Ministry != null ? leader.Ministry.MinistryName : "N/A",
                        leader.MinistryId,
                        leader.PositionTitle,
                        leader.Bio,
                        StartDate = leader.StartDate?.ToString("yyyy-MM-dd"),
                        EndDate = leader.EndDate?.ToString("yyyy-MM-dd"),
                        leader.IsActive,
                        Status = leader.IsActive ? "Active" : "Inactive"
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: Get Leaders by Ministry
        public async Task<IActionResult> GetLeadersByMinistry(int ministryId)
        {
            try
            {
                var result = await _ministriesService.GetMinistryLeadersByMinistryAsync(ministryId);

                if (!result.IsSuccess)
                {
                    return Json(new { success = false, message = "Failed to retrieve leaders." });
                }

                var leaders = result.Data.Select(l => new
                {
                    l.MinistryLeaderId,
                    MemberName = l.Member != null ? $"{l.Member.FirstName} {l.Member.OtherNames}" : "N/A",
                    l.PositionTitle,
                    StartDate = l.StartDate?.ToString("yyyy-MM-dd"),
                    EndDate = l.EndDate?.ToString("yyyy-MM-dd"),
                    l.IsActive,
                    Status = l.IsActive ? "Active" : "Inactive"
                });

                return Json(new { success = true, data = leaders });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: Get Active Leaders
        public async Task<IActionResult> GetActiveLeaders()
        {
            try
            {
                var result = await _ministriesService.GetActiveMinistryLeadersAsync();

                if (!result.IsSuccess)
                {
                    return Json(new { success = false, message = "Failed to retrieve active leaders." });
                }

                var leaders = result.Data.Select(l => new
                {
                    l.MinistryLeaderId,
                    MemberName = l.Member != null ? $"{l.Member.FirstName} {l.Member.OtherNames}" : "N/A",
                    MinistryName = l.Ministry != null ? l.Ministry.MinistryName : "N/A",
                    l.PositionTitle,
                    StartDate = l.StartDate?.ToString("yyyy-MM-dd"),
                    l.IsActive
                });

                return Json(new { success = true, data = leaders });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}