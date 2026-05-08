using GCI_Admin.DBOperations;
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
        public async Task<IActionResult> AddMinistryLeader()
        {
            try
            {
                var viewModel = new MembersMinistriesViewModel();

                // Load members who are not already ministry leaders (or all active members)
                var members = await _context.Members
                    .Where(m => m.StatusId==1)
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = $"{m.FirstName} {m.OtherNames} ({m.Email})"
                    })
                    .ToListAsync();

                // Load all active ministries
                var ministries = await _context.Ministries
                    .Where(m => m.IsActive)
                    .Select(m => new SelectListItem
                    {
                        Value = m.MinistryId.ToString(),
                        Text = m.MinistryName
                    })
                    .ToListAsync();

                ViewBag.Members = members;
                ViewBag.Ministries = ministries;

                return PartialView("_AddMinistryLeaderPartial", viewModel);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error loading AddMinistryLeader partial: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}