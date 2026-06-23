using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("MinistryMembers")]
    public class MinistryMember
    {
        [Key]
        public int Id { get; set; }

        public int MinistryId { get; set; }

        public int MemberId { get; set; }

        public bool? IsApproved { get; set; }

        public DateTime? RequestedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? MembershipStatusId { get; set; }

        public int? DepartmentId { get; set; }

        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }

        [ForeignKey("MinistryId")]
        public virtual Ministry Ministry { get; set; }
    }
}
