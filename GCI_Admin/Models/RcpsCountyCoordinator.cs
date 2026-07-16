using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    public class RcpsCountyCoordinator
    {
        [Key]
        public int RcpsCountyCoordinatorId { get; set; }

        public int MemberId { get; set; }

        public int RcpsId { get; set; }

        public string? Bio { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("MemberId")]
        public virtual Member? Member { get; set; }

        [ForeignKey("RcpsId")]
        public virtual Rcps? Rcps { get; set; }
    }
}
