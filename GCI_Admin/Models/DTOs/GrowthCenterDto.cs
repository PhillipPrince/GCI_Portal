using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models.DTOs
{
    public class GrowthCenterDto
    {
        [Required(ErrorMessage = "Center Name is required")]
        [StringLength(150, ErrorMessage = "Center Name cannot exceed 150 characters")]
        public string CenterName { get; set; }

        [StringLength(250, ErrorMessage = "Location cannot exceed 250 characters")]
        public string Location { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
    }

    public class CreateGCLeaderDto
    {
        public List<SelectListItem> Members { get; set; }
        public List<SelectListItem> GrowthCenters { get; set; }
        public GCLeaderDto GCLeader { get; set; }
    }

    public class GCLeaderDto
    {
        public int GCLeaderId { get; set; }
        public int MemberId { get; set; }
        public int GrowthCenterId { get; set; }
        public string PositionTitle { get; set; }
        public string Bio { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}