using GCI_Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

public class MembersController : Controller
{
    private static List<Member> Members = new()
{
    new Member { Id = 1, FirstName = "Phillip", OtherNames = "Simiyu", Phone = "0700413239", Email = "philllipsimiyu38@gmail.com", Gender = "Male", IsAdmin = true, Assembly = "Nairobi Central", CreatedAt = DateTime.Now.AddDays(-10), Status = MemberStatus.FullMember },
    new Member { Id = 2, FirstName = "Grace", OtherNames = "Wanjiku", Phone = "0712345678", Email = "grace@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Westlands", CreatedAt = DateTime.Now.AddDays(-5), Status = MemberStatus.MembershipClass },
    new Member { Id = 3, FirstName = "David", OtherNames = "Kamau", Phone = "0723456789", Email = "david@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Nairobi Central", CreatedAt = DateTime.Now.AddDays(-15), Status = MemberStatus.NonMember },
    new Member { Id = 4, FirstName = "Mary", OtherNames = "Achieng", Phone = "0734567890", Email = "mary@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Kilimani", CreatedAt = DateTime.Now.AddDays(-8), Status = MemberStatus.FullMember },
    new Member { Id = 5, FirstName = "James", OtherNames = "Mutua", Phone = "0745678901", Email = "james@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Westlands", CreatedAt = DateTime.Now.AddDays(-12), Status = MemberStatus.MembershipClass },
    new Member { Id = 6, FirstName = "Ruth", OtherNames = "Njeri", Phone = "0756789012", Email = "ruth@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Nairobi Central", CreatedAt = DateTime.Now.AddDays(-3), Status = MemberStatus.NonMember },
    new Member { Id = 7, FirstName = "Peter", OtherNames = "Otieno", Phone = "0767890123", Email = "peter@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Kilimani", CreatedAt = DateTime.Now.AddDays(-20), Status = MemberStatus.FullMember },
    new Member { Id = 8, FirstName = "Faith", OtherNames = "Chebet", Phone = "0778901234", Email = "faith@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Westlands", CreatedAt = DateTime.Now.AddDays(-7), Status = MemberStatus.MembershipClass },
    new Member { Id = 9, FirstName = "Michael", OtherNames = "Muriuki", Phone = "0789012345", Email = "michael@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Nairobi Central", CreatedAt = DateTime.Now.AddDays(-11), Status = MemberStatus.NonMember },
    new Member { Id = 10, FirstName = "Joy", OtherNames = "Wanjiru", Phone = "0790123456", Email = "joy@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Kilimani", CreatedAt = DateTime.Now.AddDays(-6), Status = MemberStatus.FullMember },
    new Member { Id = 11, FirstName = "Brian", OtherNames = "Munyua", Phone = "0701234567", Email = "brian@gmail.com", Gender = "Male", IsAdmin = true, Assembly = "Westlands", CreatedAt = DateTime.Now.AddDays(-2), Status = MemberStatus.FullMember },
    new Member { Id = 12, FirstName = "Linda", OtherNames = "Njoki", Phone = "0712345670", Email = "linda@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Nairobi Central", CreatedAt = DateTime.Now.AddDays(-18), Status = MemberStatus.MembershipClass },
    new Member { Id = 13, FirstName = "Anthony", OtherNames = "Mwangi", Phone = "0723456701", Email = "anthony@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Kilimani", CreatedAt = DateTime.Now.AddDays(-9), Status = MemberStatus.NonMember },
    new Member { Id = 14, FirstName = "Naomi", OtherNames = "Wairimu", Phone = "0734567012", Email = "naomi@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Westlands", CreatedAt = DateTime.Now.AddDays(-4), Status = MemberStatus.FullMember },
    new Member { Id = 15, FirstName = "Daniel", OtherNames = "Kariuki", Phone = "0745670123", Email = "daniel@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Nairobi Central", CreatedAt = DateTime.Now.AddDays(-1), Status = MemberStatus.MembershipClass },
    new Member { Id = 16, FirstName = "Sarah", OtherNames = "Akinyi", Phone = "0756701234", Email = "sarah@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Kilimani", CreatedAt = DateTime.Now.AddDays(-14), Status = MemberStatus.NonMember },
    new Member { Id = 17, FirstName = "Samuel", OtherNames = "Maina", Phone = "0767012345", Email = "samuel@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Westlands", CreatedAt = DateTime.Now.AddDays(-13), Status = MemberStatus.FullMember },
    new Member { Id = 18, FirstName = "Esther", OtherNames = "Wambui", Phone = "0770123456", Email = "esther@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Nairobi Central", CreatedAt = DateTime.Now.AddDays(-16), Status = MemberStatus.MembershipClass },
    new Member { Id = 19, FirstName = "Kevin", OtherNames = "Ndungu", Phone = "0781234567", Email = "kevin@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Kilimani", CreatedAt = DateTime.Now.AddDays(-17), Status = MemberStatus.NonMember },
    new Member { Id = 20, FirstName = "Anne", OtherNames = "Mwikali", Phone = "0792345678", Email = "anne@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Westlands", CreatedAt = DateTime.Now.AddDays(-19), Status = MemberStatus.FullMember },
    new Member { Id = 21, FirstName = "Joseph", OtherNames = "Mutiso", Phone = "0703456789", Email = "joseph@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Nairobi Central", CreatedAt = DateTime.Now.AddDays(-20), Status = MemberStatus.MembershipClass },
    new Member { Id = 22, FirstName = "Faith", OtherNames = "Nyambura", Phone = "0714567890", Email = "faith2@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Kilimani", CreatedAt = DateTime.Now.AddDays(-21), Status = MemberStatus.NonMember },
    new Member { Id = 23, FirstName = "Mark", OtherNames = "Onyango", Phone = "0725678901", Email = "mark@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Westlands", CreatedAt = DateTime.Now.AddDays(-22), Status = MemberStatus.FullMember },
    new Member { Id = 24, FirstName = "Joyce", OtherNames = "Nyokabi", Phone = "0736789012", Email = "joyce@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Nairobi Central", CreatedAt = DateTime.Now.AddDays(-23), Status = MemberStatus.MembershipClass },
    new Member { Id = 25, FirstName = "David", OtherNames = "Ouma", Phone = "0747890123", Email = "david2@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Kilimani", CreatedAt = DateTime.Now.AddDays(-24), Status = MemberStatus.NonMember },
    new Member { Id = 26, FirstName = "Ruth", OtherNames = "Chepkemoi", Phone = "0758901234", Email = "ruth2@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Westlands", CreatedAt = DateTime.Now.AddDays(-25), Status = MemberStatus.FullMember },
    new Member { Id = 27, FirstName = "Brian", OtherNames = "Kibet", Phone = "0769012345", Email = "brian2@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Nairobi Central", CreatedAt = DateTime.Now.AddDays(-26), Status = MemberStatus.MembershipClass },
    new Member { Id = 28, FirstName = "Mercy", OtherNames = "Wanjiru", Phone = "0770123456", Email = "mercy@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Kilimani", CreatedAt = DateTime.Now.AddDays(-27), Status = MemberStatus.NonMember },
    new Member { Id = 29, FirstName = "John", OtherNames = "Karanja", Phone = "0781234560", Email = "john@gmail.com", Gender = "Male", IsAdmin = false, Assembly = "Westlands", CreatedAt = DateTime.Now.AddDays(-28), Status = MemberStatus.FullMember },
    new Member { Id = 30, FirstName = "Linda", OtherNames = "Naliaka", Phone = "0792345601", Email = "linda2@gmail.com", Gender = "Female", IsAdmin = false, Assembly = "Nairobi Central", CreatedAt = DateTime.Now.AddDays(-29), Status = MemberStatus.MembershipClass }
};


