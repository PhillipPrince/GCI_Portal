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
}