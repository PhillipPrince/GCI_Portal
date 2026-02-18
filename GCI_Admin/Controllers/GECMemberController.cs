using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GCI_Admin.Controllers
{
    public class GECMemberController : Controller
    {
        private readonly IGECMemberService _gecMemberService;
        private readonly AppDbContext _context;
        private readonly MembersRepository _membersRepository;


        public GECMemberController(IGECMemberService gecMemberService, AppDbContext context,MembersRepository repository)
        {
            _gecMemberService = gecMemberService;
            _context = context;
                _membersRepository = repository;
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

            dto.Members   = members.Data;


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

        // ✅ GET BY ID (For Edit)
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _gecMemberService.GetGECMemberByIdAsync(id);

            if (!response.IsSuccess)
                return NotFound(response.Message);

            return Json(response.Data);
        }

        public async Task<IActionResult> LoadCreateForm()
        {
            CreateGECMemberDto dto = new CreateGECMemberDto();
            var members = await _membersRepository.GetAllMembersAsync();
            dto.Members = members.Data;

            ViewBag.IsEdit = false;
            return PartialView("_CreateGECMemberPartial", dto);
        }

        //// GET: Load edit form in modal
        //public async Task<IActionResult> LoadEditForm(int id)
        //{
        //    var gecMember = await _gecMemberRepository.GetByIdAsync(id);
        //    if (gecMember == null)
        //    {
        //        return NotFound();
        //    }

        //    // Map to DTO
        //    CreateGECMemberDto dto = new CreateGECMemberDto
        //    {
        //        GECId = gecMember.GECId,
        //        MemberId = gecMember.MemberId,
        //        PositionTitle = gecMember.PositionTitle,
        //        Bio = gecMember.Bio,
        //        StartDate = gecMember.StartDate,
        //        EndDate = gecMember.EndDate,
        //        IsActive = gecMember.IsActive
        //    };

        //    var members = await _membersRepository.GetAllMembersAsync();
        //    dto.Members = members.Data;

        //    ViewBag.IsEdit = true;
        //    return PartialView("_CreateGECMemberPartial", dto);
        //}

        //// POST: Create new GEC member
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateGECMember(CreateGECMemberDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "Validation failed",
        //            errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
        //        });
        //    }

        //    try
        //    {
        //        var result = await _gecMemberService.CreateGECMember(dto);
        //        if (result.Success)
        //        {
        //            return Json(new
        //            {
        //                success = true,
        //                message = "GEC member created successfully",
        //                data = result.Data
        //            });
        //        }

        //        return Json(new { success = false, message = result.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "An error occurred while creating the member" });
        //    }
        //}

        //// POST: Update GEC member
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateGECMember(CreateGECMemberDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "Validation failed",
        //            errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
        //        });
        //    }

        //    try
        //    {
        //        var result = await _gecMemberService.UpdateGECMember(dto);
        //        if (result.Success)
        //        {
        //            return Json(new
        //            {
        //                success = true,
        //                message = "GEC member updated successfully",
        //                data = result.Data
        //            });
        //        }

        //        return Json(new { success = false, message = result.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "An error occurred while updating the member" });
        //    }
        //}
    }
}
