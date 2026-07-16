using System;
using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models.DTOs
{
    public class ChurchDailyActivityDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Day of week is required")]
        public string DayOfWeek { get; set; }
        
        [Required(ErrorMessage = "Activity name is required")]
        public string ActivityName { get; set; }
        
        public string Description { get; set; }
        
        public TimeSpan? StartTime { get; set; }
        
        public TimeSpan? EndTime { get; set; }
        
        public bool IsActive { get; set; }
    }
}
