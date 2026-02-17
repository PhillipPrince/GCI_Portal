using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.Services.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

public class MembersController : Controller
{
    private readonly IMembersService _membersService;
    public MembersController(IMembersService membersService)
    {
        _membersService = membersService;
    }




    public async Task<IActionResult> Index(string search)
    {
        var members = await GetMembers();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            members = members.Where(x =>
                (!string.IsNullOrEmpty(x.FirstName) && x.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(x.OtherNames) && x.OtherNames.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(x.Phone) && x.Phone.Contains(search)) ||
                (!string.IsNullOrEmpty(x.Email) && x.Email.Contains(search, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        var result = members
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return View(result);
    }


    private async Task<List<Member>> GetMembers()
    {
        var response = await _membersService.GetAllMembersAsync();

        if (response == null || response.Data == null)
            return new List<Member>();

        return response.Data
                       .OrderBy(m => m.Id)
                       .ToList();
    }



    //    // VIEW PROFILE
    //    public IActionResult Profile(int id)
    //    {
    //        var member = Members.FirstOrDefault(x => x.Id == id);
    //        return PartialView("_Profile", member);
    //    }

    // CREATE
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
    public IActionResult AddMembershipClassPartial(int memberId)
    {
        var model = new MembershipClassDto
        {
            MemberId = memberId,
            MembershipYear = DateTime.Now.Year.ToString()
        };

        return PartialView("_AddMembershipClass", model);
    }


    [HttpPost]
    public async Task<IActionResult> CreateMembershipClass([FromBody] MembershipClassDto dto)
    {
        var result = await _membersService.CreateMembershipClassAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }


    //    // EDIT
    //    public IActionResult Edit(int id)
    //    {
    //        var member = Members.FirstOrDefault(x => x.Id == id);
    //        return PartialView("_CreateEdit", member);
    //    }

    //    [HttpPost]
    //    public IActionResult Edit(Member model)
    //    {
    //        var member = Members.FirstOrDefault(x => x.Id == model.Id);

    //        if (member != null)
    //        {
    //            member.FirstName = model.FirstName;
    //            member.OtherNames = model.OtherNames;
    //            member.Phone = model.Phone;
    //            member.Email = model.Email;
    //            member.Gender = model.Gender;
    //            member.Assembly = model.Assembly;
    //\        }

    //        return Json(new { success = true });
    //    }

    //    // DELETE
    //    [HttpPost]
    //    public IActionResult Delete(int id)
    //    {
    //        var member = Members.FirstOrDefault(x => x.Id == id);
    //        if (member != null)
    //            Members.Remove(member);

    //        return Json(new { success = true });
    //    }
}
