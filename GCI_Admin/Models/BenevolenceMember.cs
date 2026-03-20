using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{

    public class BenevolenceMember
    {
        [Key]
        public int Id { get; set; }
    //    public string MemberId { get; set; }
        public decimal PreferredCoverAmount { get; set; }
        public string NationalId { get; set; }
        public string NextOfKinName { get; set; }
        public string NextOfKinPhone { get; set; }
        public int NumberOfDependants { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string RegNo { get; set; }
        public decimal TotalAmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceAmount { get; set; }

        public Member Member { get; set; }
    }

    public class BenevolenceData
    {
        public List<BenevolenceMember> BenevolenceMembers { get; set; }
        public int TotalMembers { get; set; }
        public int TotalActiveMembers { get; set; }
        public decimal TotalAmountDue { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public decimal TotalBalance { get; set; }
    }
    public class BenevolenceBeneficiary
    {
        public int Id { get; set; }
        public int BenevolenceMemberId { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? NationalId { get; set; }
        public string Gender { get; set; }
        public string Relationship { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class BenevolenceDetails
    {
        public BenevolenceMember Member { get; set; }
        public List<BenevolenceBeneficiary> Beneficiaries { get; set; }
        public int TotalBeneficiaries { get; set; }
    }
}
