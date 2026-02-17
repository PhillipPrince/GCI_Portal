namespace GCI_Admin.Models.DTOs
{
    public class MemberDto
    {
        public string FirstName { get; set; }
        public string OtherNames { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string Gender { get; set; }

        //public string PasswordHash { get; set; }

        public string Assembly { get; set; }

        public int? StatusId { get; set; }
        public string? SocialMediaName { get; set; }

        public string? ResidentialAddress { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? MaritalStatus { get; set; }

        public int? NumberOfChildren { get; set; }

        public string? SpouseName { get; set; }
    }
}
