using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    
        [Table("Collections")]
        public class Collection
        {
            [Key]
            public int Id { get; set; }

            public int MemberId { get; set; }

            [MaxLength(100)]
            public string? MerchantRequestID { get; set; }

            [MaxLength(100)]
            public string? CheckoutRequestID { get; set; }

            [MaxLength(100)]
            public string? AccountReference { get; set; }

            [MaxLength(20)]
            public string? PhoneNumber { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            public decimal Amount { get; set; }

            [MaxLength(50)]
            public string? MpesaReceiptNumber { get; set; }

            [MaxLength(50)]
            public string? Paybill { get; set; }

            public DateTime? TransactionDate { get; set; }

            public int PaymentStatusId { get; set; }

            public int? MeetingId { get; set; }

            public int? ResultCode { get; set; }

            [MaxLength(255)]
            public string? ResultDesc { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public DateTime? UpdatedAt { get; set; }

            
        }
    
}
