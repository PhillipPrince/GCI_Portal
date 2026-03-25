namespace GCI_Admin.Models
{
    public class Elder
    {
        public int ElderId { get; set; }
        public int MemberId { get; set; }
        public string Description { get; set; }
        public DateTime? DateOrdained { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Member Member { get; set; }
    }
}
