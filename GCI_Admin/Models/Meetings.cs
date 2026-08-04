using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCI_Admin.Models
{
    public class MeetingAttendance
    {
        [Key]
        public int MeetingAttendancesId { get; set; }

        public string MeetingType { get; set; }

        public DateTime MeetingDate { get; set; }

        public int TotalAttendees { get; set; }

        public int? MaleCount { get; set; }

        public int? FemaleCount { get; set; }

        public int? ChildrenCount { get; set; }

        public string? Remarks { get; set; }

        public int? RecordedBy { get; set; }
        [NotMapped]
        public string? RecorderName { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
        [NotMapped]
        public virtual Member Recorder { get; set; }



    }
    public class ServiceCollectionSummary
    {
        [Key]
        public int ServiceCollectionSummaryId { get; set; }

        public int MeetingAttendancesId { get; set; }

        public decimal Tithes { get; set; }
        public decimal Offerings { get; set; }
        public decimal SundaySchool { get; set; }
        public decimal Thanksgiving { get; set; }
        public decimal Missions { get; set; }
        public decimal Projects { get; set; }
        public decimal Youth { get; set; }
        public decimal WidowsOrphans { get; set; }
        public decimal Others { get; set; }
        public bool IsVerified { get; set; }

        public int VerifiedBy { get; set; }
        public DateTime? VerifiedAt{get; set;}



    }
    public class ServiceCashBreakdown
    {
        [Key]
        public int ServiceCashBreakdownId { get; set; }

        public int MeetingAttendancesId { get; set; }

        public int Denomination { get; set; }

        public int Quantity { get; set; }

        public int Total { get; set; }

    }
    public class ServiceBankCollection
    {
        [Key]
        public int ServiceBankCollectionId { get; set; }

        public int MeetingAttendancesId { get; set; }

        public decimal Amount { get; set; }

        public string? Reference { get; set; }

    }
    public class ServiceCollectionSignature
    {
        [Key]
        public int ServiceCollectionSignatureId { get; set; }
        public int MeetingAttendancesId { get; set; }
        public int SignerMemberId { get; set; }
        [NotMapped]
        public string? Name { get; set; }

        public int? SignatureOrder { get; set; }

        public bool IsSigned { get; set; } = false;
        public DateTime? SignedAt { get; set; }

        public bool IsVerified { get; set; } = false;
        public DateTime? VerifiedAt { get; set; }

        public DateTime? OtpSentAt { get; set; }
        public DateTime? OtpVerifiedAt { get; set; }
        public string? OtpChannel { get; set; }
        public int? ResendCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        [NotMapped]
        public virtual Member Signer { get; set; }

    }
    public class DashboardStats
    {
        public int TotalMeetings { get; set; }
        public int MeetingTypesCount { get; set; }
        public int TotalAttendees { get; set; }
        public decimal AverageAttendance { get; set; }
        public int TotalMale { get; set; }
        public int TotalFemale { get; set; }
        public int TotalChildren { get; set; }
        public int MeetingsLast30Days { get; set; }
        public int AttendeesLast30Days { get; set; }

        // Financial stats
        public decimal TotalTithes { get; set; }
        public decimal TotalOfferings { get; set; }
        public decimal TotalSundaySchool { get; set; }
        public decimal TotalThanksgiving { get; set; }
        public decimal TotalMissions { get; set; }
        public decimal TotalProjects { get; set; }
        public decimal TotalYouth { get; set; }
        public decimal TotalWidowsOrphans { get; set; }
        public decimal TotalOthers { get; set; }
        public decimal GrandTotalCollections { get; set; }

        // Signature stats
        public int TotalSignaturesRequired { get; set; }
        public int TotalSignaturesCompleted { get; set; }
        public int TotalVerificationsCompleted { get; set; }
        public decimal SignatureCompletionRate { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public class MeetingFullDetails
    {
        public MeetingAttendance Meeting { get; set; }
        public ServiceCollectionSummary FinancialSummary { get; set; }
        public List<ServiceCashBreakdown> CashBreakdowns { get; set; }
        public List<ServiceBankCollection> BankCollections { get; set; }
        public List<ServiceCollectionSignature> Signatures { get; set; }
    }

    public class MonthlyAttendanceStats
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalMeetings { get; set; }
        public int TotalAttendees { get; set; }
        public double AverageAttendance { get; set; }
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM");
    }

    public class FinancialStatistics
    {
        public decimal TotalTithes { get; set; }
        public decimal TotalOfferings { get; set; }
        public decimal TotalSundaySchool { get; set; }
        public decimal TotalThanksgiving { get; set; }
        public decimal TotalMissions { get; set; }
        public decimal TotalProjects { get; set; }
        public decimal TotalYouth { get; set; }
        public decimal TotalWidowsOrphans { get; set; }
        public decimal TotalOthers { get; set; }
        public decimal GrandTotal { get; set; }
    }

    public class MeetingFinancialData
    {
        public int MeetingId { get; set; }
        public ServiceCollectionSummary Summary { get; set; }
        public List<ServiceCashBreakdown> CashBreakdowns { get; set; }
        public List<ServiceBankCollection> BankCollections { get; set; }
        public List<ServiceCollectionSignature> Signatures { get; set; }
    }

    public class SignatureStatusSummary
    {
        public int MeetingId { get; set; }
        public int TotalSignatures { get; set; }
        public int SignedCount { get; set; }
        public int VerifiedCount { get; set; }
        public int PendingCount { get; set; }
        public decimal CompletionPercentage { get; set; }
        public decimal VerificationPercentage { get; set; }
        public List<ServiceCollectionSignature> Signatures { get; set; }
    }

    public class TotalCollectionsSummary
    {
        public int MeetingId { get; set; }
        public decimal CashTotal { get; set; }
        public decimal BankTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public SummaryBreakdown SummaryBreakdown { get; set; }
    }

    public class SummaryBreakdown
    {
        public decimal Tithes { get; set; }
        public decimal Offerings { get; set; }
        public decimal SundaySchool { get; set; }
        public decimal Thanksgiving { get; set; }
        public decimal Missions { get; set; }
        public decimal Projects { get; set; }
        public decimal Youth { get; set; }
        public decimal WidowsOrphans { get; set; }
        public decimal Others { get; set; }
    }
}
