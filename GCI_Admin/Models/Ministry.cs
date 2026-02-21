using System;
using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models
{
    public class Ministry
    {
        [Key]
        public int MinistryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string MinistryName { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}