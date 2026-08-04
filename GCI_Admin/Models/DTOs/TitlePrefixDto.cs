using System;

namespace GCI_Admin.Models.DTOs
{
    public class TitlePrefixDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
