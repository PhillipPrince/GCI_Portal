using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    public class MonthlyTheme
    {
        [Key]
        public int ThemeId { get; set; }

        [Required]
        [StringLength(255)]
        public string Theme { get; set; }

        public string Description { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(150)]
        public string? Assembly { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public byte[]? MonthThemeImage { get; set; }
    }
}
