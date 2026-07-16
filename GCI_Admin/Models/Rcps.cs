using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models
{
    public class Rcps
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal AmountRaised { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? CountyCode { get; set; }
        public virtual ICollection<RcpsCountyCoordinator> CountyCoordinators { get; set; }


        public decimal PercentageComplete
        {
            get
            {
                if (TargetAmount <= 0) return 0;
                return (AmountRaised / TargetAmount) * 100;
            }
        }
    }

    public class RcpsViewModel
    {
        public int TotalRcps { get; set; }
        public int ActiveRcps { get; set; }
        public int CompletedRcps { get; set; }
        public decimal TotalRaised { get; set; }
        public Rcps CurrentActiveRcps { get; set; }
        public List<Rcps> Rcps { get; set; }
        public List<RcpsCountyCoordinator> CountyCoordinators { get; set; }
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Counties { get; set; }
    }
    public enum RCPStatus
    {
        [Display(Name = "Planning")]
        Planning = 1,

        [Display(Name = "Ongoing")]
        Active = 2,

        [Display(Name = "On Hold")]
        OnHold = 3,

        [Display(Name = "Completed")]
        Completed = 4,

        [Display(Name = "Cancelled")]
        Cancelled = 5
    }
    public class RcpsDetailsViewModel
    {
        public Rcps Rcps { get; set; }
        public List<RcpsPledges> Pledges { get; set; }
        public List<RcpsCountyCoordinator> CountyCoordinators { get; set; }
        public List<RcpCountyMember> CountyMembers { get; set; } = new List<RcpCountyMember>();

        // Statistics
        public int TotalPledges { get; set; }
        public decimal TotalPledgedAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public int ActivePledges { get; set; }

        // Calculated Properties
        public decimal ProgressPercentage
        {
            get
            {
                if (Rcps == null || Rcps.TargetAmount <= 0) return 0;
                return (Rcps.AmountRaised / Rcps.TargetAmount) * 100;
            }
        }

        public decimal RedemptionPercentage
        {
            get
            {
                if (TotalPledgedAmount <= 0) return 0;
                return (TotalPaidAmount / TotalPledgedAmount) * 100;
            }
        }

        public decimal RemainingPledgeAmount
        {
            get
            {
                return TotalPledgedAmount - TotalPaidAmount;
            }
        }

        public bool IsFullyFunded
        {
            get
            {
                return Rcps != null && Rcps.AmountRaised >= Rcps.TargetAmount;
            }
        }
    }

    public class RcpsPledges
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int RcpsId { get; set; }
        public decimal PledgedAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PledgeDate { get; set; }
        public DateTime? TargetCompletionDate { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal Balance { get; set; }
        public bool PaymentRecieved { get; set; }

        public Member Member { get; set; }
    }

    public class RcpsPledgesViewModel
    {
        // Statistics
        public int TotalPledges { get; set; }
        public decimal TotalPledgedAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public int ActivePledges { get; set; }

        // RCP Selection
        public List<DropdownItem> RcpsList { get; set; }

        public int? SelectedRcpsId { get; set; }
        public string SelectedRcpsName { get; set; }

        // RCP Specific Statistics
        public RcpsStatisticsViewModel RcpsStats { get; set; } = new RcpsStatisticsViewModel();
        // Pledges List
        public List<RcpsPledges> Pledges { get; set; }
    }
    public class RcpsStatisticsViewModel
    {
        public int PledgeCount { get; set; }
        public decimal TotalPledged { get; set; }
        public decimal TotalRedeemed { get; set; }
        public decimal FulfillmentRate { get; set; }
        public decimal? RcpsTarget { get; set; }
        public decimal? PercentageOfTarget { get; set; }
        public int ActivePledgesCount { get; set; }
        public int FulfilledPledgesCount { get; set; }
        public int OverduePledgesCount { get; set; }
        public int PendingPledgesCount { get; set; }
        public int PartiallyPaidPledgesCount { get; set; }

        public Dictionary<string, int> PledgeStatusBreakdown { get; set; }

        public decimal AveragePledgeAmount
        {
            get
            {
                if (PledgeCount <= 0) return 0;
                return TotalPledged / PledgeCount;
            }
        }

        public decimal AverageRedemptionRate
        {
            get
            {
                if (PledgeCount <= 0) return 0;
                return TotalRedeemed / PledgeCount;
            }
        }
    }

    public class PledgeChartData
    {
        public List<string> Labels { get; set; }
        public List<decimal> PledgedAmounts { get; set; }
        public List<decimal> RedeemedAmounts { get; set; }
        public List<int> PledgeCounts { get; set; }
        public Dictionary<string, int> StatusDistribution { get; set; }
        public Dictionary<string, decimal> TopRCPsByPledge { get; set; }
        public Dictionary<string, decimal> TopRCPsByRedemption { get; set; }
    }

}