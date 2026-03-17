using GCI_Admin.Models.DTOs;

namespace GCI_Admin.Models
{
    public class Finance
    {
        public List<Payment> Payments { get; set; }

        public List<AccountReferenceSummaryDto> AccountReferenceSummaries { get; set; }
    }
}
