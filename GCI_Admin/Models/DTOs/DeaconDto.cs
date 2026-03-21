namespace GCI_Admin.Models.DTOs
{
    public class DeaconDto
    {
        public int MemberId { get; set; }
        public string MinistryId { get; set; }
        public string Bio { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool OnDuty { get; set; }
    }
}
