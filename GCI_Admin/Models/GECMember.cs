using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    public class GECMember
    {
        [Key]
        public int GECId { get; set; }
        public int MemberId { get; set; }
        public string PositionTitle { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public byte[]? Photo { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public string Phone { get; set; }
        [NotMapped]
        public string Email { get; set; }
        [NotMapped]
        public string Gender { get; set; }
    }
}
