using Microsoft.AspNetCore.Mvc;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.DBOperations.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]
    public class BranchLeadersController : Controller
    {
        private readonly IAssembliesService _assembliesService;
        private readonly MembersRepository _membersRepository;
        private readonly TitlePrefixRepository _prefixRepository;

        public BranchLeadersController(IAssembliesService assembliesService, MembersRepository membersRepository, TitlePrefixRepository prefixRepository)
        {
            _assembliesService = assembliesService;
            _membersRepository = membersRepository;
            _prefixRepository = prefixRepository;
        }

        public async Task<IActionResult> LoadCreateForm()
        {
            var dto = new CreateAssemblyLeaderDto();

            // Get all members
            var membersResult = await _membersRepository.GetAllMembersAsync();
            if (membersResult.Success && membersResult.Data != null)
            {
                dto.Members = membersResult.Data.Where(m => m.StatusId == 1).ToList();
            }

            // Get all assemblies
            var assembliesResult = await _assembliesService.GetAllAssembliesAsync();
            if (assembliesResult.IsSuccess && assembliesResult.Data != null)
            {
                dto.Assemblies = assembliesResult.Data;
            }

            var prefixesResult = await _prefixRepository.GetActivePrefixesAsync();
            if (prefixesResult.Success && prefixesResult.Data != null)
            {
                dto.TitlePrefixes = prefixesResult.Data;
            }

            dto.AssemblyLeader = new AssemblyLeaderDto
            {
                StartDate = DateTime.Today,
                IsActive = true
            };

            ViewBag.IsEdit = false;
            return PartialView("_CreateBranchLeaderPartial", dto);
        }

        public async Task<IActionResult> LoadEditForm(int id)
        {
            var leaderResult = await _assembliesService.GetAssemblyLeaderByIdAsync(id);
            if (leaderResult == null || !leaderResult.IsSuccess || leaderResult.Data == null)
            {
                return NotFound("Assembly leader not found");
            }

            var dto = new CreateAssemblyLeaderDto();

            // Get all members
            var membersResult = await _membersRepository.GetAllMembersAsync();
            if (membersResult.Success && membersResult.Data != null)
            {
                dto.Members = membersResult.Data.Where(m => m.StatusId == 1).ToList();
            }

            // Get all assemblies
            var assembliesResult = await _assembliesService.GetAllAssembliesAsync();
            if (assembliesResult.IsSuccess && assembliesResult.Data != null)
            {
                dto.Assemblies = assembliesResult.Data;
            }

            var prefixesResult = await _prefixRepository.GetActivePrefixesAsync();
            if (prefixesResult.Success && prefixesResult.Data != null)
            {
                dto.TitlePrefixes = prefixesResult.Data;
            }

            var leader = leaderResult.Data;
            dto.AssemblyLeader = new AssemblyLeaderDto
            {
                AssemblyLeaderId = leader.AssemblyLeaderId,
                MemberId = leader.MemberId,
                AssemblyId = leader.AssemblyId,
                TitlePrefixId = leader.TitlePrefixId,
                Bio = leader.Bio,
                StartDate = leader.StartDate,
                EndDate = leader.EndDate,
                IsActive = leader.IsActive
            };

            ViewBag.IsEdit = true;
            return PartialView("_CreateBranchLeaderPartial", dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AssemblyLeaderDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Please correct the validation errors.", errors });
                }

                var result = await _assembliesService.CreateAssemblyLeaderAsync(model);
                if (result.IsSuccess)
                {
                    return Json(new { success = true, message = "Assembly leader assigned successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message ?? "Failed to assign assembly leader." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromBody] AssemblyLeaderDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Please correct the validation errors.", errors });
                }

                var result = await _assembliesService.UpdateAssemblyLeaderAsync(id, model);
                if (result.IsSuccess)
                {
                    return Json(new { success = true, message = "Assembly leader updated successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message ?? "Failed to update assembly leader." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _assembliesService.DeleteAssemblyLeaderAsync(id);
            if (!result.IsSuccess)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, message = "Assembly leader deleted successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus([FromBody] ToggleStatusDto toggleDto)
        {
            var result = await _assembliesService.ToggleAssemblyLeaderStatusAsync(toggleDto.Id, toggleDto.IsActive);
            if (!result.IsSuccess)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, message = result.Message });
        }

        // Action to get details
        public async Task<IActionResult> Details(int id)
        {
            var result = await _assembliesService.GetAssemblyLeaderByIdAsync(id);
            if (result == null || !result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = "Assembly leader not found.";
                return RedirectToAction("Index", "Assemblies");
            }

            return View(result.Data);
        }
    }

    public class ToggleStatusDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
