using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("Assemblies")]
    public class Assembly
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(20)]
        [Phone]
        public string? ContactPhone { get; set; }

        [StringLength(150)]
        [EmailAddress]
        public string? ContactEmail { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
