using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models.DTOs
{
    public class MinistryDto
    {
        [Required(ErrorMessage = "Ministry name is required")]
        [StringLength(150, ErrorMessage = "Ministry name cannot exceed 150 characters")]
        public string MinistryName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
    }
    public class CreateMinistryLeader
    {
        public List<DropdownItem> MembersList { get; set; }
    }

    public class MembersMinistriesViewModel
    {
        public List<SelectListItem> MembersList { get; set; }
        public List<SelectListItem> MinistriesList { get; set; }
        public int SelectedMemberId { get; set; }
        public int SelectedMinistryId { get; set; }
        public string PositionTitle { get; set; }
        public string Bio { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }


    public class CreateMinistryLeaderDto
    {
        public List<SelectListItem> Members { get; set; }
        public List<SelectListItem> Ministries { get; set; }
        public MinistryLeaderDto MinistryLeader { get; set; }
    }

   
    public class MinistryLeaderDto
    {
        public int MinistryLeaderId { get; set; }
        public int MemberId { get; set; }
        public int MinistryId { get; set; }
        public string PositionTitle { get; set; }
        public string Bio { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }

        public List<SelectListItem> MembersList { get; set; }
        public List<SelectListItem> MinistriesList { get; set; }
    }
}