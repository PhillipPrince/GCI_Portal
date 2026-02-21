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
}