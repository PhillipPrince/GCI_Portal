using GCI_Admin.Models.DTOs;

namespace GCI_Admin.Models
{
    public class Finance
    {
        public List<Payment> Payments { get; set; }

        public List<AccountReferenceSummaryDto> AccountReferenceSummaries { get; set; }
    }
    public class SendOtpRequest
    {
        public int MeetingId { get; set; }
        public string EmailOrPhone { get; set; }
    }

    public class VerifyCollectionRequest
    {
        public int MeetingId { get; set; }
        public string OtpCode { get; set; }
        public string EmailOrPhone { get; set; }
    }
}
