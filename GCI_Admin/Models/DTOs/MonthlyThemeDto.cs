using System;
using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models.DTOs
{
    public class MonthlyThemeDto
    {
        public int Id { get; set; }

        [Required]
        public string Theme { get; set; }

        public string Description { get; set; }

        public string? Assembly { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        public bool IsActive { get; set; }

        public string ThemeImage { get; set; }
    }
}
