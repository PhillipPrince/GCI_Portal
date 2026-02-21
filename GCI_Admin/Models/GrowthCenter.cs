using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models
{
    public class GrowthCenter
    {
        [Key]
        public int GrowthCenterId { get; set; }

        [Required]
        [StringLength(150)]
        public string CenterName { get; set; }

        [StringLength(150)]
        public string Location { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

    }
}