namespace GCI_Admin.Models.DTOs
{
    public class AccountReferenceSummaryDto
    {
        public string AccountReference { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
    }
}
