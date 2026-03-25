using System;
using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models.DTOs
{
    public class AnnualThemeDto
    {
        [Required]
        public string Theme { get; set; }

        [Required]
        public string Verse { get; set; }

        public string Description { get; set; }

        [Required]
        public int Year { get; set; }

        public bool IsActive { get; set; }
    }
}