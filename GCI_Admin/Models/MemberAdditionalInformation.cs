using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    public class MemberAdditionalInformation
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
       public int? MembershipYear { get; set; }
        public string Cohort { get; set; }
        public bool IsMemberOfAnotherChurch { get; set; }
        public string FormerChurchName { get; set; }
        public string ReasonForLeavingFormerChurch { get; set; }
        public DateTime? DateBeganAttendingGCI { get; set; }
        public bool SeekingMembership { get; set; }
        public bool IsBornAgain { get; set; }
        public DateTime? DateOfConversion { get; set; }
        public string PlaceOfConversion { get; set; }
        public bool HasEternalLifeAssurance { get; set; }
        public string HeavenReason { get; set; }
        public string MeaningOfChristsDeath { get; set; }

        public bool IsBaptizedByImmersion { get; set; }
        public DateTime? BaptismDate { get; set; }
        public string BaptismPlace { get; set; }
        public bool WillingToBeBaptizedAtGCI { get; set; }

        public string PreviousMinistryExperience { get; set; }
        public string SpecialGiftsOrServiceInterest { get; set; }

        public bool IsInformationConfirmed { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }
        [ForeignKey(nameof(MemberId))]
        public Member Member { get; set; }
        [MaxLength(1000)]


        public string? County { get; set; }
        [MaxLength(150)]
        public string? Occupation { get; set; }
        [MaxLength(500)]
        public string? ReasonsForMembership { get; set; }

        [MaxLength(50)]
        public string? SpousePhone { get; set; }
        public int? YearMarried { get; set; }

        


    }
}
