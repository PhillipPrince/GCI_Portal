namespace GCI_Admin.Models.DTOs
{
    public class MemberDto
    {
        public string FirstName { get; set; }
        public string OtherNames { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string Gender { get; set; }

        public string PasswordHash { get; set; }

        public string Assembly { get; set; }

        public int? StatusId { get; set; }
    }
}
