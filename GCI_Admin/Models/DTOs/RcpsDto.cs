namespace GCI_Admin.Models.DTOs
{
    public class RcpsDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal AmountRaised { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }


    public class RcpsPledgesDto
    {
        public int MemberId { get; set; }
        public int RcpsId { get; set; }
        public decimal PledgedAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PledgeDate { get; set; }
        public DateTime? TargetCompletionDate { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public bool PaymentRecieved { get; set; }
    }
}