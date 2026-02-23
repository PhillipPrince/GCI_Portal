using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    public class AnnualTheme
    {
        [Key]
        public int ThemeId { get; set; }

        [Required]
        [StringLength(255)]
        public string Theme { get; set; }

        [Required]
        [StringLength(255)]
        public string Verse { get; set; }

        public string Description { get; set; }

        [Required]
        public int Year { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
        [NotMapped]
        public byte[]? YearThemeImage { get; set; }
    }
}
