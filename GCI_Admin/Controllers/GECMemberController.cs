using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]

    public class GECMemberController : Controller
    {
        private readonly IGECMemberService _gecMemberService;
        private readonly AppDbContext _context;
        private readonly MembersRepository _membersRepository;
        private readonly GECPositionRepository _positionsRepository;

        public GECMemberController(IGECMemberService gecMemberService, AppDbContext context, MembersRepository repository, GECPositionRepository positionsRepository)
        {
            _gecMemberService = gecMemberService;
            _context = context;
            _membersRepository = repository;
            _positionsRepository = positionsRepository;
        }

        // ✅ INDEX
        public async Task<IActionResult> Index()
        {
            var response = await _gecMemberService.GetGECMembersAsync();
            var members = (response != null && response.IsSuccess && response.Data != null)
                ? response.Data.ToList()
                : new List<GECMember>();

            return View(members);
        }

        //public async Task<IActionResult> Index()
        //{
        //    return View();
        //}




        public async Task<IActionResult> AddNewGecMember()
        {
            CreateGECMemberDto dto = new CreateGECMemberDto();
            // Get all members for the dropdown
            var members = await _membersRepository.GetAllMembersAsync();
            var positions = await _positionsRepository.GetAllPositionsAsync();

            dto.Members = members.Data;
            dto.Positions = positions.Data?.Where(p => p.IsActive).ToList() ?? new List<GECPosition>();


            return View(dto);
        }


        // ✅ CREATE
        [HttpPost]
        public async Task<IActionResult> Create(GECMemberDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            var response = await _gecMemberService.CreateGECMemberAsync(dto);

            if (!response.IsSuccess)
                return BadRequest(response.Message);

            return Json(response);
        }

      

        public async Task<IActionResult> LoadCreateForm()
        {
            CreateGECMemberDto dto = new CreateGECMemberDto();
            var members = await _membersRepository.GetAllMembersAsync();
            var positions = await _positionsRepository.GetAllPositionsAsync();
            dto.Members = members.Data;
            dto.Positions = positions.Data?.Where(p => p.IsActive).ToList() ?? new List<GECPosition>();

            ViewBag.IsEdit = false;
            return PartialView("_CreateGECMemberPartial", dto);
        }



        [HttpPost]
        public async Task<IActionResult> CreateGECMember([FromBody] GECMemberDto dto)
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
                    message = "Validation failed. Please check the form.",
                    errors = errors
                });
            }

            try
            {
                var result = await _gecMemberService.CreateGECMemberAsync(dto);

                if (result.IsSuccess)
                {
                    return Json(new
                    {
                        success = true,
                        message = "GEC member created successfully",
                        data = result.Data
                    });
                }

                return Json(new
                {
                    success = false,
                    message = result.Message ?? "Failed to create GEC member"
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                Loggers.DoLogs("Error creating GEC member: " + ex.ToString());

                return Json(new
                {
                    success = false,
                    message = "An error occurred while creating the member. Please try again."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGECMember([FromBody] GECMemberDto dto)
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
                    message = "Validation failed. Please check the form.",
                    errors = errors
                });
            }

            if (dto.GECId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid GEC member ID"
                });
            }

            try
            {
                var result = await _gecMemberService.UpdateGECMemberAsync(dto);

                if (result.IsSuccess)
                {
                    return Json(new
                    {
                        success = true,
                        message = "GEC member updated successfully",
                        data = result.Data
                    });
                }

                return Json(new
                {
                    success = false,
                    message = result.Message ?? "Failed to update GEC member"
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                Loggers.DoLogs("Error updating GEC member: " + ex.ToString());

                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating the member. Please try again."
                });
            }
        }

        public async Task<IActionResult> LoadEditForm(int id)
        {
            var members = await _membersRepository.GetAllMembersAsync();
            var positions = await _positionsRepository.GetAllPositionsAsync();
            var gecResponse = await _gecMemberService.GetGECMemberByIdAsync(id);
            if (gecResponse == null || !gecResponse.IsSuccess)
                return NotFound("GEC Member not found");

            var dto = new CreateGECMemberDto
            {
                Members = members.Data,
                Positions = positions.Data?.Where(p => p.IsActive).ToList() ?? new List<GECPosition>(),
                GECMember = new GECMemberDto
                {
                    GECId = gecResponse.Data.GECId,
                    MemberId = gecResponse.Data.MemberId,
                    GECPositionId = gecResponse.Data.GECPositionId,
                    Bio = gecResponse.Data.Bio,
                    StartDate = gecResponse.Data.StartDate,
                    EndDate = gecResponse.Data.EndDate,
                    IsActive = gecResponse.Data.IsActive
                }
            };

            ViewBag.IsEdit = true;
            if (gecResponse.Data.Member != null)
            {
                if (!string.IsNullOrEmpty(gecResponse.Data.Member.ProfilePictureUrl))
                {
                    ViewBag.CurrentImageUrl = gecResponse.Data.Member.ProfilePictureUrl;
                }
                else if (gecResponse.Data.Member.ProfileImage != null && gecResponse.Data.Member.ProfileImage.Length > 0)
                {
                    ViewBag.CurrentImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(gecResponse.Data.Member.ProfileImage);
                }
            }
            return PartialView("_CreateGECMemberPartial", dto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _gecMemberService.DeleteGECMemberAsync(id);
            if (!result.IsSuccess)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, message = "GEC member deleted successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var result = await _gecMemberService.ToggleGECMemberStatusAsync(id, isActive);
            if (!result.IsSuccess)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, message = result.Message });
        }

        //add get by id
        public async Task<IActionResult> Details(int id)
        {
            GECMemberDetailsViewModel member=new GECMemberDetailsViewModel();
            var response = await _gecMemberService.GetGECMemberByIdAsync(id);

            if (response == null)
            {
                TempData["ErrorMessage"] = "GEC member not found.";
                return RedirectToAction(nameof(Index));
            }
            member.GECMember = response.Data;

            return View(member);
        }
    }
}
