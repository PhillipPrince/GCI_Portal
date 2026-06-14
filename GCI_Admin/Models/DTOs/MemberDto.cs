namespace GCI_Admin.Models.DTOs
{
    public class MemberDto
    {
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Assembly { get; set; }
        public int? StatusId { get; set; }
        public string? SocialMediaName { get; set; }
        public string? ResidentialAddress { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? MaritalStatus { get; set; }
        public int? NumberOfChildren { get; set; }
        public string? SpouseName { get; set; }
    }
    public class MemberUploadResponse
    {
        public int TotalRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public string FailedRecordsFileBase64 { get; set; }
        public string FailedRecordsFileName { get; set; }
    }
    public class ExcelMemberDto
    {
        public int RowNumber { get; set; }
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string MaritalStatus { get; set; }
        public string SpouseName { get; set; }
        public string NumberOfChildren { get; set; }
        public string Assembly { get; set; }
        public string ResidentialAddress { get; set; }
        public string SocialMediaName { get; set; }
    }

    public class FailedMemberRecord
    {
        public int RowNumber { get; set; }
        public string ErrorMessage { get; set; }

        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string MaritalStatus { get; set; }
        public string SpouseName { get; set; }
        public string NumberOfChildren { get; set; }
        public string Assembly { get; set; }
        public string ResidentialAddress { get; set; }
        public string SocialMediaName { get; set; }
    }
}
