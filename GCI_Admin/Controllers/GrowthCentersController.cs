using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]

    public class GrowthCentersController : Controller
    {
        private readonly IGrowthCentersService _growthCentersService;
        private readonly MembersRepository _membersRepository;

        public GrowthCentersController(IGrowthCentersService growthCentersService, MembersRepository membersRepository)
        {
            _growthCentersService = growthCentersService;
            _membersRepository = membersRepository;
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
            //var leaderResult = await _growthCentersService.GetGCLeaderByIdAsync(id);
            //if (leaderResult.IsSuccess && leaderResult.Data != null)
            //{
            //    dto.GCLeader = new GCLeaderDto
            //    {
            //        GCLeaderId = leaderResult.Data.GrowthCenterLeaderId,
            //        MemberId = leaderResult.Data.MemberId,
            //        GrowthCenterId = leaderResult.Data.GrowthCenterId,
            //        PositionTitle = leaderResult.Data.PositionTitle,
            //        Bio = leaderResult.Data.Bio,
            //        StartDate = leaderResult.Data.StartDate,
            //        EndDate = leaderResult.Data.EndDate,
            //        IsActive = leaderResult.Data.IsActive,
            //        CreatedAt = leaderResult.Data.CreatedAt
            //    };
            //}

            ViewBag.IsEdit = true;
            return PartialView("_CreateGCLeaderPartial", dto);
        }

        // Action to create a new GC leader
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGCLeader([FromForm] GCLeaderDto model)
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
                var result = await _growthCentersService.CreateGCLeaderAsync(new GrowthCenterLeader
                {
                    MemberId = model.MemberId,
                    GrowthCenterId = model.GrowthCenterId,
                    //PositionTitle = model.PositionTitle,
                    Bio = model.Bio,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.UtcNow
                });

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGCLeader([FromForm] GCLeaderDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Please correct the validation errors.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }

                // Update the GC leader
                var result = await _growthCentersService.UpdateGCLeaderAsync(new GrowthCenterLeader
                {
                    GrowthCenterLeaderId = model.GCLeaderId,
                    MemberId = model.MemberId,
                    GrowthCenterId = model.GrowthCenterId,
                   // PositionTitle = model.PositionTitle,
                    Bio = model.Bio,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    IsActive = model.IsActive
                });

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



    }
}