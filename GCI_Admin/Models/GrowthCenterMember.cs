using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("GrowthCenterMembers")]
    public class GrowthCenterMember
    {
        [Key]
        public int GrowthCenterMemberId { get; set; }

        public int GrowthCenterId { get; set; }

        public int MemberId { get; set; }

        public int? MembershipStatusId { get; set; }

        public bool IsActive { get; set; }

        public DateTime? RequestedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }

        [ForeignKey("GrowthCenterId")]
        public virtual GrowthCenter GrowthCenter { get; set; }
    }
}
