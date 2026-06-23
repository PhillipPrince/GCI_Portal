using DocumentFormat.OpenXml.Spreadsheet;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.Services.Service;
using Member = GCI_Admin.Models.Member;
using Microsoft.AspNetCore.Mvc;
using Utils;

[SessionAuthorize]

public class MembersController : Controller
{
    private readonly IMembersService _membersService;
    private readonly MembersRepository _memberRepository;
    private readonly IRolesService _rolesService;

    public MembersController(IMembersService membersService, MembersRepository memberRepository, IRolesService rolesService)
    {
        _membersService = membersService;
        _memberRepository = memberRepository;
        _rolesService = rolesService;
    }

    // =========================================================
    // ✅ MEMBERS
    // =========================================================

    public async Task<IActionResult> Index()
    {
        MembersListViewModel membersListViewModel = new MembersListViewModel();
        if (membersListViewModel.MemberStatus == null)
        {
            membersListViewModel.MemberStatus = new MemberStatusModel();
        }

        // Just get the counts to avoid heavy data loading
        // We'll mock the counts or you can implement GetMemberCountsAsync
        var allMembers = await _membersService.GetAllMembersAsync();
        var members = allMembers?.Data ?? new List<Member>();

        membersListViewModel.MemberStatus.AllMembers = new List<Member>();
        membersListViewModel.MemberStatus.MembershipClassMembers = new List<Member>();
        membersListViewModel.MemberStatus.ActiveMembers = new List<Member>();
        membersListViewModel.MemberStatus.InactiveMembers = new List<Member>();
        membersListViewModel.MemberStatus.AwaitingConfirmationMembers = new List<Member>();
        membersListViewModel.MemberStatus.TransferredMembers = new List<Member>();
        membersListViewModel.MemberStatus.PromotedToGlory = new List<Member>();
        membersListViewModel.MemberStatus.WithdrawnMembers = new List<Member>();
        membersListViewModel.MemberStatus.NonMembers = new List<Member>();
        
        // We still assign counts here if you want to keep the view badges
        ViewBag.TotalMembers = members.Count;
        ViewBag.ActiveMembersCount = members.Count(x => x.StatusId == 1);
        ViewBag.MembershipClassCount = members.Count(x => x.StatusId == 2);
        ViewBag.AwaitingCount = members.Count(x => x.StatusId == 4);
        ViewBag.InactiveCount = members.Count(x => x.StatusId == 3);
        ViewBag.TransferredCount = members.Count(x => x.StatusId == 7);
        ViewBag.PromotedCount = members.Count(x => x.StatusId == 5);
        ViewBag.WithdrawnCount = members.Count(x => x.StatusId == 6);

        membersListViewModel.TotalMembers = members.Count;

        return View(membersListViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> GetMembersDataTable([FromForm] int draw, [FromForm] int start, [FromForm] int length, [FromForm] int? statusId)
    {
        string searchValue = Request.Form["search[value]"].FirstOrDefault();

        var response = await _membersService.GetMembersDataTableAsync(draw, start, length, searchValue, statusId);

        if (response.IsSuccess && response.Data != null)
        {
            return Json(response.Data);
        }

        return Json(new DataTableResponse<Member>
        {
            draw = draw,
            recordsTotal = 0,
            recordsFiltered = 0,
            data = new List<Member>(),
            error = response.Message
        });
    }

    

    public async Task<IActionResult> MemberDetails(int memberId)
    {
        GCI_Admin.Models.Member member = new GCI_Admin.Models.Member();
        if (memberId <= 0)
            return BadRequest("Invalid member ID");

        var memberResponse = await _memberRepository.GetMemberByIdAsync(memberId);

        if (!memberResponse.Success || memberResponse.Data == null)
            return NotFound("Member not found");
        member = memberResponse.Data;

        var additionalInfoResponse = await _memberRepository.GetAdditionalInfoByMemberIdAsync(member.Id);
        var userRole = await _rolesService.GetRoleByIdAsync(member.UserRole);
        member.RoleName = userRole.Data != null ? userRole.Data.RoleName : "Unknown Role";
        var rolesResponse = await _rolesService.GetAllRolesAsync();
        var roles = rolesResponse?.Data ?? new List<Role>();

        List<DropdownItem> userRoles = roles
            .Select(r => new DropdownItem
            {
                Value = r.RoleId.ToString(),
                Text = r.RoleName
            })
            .ToList();
        var model = new MemberDetailsViewModel
        {
            Member = member,
            UserRoles = userRoles,
            AdditionalInformation = additionalInfoResponse.Data 
        };

        return View(model);
    }

    public IActionResult AddMemberPartial()
    {
        return PartialView("_AddMember");
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] MemberDto dto)
    {
        var result = await _membersService.CreateUserAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

   
    

    // CREATE
    [HttpPost]
    public async Task<IActionResult> CreateAdditionalInfo([FromBody] MemberAdditionalInformationDto dto)
    {
        var result = await _membersService.CreateAdditionalInfoAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    // GET BY MEMBER
    [HttpGet]
    public async Task<IActionResult> GetAdditionalInfo(int memberId)
    {
        var result = await _membersService.GetAdditionalInfoByMemberIdAsync(memberId);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    // UPDATE
    [HttpPut]
    public async Task<IActionResult> UpdateAdditionalInfo(int id, [FromBody] MemberAdditionalInformationDto dto)
    {
        var result = await _membersService.UpdateAdditionalInfoAsync(id, dto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("UpdateUserRole")]
    public async Task<IActionResult> UpdateUserRole(int memberId, int roleId)
    {
        try
        {
            if (memberId <= 0 || roleId <= 0)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "400",
                    Message = "Invalid memberId or roleId"
                });
            }

            var response = await _membersService.UpdateMemberRoleAsync(memberId, roleId);

            if (!response.IsSuccess)
            {
                return StatusCode(int.Parse(response.Code ?? "500"), response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>
            {
                IsSuccess = false,
                Code = "500",
                Message = ex.Message
            });
        }


    }
    [HttpPost]
    public async Task<IActionResult> UploadMembersExcel(IFormFile file, string uploadOption)
    {
        try
        {
            if (file == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Code = "400",
                    Message = "Please select a file to upload."
                });
            }

            string createdBy = User?.Identity?.Name ?? "System";

            var response = await _membersService.ProcessMembersExcelUploadAsync(file, createdBy, uploadOption);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                IsSuccess = false,
                Code = "500",
                Message = ex.Message
            });
        }
    }

    //add method to UpdateFullMembershipStatus for a member
    [HttpPost]
    public async Task<IActionResult> UpdateFullMembershipStatus(int memberId)
    {
        try
        {
            if (memberId <= 0)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "400",
                    Message = "Invalid memberId"
                });
            }
            var response = await _membersService.UpdateFullMembershipStatusAsync(memberId);
            if (!response.IsSuccess)
            {
                return StatusCode(int.Parse(response.Code ?? "500"), new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = response.Code,
                    Message = response.Message
                });
            }
            return Ok(new ApiResponse<bool>
            {
                IsSuccess = true,
                Code = "200",
                Message = "Membership status updated successfully",
                Data = response.Data
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>
            {
                IsSuccess = false,
                Code = "500",
                Message = ex.Message
            });
        }
    }


}