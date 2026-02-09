namespace GCI_Admin.Models
{
    public class Member
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string OtherNames { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string Gender { get; set; }

        public string Assembly { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime CreatedAt { get; set; }
        public MemberStatus Status { get; set; }

    }

}
