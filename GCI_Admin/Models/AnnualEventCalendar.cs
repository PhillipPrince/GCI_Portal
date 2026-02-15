using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("AnnualEventCalendar")]
    public class AnnualEventCalendar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CalendarEventId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Column("EventStartDate")]
        public DateTime EventStartDate { get; set; }

        [Column("EventEndDate")]
        public DateTime EventEndDate { get; set; }

        public int Year { get; set; }

        [StringLength(255)]
        public string Location { get; set; }

        public bool IsActive { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedAt { get; set; }
    }
}
