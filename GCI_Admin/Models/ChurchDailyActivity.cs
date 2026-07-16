using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("ChurchDailyActivities")]
    public class ChurchDailyActivity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string DayOfWeek { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string ActivityName { get; set; }
        
        public string Description { get; set; }
        
        public TimeSpan? StartTime { get; set; }
        
        public TimeSpan? EndTime { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}
