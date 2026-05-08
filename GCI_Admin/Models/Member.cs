using System.ComponentModel.DataAnnotations.Schema;

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

        public string PasswordHash { get; set; }

        public string? Assembly { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? StatusId { get; set; }
        public string? SocialMediaName { get; set; }

        public string? ResidentialAddress { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? MaritalStatus { get; set; }

        public int? NumberOfChildren { get; set; }

        public string? SpouseName { get; set; }
        public int UserRole { get; set; }
        [NotMapped]
        public string RoleName { get; set; }
        public bool MustChangePassword { get; set; }
        [NotMapped]
        public bool UseOtp { get; set; }
        [NotMapped]
        public string Token { get; set; } = string.Empty;
        [NotMapped]
        public byte[] ProfileImage { get; set; }

    }

    public class MembersListViewModel
    {
        public int TotalMembers { get; set; }

        public MemberStatusModel MemberStatus { get; set; }

    }

    public class MemberDetailsViewModel
    {
        public Member Member { get; set; }
        public MemberAdditionalInformation AdditionalInformation { get; set; }
        public List<DropdownItem> UserRoles { get; set; }
    }

    public class MemberStatusModel {
        public List<Member> AllMembers { get; set; }
        public List<Member> MembershipClassMembers { get; set; }
        public List<Member> ActiveMembers { get; set; }
        public List<Member> InactiveMembers { get; set; }
        public List<Member> TransferredMembers { get; set; }
        public List<Member> PromotedToGlory { get; set; }
        public List<Member> WithdrawnMembers { get; set; }

        public List<Member> NonMembers { get; set; }
    }
}