    public IActionResult Index(string search)
    {
        var query = Members.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x =>
                x.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                x.OtherNames.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                x.Phone.Contains(search) ||
                x.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        return View(query.OrderByDescending(x => x.CreatedAt).ToList());
    }

    // VIEW PROFILE
    public IActionResult Profile(int id)
    {
        var member = Members.FirstOrDefault(x => x.Id == id);
        return PartialView("_Profile", member);
    }

    // CREATE
    public IActionResult Create()
    {
        return PartialView("_CreateEdit", new Member());
    }

    [HttpPost]
    public IActionResult Create(Member model)
    {
        model.Id = Members.Max(x => x.Id) + 1;
        model.CreatedAt = DateTime.Now;
        Members.Add(model);

        return Json(new { success = true });
    }

    // EDIT
    public IActionResult Edit(int id)
    {
        var member = Members.FirstOrDefault(x => x.Id == id);
        return PartialView("_CreateEdit", member);
    }

    [HttpPost]
    public IActionResult Edit(Member model)
    {
        var member = Members.FirstOrDefault(x => x.Id == model.Id);

        if (member != null)
        {
            member.FirstName = model.FirstName;
            member.OtherNames = model.OtherNames;
            member.Phone = model.Phone;
            member.Email = model.Email;
            member.Gender = model.Gender;
            member.Assembly = model.Assembly;
            member.IsAdmin = model.IsAdmin;
        }

        return Json(new { success = true });
    }

    // DELETE
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var member = Members.FirstOrDefault(x => x.Id == id);
        if (member != null)
            Members.Remove(member);

        return Json(new { success = true });
    }
}
