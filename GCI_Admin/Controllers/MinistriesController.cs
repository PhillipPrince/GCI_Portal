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
        private readonly IMembersService _members;
        private readonly AppDbContext _context;

        public MinistriesController(IMinistriesService ministriesService, IMembersService members, AppDbContext context)
        {
            _ministriesService = ministriesService;
            _members = members;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                MinistriesData ministriesData = new MinistriesData();

                var ministryRes = await _ministriesService.GetAllMinistriesAsync();
                var leaderRes = await _ministriesService.GetAllMinistryLeadersAsync();

                if (ministryRes != null)
                {
                    ministriesData.Ministries = ministryRes.Data;
                }

                if (leaderRes != null)
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

        // In your MinistriesController.cs
        public async Task<IActionResult> LoadCreateForm()
        {
            CreateMinistryLeaderDto dto = new CreateMinistryLeaderDto();

            // Get all members (active members only)
            var membersResult = await _members.GetAllMembersAsync();
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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateMinistryLeader([FromForm] MinistryLeaderDto model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return Json(new { success = false, message = "Please correct the validation errors.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        //        }

        //        // Check if member is already a leader in this ministry
        //        var existingLeader = await _ministriesService.GetMinistryLeaderByMemberAndMinistryAsync(model.MemberId, model.MinistryId);
        //        if (existingLeader.Success && existingLeader.Data != null)
        //        {
        //            return Json(new { success = false, message = "This member is already a leader in this ministry." });
        //        }

        //        // Create the ministry leader
        //        var result = await _ministriesRepository.CreateMinistryLeaderAsync(new MinistryLeader
        //        {
        //            MemberId = model.MemberId,
        //            MinistryId = model.MinistryId,
        //            PositionTitle = model.PositionTitle,
        //            Bio = model.Bio,
        //            StartDate = model.StartDate,
        //            EndDate = model.EndDate,
        //            IsActive = model.IsActive,
        //            CreatedAt = DateTime.UtcNow
        //        });

        //        if (result.Success)
        //        {
        //            return Json(new { success = true, message = "Ministry Leader assigned successfully!" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = result.Message ?? "Failed to assign ministry leader." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateMinistryLeader([FromForm] MinistryLeaderDto model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return Json(new { success = false, message = "Please correct the validation errors.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        //        }

        //        // Update the ministry leader
        //        var result = await _ministriesService.UpdateMinistryLeaderAsync(new MinistryLeader
        //        {
        //            MinistryLeaderId = model.MinistryLeaderId,
        //            MemberId = model.MemberId,
        //            MinistryId = model.MinistryId,
        //            PositionTitle = model.PositionTitle,
        //            Bio = model.Bio,
        //            StartDate = model.StartDate,
        //            EndDate = model.EndDate,
        //            IsActive = model.IsActive
        //        });

        //        if (result.Success)
        //        {
        //            return Json(new { success = true, message = "Ministry Leader updated successfully!" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = result.Message ?? "Failed to update ministry leader." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
        //    }
        //}
    }
}