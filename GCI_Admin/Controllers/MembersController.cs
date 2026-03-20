using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.Services.Service;
using Microsoft.AspNetCore.Mvc;
using Utils;

public class MembersController : Controller
{
    private readonly IMembersService _membersService;
    private readonly MembersRepository _memberRepository;

    public MembersController(IMembersService membersService, MembersRepository memberRepository)
    {
        _membersService = membersService;
        _memberRepository = memberRepository;
    }

    // =========================================================
    // ✅ MEMBERS
    // =========================================================

    public async Task<IActionResult> index()
    {

        MembersListViewModel membersListViewModel = new MembersListViewModel();

        var allMembers = await _membersService.GetAllMembersAsync();


        membersListViewModel.ActiveMembers = allMembers.Data.Where(m => m.StatusId == 1).ToList();
        membersListViewModel.MembershipClassMembers = allMembers.Data.Where(m => m.StatusId == 2).ToList();
        membersListViewModel.NonMembers = allMembers.Data.Where(m => m.StatusId == 3).ToList();
        membersListViewModel.TotalMembers = allMembers.Data.Count;

        

        return View(membersListViewModel);
    }

    

    public async Task<IActionResult> MemberDetails(int memberId)
    {
        if (memberId <= 0)
            return BadRequest("Invalid member ID");

        var memberResponse = await _memberRepository.GetMemberByIdAsync(memberId);

        if (!memberResponse.Success || memberResponse.Data == null)
            return NotFound("Member not found");

        var additionalInfoResponse = await _memberRepository.GetAdditionalInfoByMemberIdAsync(memberId);

        var model = new MemberDetailsViewModel
        {
            Member = memberResponse.Data,
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

   
}