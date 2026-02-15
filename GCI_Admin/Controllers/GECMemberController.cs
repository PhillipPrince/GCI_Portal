using GCI_Admin.DBOperations;
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


        public GECMemberController(IGECMemberService gecMemberService, AppDbContext context)
        {
            _gecMemberService = gecMemberService;
            _context = context;
        }

        // ✅ INDEX
        public async Task<IActionResult> Index()
        {
            var response = await _gecMemberService.GetGECMembersAsync();
            return View(response.Data);
        }

        // ✅ PARTIAL TABLE REFRESH
        public async Task<IActionResult> GetMembersTable()
        {
            var response = await _gecMemberService.GetGECMembersAsync();
            return PartialView("_GECMembersTable", response.Data);
        }

        public async Task<IActionResult> AddNewGecMember()
        {
            // Get all members for the dropdown
            var members = await _context.Members
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.OtherNames)
                .ToListAsync();

            // Example positions (can be fetched from DB if dynamic)
            var positions = new List<string>
    {
        "Chairperson",
        "Secretary",
        "Treasurer",
        "Member"
    };

            ViewBag.Members = members;
            ViewBag.Positions = positions;

            return View();
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

        // ✅ UPDATE
        [HttpPost]
        public async Task<IActionResult> Update(int id, GECMemberDto dto)
        {
            var response = await _gecMemberService.UpdateGECMemberAsync(id, dto);

            if (!response.IsSuccess)
                return BadRequest(response.Message);

            return Json(response);
        }

        // ✅ DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _gecMemberService.DeleteGECMemberAsync(id);

            if (!response.IsSuccess)
                return NotFound(response.Message);

            return Json(response);
        }
    }
}
