namespace GCI_Admin.Models
{
    public class EventUploadResponse
    {
        public int TotalRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }

        public List<string> ErrorMessages { get; set; } = new();
        public List<Event> CreatedEvents { get; set; } = new();
    }


    public class ExcelEventDto
    {
        public int RowNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string EventDate { get; set; }
        public string Location { get; set; }
        public string IsPaid { get; set; }
        public string Price { get; set; }
    }

}
