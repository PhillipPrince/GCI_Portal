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


        public GECMemberController(IGECMemberService gecMemberService, AppDbContext context, MembersRepository repository)
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

            dto.Members = members.Data;


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

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGECMember([FromForm] GECMemberDto dto)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGECMember([FromForm] GECMemberDto dto)
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
    }
}