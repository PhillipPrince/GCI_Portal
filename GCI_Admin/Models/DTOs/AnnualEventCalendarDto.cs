namespace GCI_Admin.Models.DTOs
{
    public class AnnualEventCalendarDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public int Year { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
    }
}
