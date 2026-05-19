using GCI_Admin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class MeetingsRepository
    {
        private readonly AppDbContext _context;
        private readonly CommunicationService _communicationService;

        public MeetingsRepository(AppDbContext context, CommunicationService communicationService)
        {
            _context = context;
            _communicationService = communicationService;
        }

        //get all meetings
        public async Task<DbResponse<List<MeetingAttendance>>> GetAllMeetingsAsync()
        {
            var response = new DbResponse<List<MeetingAttendance>>();
            try
            {
                response.Data = await _context.MeetingAttendances
                    .Where(x => x.IsActive)
                    .OrderByDescending(x => x.MeetingDate)
                    .ToListAsync();
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetAllMeetingsAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<DbResponse<MeetingAttendance>> GetMeetingDetailsByIdAsync(int meetingId)
        {
            var response = new DbResponse<MeetingAttendance>();
            try
            {
                var meeting = await _context.MeetingAttendances
                    .FirstOrDefaultAsync(x => x.MeetingAttendancesId == meetingId && x.IsActive);
                if (meeting == null)
                {
                    response.Success = false;
                    response.Message = "Meeting not found";
                    return response;
                }
                response.Data = meeting;
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetMeetingDetailsByIdAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<DbResponse<ServiceCollectionSummary>> GetFinancialSummaryByMeetingIdAsync(int meetingId)
        {
            var response = new DbResponse<ServiceCollectionSummary>();
            try
            {
                var summary = await _context.ServiceCollectionSummaries
                    .FirstOrDefaultAsync(x => x.MeetingAttendancesId == meetingId);
                if (summary == null)
                {
                    response.Success = false;
                    response.Message = "Financial summary not found for this meeting";
                    return response;
                }
                response.Data = summary;
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetFinancialSummaryByMeetingIdAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }


        // 📊 DASHBOARD STATISTICS METHODS
        public async Task<DbResponse<DashboardStats>> GetDashboardStatsAsync()
        {
            var response = new DbResponse<DashboardStats>();

            try
            {
                var stats = new DashboardStats();

                // Total meetings
                stats.TotalMeetings = await _context.MeetingAttendances
                    .Where(x => x.IsActive)
                    .CountAsync();

                // Total unique meeting types
                stats.MeetingTypesCount = await _context.MeetingAttendances
                    .Where(x => x.IsActive)
                    .Select(x => x.MeetingType)
                    .Distinct()
                    .CountAsync();

                // Total attendees across all meetings
                stats.TotalAttendees = await _context.MeetingAttendances
                    .Where(x => x.IsActive)
                    .SumAsync(x => x.TotalAttendees);

                // Average attendance per meeting
                var meetingCount = await _context.MeetingAttendances
                    .Where(x => x.IsActive)
                    .CountAsync();

                stats.AverageAttendance = meetingCount > 0
                    ? stats.TotalAttendees / meetingCount
                    : 0;

                // Gender distribution
                stats.TotalMale = await _context.MeetingAttendances
                    .Where(x => x.IsActive && x.MaleCount.HasValue)
                    .SumAsync(x => x.MaleCount ?? 0);

                stats.TotalFemale = await _context.MeetingAttendances
                    .Where(x => x.IsActive && x.FemaleCount.HasValue)
                    .SumAsync(x => x.FemaleCount ?? 0);

                stats.TotalChildren = await _context.MeetingAttendances
                    .Where(x => x.IsActive && x.ChildrenCount.HasValue)
                    .SumAsync(x => x.ChildrenCount ?? 0);

                // Recent meetings (last 30 days)
                var last30Days = DateTime.Now.AddDays(-30);
                stats.MeetingsLast30Days = await _context.MeetingAttendances
                    .Where(x => x.IsActive && x.MeetingDate >= last30Days)
                    .CountAsync();

                stats.AttendeesLast30Days = await _context.MeetingAttendances
                    .Where(x => x.IsActive && x.MeetingDate >= last30Days)
                    .SumAsync(x => x.TotalAttendees);

                // Financial statistics
                var financialStats = await GetFinancialStatisticsAsync();
                stats.TotalTithes = financialStats.TotalTithes;
                stats.TotalOfferings = financialStats.TotalOfferings;
                stats.TotalMissions = financialStats.TotalMissions;
                stats.TotalProjects = financialStats.TotalProjects;
                stats.TotalSundaySchool = financialStats.TotalSundaySchool;
                stats.TotalThanksgiving = financialStats.TotalThanksgiving;
                stats.TotalYouth = financialStats.TotalYouth;
                stats.TotalWidowsOrphans = financialStats.TotalWidowsOrphans;
                stats.TotalOthers = financialStats.TotalOthers;
                stats.GrandTotalCollections = financialStats.GrandTotal;

                // Signature verification stats
                stats.TotalSignaturesRequired = await _context.ServiceCollectionSignatures
                    .CountAsync();

                stats.TotalSignaturesCompleted = await _context.ServiceCollectionSignatures
                    .Where(x => x.IsSigned)
                    .CountAsync();

                stats.TotalVerificationsCompleted = await _context.ServiceCollectionSignatures
                    .Where(x => x.IsVerified)
                    .CountAsync();

                stats.SignatureCompletionRate = stats.TotalSignaturesRequired > 0
                    ? (decimal)stats.TotalSignaturesCompleted / stats.TotalSignaturesRequired * 100
                    : 0;

                response.Data = stats;
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetDashboardStatsAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 📈 GET ALL MEETINGS WITH PAGINATION
        public async Task<DbResponse<PagedResult<MeetingAttendance>>> GetAllMeetingsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = new DbResponse<PagedResult<MeetingAttendance>>();

            try
            {
                var query = _context.MeetingAttendances
                    .Where(x => x.IsActive)
                    .OrderByDescending(x => x.MeetingDate);

                var totalCount = await query.CountAsync();
                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                response.Data = new PagedResult<MeetingAttendance>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetAllMeetingsAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 🔍 GET MEETING BY ID WITH FULL DETAILS
        public async Task<DbResponse<MeetingFullDetails>> GetMeetingByIdAsync(int meetingId)
        {
            var response = new DbResponse<MeetingFullDetails>();

            try
            {
                var meeting = await _context.MeetingAttendances
                    .FirstOrDefaultAsync(x => x.MeetingAttendancesId == meetingId && x.IsActive);

                if (meeting == null)
                {
                    response.Success = false;
                    response.Message = "Meeting not found";
                    return response;
                }

                var details = new MeetingFullDetails
                {
                    Meeting = meeting,
                    FinancialSummary = await _context.ServiceCollectionSummaries
                        .FirstOrDefaultAsync(x => x.MeetingAttendancesId == meetingId),
                    CashBreakdowns = await _context.ServiceCashBreakdowns
                        .Where(x => x.MeetingAttendancesId == meetingId)
                        .ToListAsync(),
                    BankCollections = await _context.ServiceBankCollections
                        .Where(x => x.MeetingAttendancesId == meetingId)
                        .ToListAsync(),
                    Signatures = await _context.ServiceCollectionSignatures
                        .Where(x => x.MeetingAttendancesId == meetingId)
                        .OrderBy(x => x.SignatureOrder)
                        .ToListAsync()
                };

                response.Data = details;
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetMeetingByIdAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 📅 GET MEETINGS BY TYPE
        public async Task<DbResponse<List<MeetingAttendance>>> GetMeetingsByTypeAsync(string meetingType)
        {
            var response = new DbResponse<List<MeetingAttendance>>();

            try
            {
                response.Data = await _context.MeetingAttendances
                    .Where(x => x.MeetingType == meetingType && x.IsActive)
                    .OrderByDescending(x => x.MeetingDate)
                    .ToListAsync();

                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetMeetingsByTypeAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 📊 GET MEETINGS BY DATE RANGE
        public async Task<DbResponse<List<MeetingAttendance>>> GetMeetingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var response = new DbResponse<List<MeetingAttendance>>();

            try
            {
                response.Data = await _context.MeetingAttendances
                    .Where(x => x.IsActive && x.MeetingDate >= startDate && x.MeetingDate <= endDate)
                    .OrderByDescending(x => x.MeetingDate)
                    .ToListAsync();

                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetMeetingsByDateRangeAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 💰 GET FINANCIAL SUMMARY FOR A MEETING
        public async Task<DbResponse<ServiceCollectionSummary>> GetMeetingFinancialSummaryAsync(int meetingId)
        {
            var response = new DbResponse<ServiceCollectionSummary>();

            try
            {
                var summary = await _context.ServiceCollectionSummaries
                    .FirstOrDefaultAsync(x => x.MeetingAttendancesId == meetingId);

                if (summary == null)
                {
                    response.Success = false;
                    response.Message = "Financial summary not found for this meeting";
                    return response;
                }

                response.Data = summary;
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetMeetingFinancialSummaryAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 📋 GET SIGNATURE STATUS FOR A MEETING
        public async Task<DbResponse<List<ServiceCollectionSignature>>> GetMeetingSignaturesAsync(int meetingId)
        {
            var response = new DbResponse<List<ServiceCollectionSignature>>();

            try
            {
                response.Data = await _context.ServiceCollectionSignatures
                    .Where(x => x.MeetingAttendancesId == meetingId)
                    .OrderBy(x => x.SignatureOrder)
                    .ToListAsync();

                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetMeetingSignaturesAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 📊 GET MONTHLY ATTENDANCE TREND
        public async Task<DbResponse<List<MonthlyAttendanceStats>>> GetMonthlyAttendanceTrendAsync(int months = 6)
        {
            var response = new DbResponse<List<MonthlyAttendanceStats>>();

            try
            {
                var startDate = DateTime.Now.AddMonths(-months);
                var stats = await _context.MeetingAttendances
                    .Where(x => x.IsActive && x.MeetingDate >= startDate)
                    .GroupBy(x => new { x.MeetingDate.Year, x.MeetingDate.Month })
                    .Select(g => new MonthlyAttendanceStats
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalMeetings = g.Count(),
                        TotalAttendees = g.Sum(x => x.TotalAttendees),
                        AverageAttendance = g.Average(x => x.TotalAttendees)
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                response.Data = stats;
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetMonthlyAttendanceTrendAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // Helper method for financial statistics
        private async Task<FinancialStatistics> GetFinancialStatisticsAsync()
        {
            var summaries = await _context.ServiceCollectionSummaries.ToListAsync();

            return new FinancialStatistics
            {
                TotalTithes = summaries.Sum(x => x.Tithes),
                TotalOfferings = summaries.Sum(x => x.Offerings),
                TotalSundaySchool = summaries.Sum(x => x.SundaySchool),
                TotalThanksgiving = summaries.Sum(x => x.Thanksgiving),
                TotalMissions = summaries.Sum(x => x.Missions),
                TotalProjects = summaries.Sum(x => x.Projects),
                TotalYouth = summaries.Sum(x => x.Youth),
                TotalWidowsOrphans = summaries.Sum(x => x.WidowsOrphans),
                TotalOthers = summaries.Sum(x => x.Others),
                GrandTotal = summaries.Sum(x => x.Tithes + x.Offerings + x.SundaySchool + x.Thanksgiving +
                                               x.Missions + x.Projects + x.Youth + x.WidowsOrphans + x.Others)
            };
        }
        // 💰 GET SERVICE COLLECTION SUMMARY BY MEETING ID
        public async Task<DbResponse<ServiceCollectionSummary>> GetServiceCollectionSummaryByMeetingIdAsync(int meetingId)
        {
            var response = new DbResponse<ServiceCollectionSummary>();

            try
            {
                var summary = await _context.ServiceCollectionSummaries
                    .FirstOrDefaultAsync(x => x.MeetingAttendancesId == meetingId);

                if (summary == null)
                {
                    response.Success = false;
                    response.Message = "Service collection summary not found for this meeting";
                    return response;
                }

                response.Data = summary;
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetServiceCollectionSummaryByMeetingIdAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 💵 GET ALL CASH BREAKDOWNS BY MEETING ID
        public async Task<DbResponse<List<ServiceCashBreakdown>>> GetCashBreakdownsByMeetingIdAsync(int meetingId)
        {
            var response = new DbResponse<List<ServiceCashBreakdown>>();

            try
            {
                var cashBreakdowns = await _context.ServiceCashBreakdowns
                    .Where(x => x.MeetingAttendancesId == meetingId)
                    .OrderBy(x => x.Denomination)
                    .ToListAsync();

                response.Data = cashBreakdowns;
                response.Success = true;

                if (cashBreakdowns.Count == 0)
                {
                    response.Message = "No cash breakdowns found for this meeting";
                }
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetCashBreakdownsByMeetingIdAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 🏦 GET ALL BANK COLLECTIONS BY MEETING ID
        public async Task<DbResponse<List<ServiceBankCollection>>> GetBankCollectionsByMeetingIdAsync(int meetingId)
        {
            var response = new DbResponse<List<ServiceBankCollection>>();

            try
            {
                var bankCollections = await _context.ServiceBankCollections
                    .Where(x => x.MeetingAttendancesId == meetingId)
                    .OrderByDescending(x => x.ServiceBankCollectionId)
                    .ToListAsync();

                response.Data = bankCollections;
                response.Success = true;

                if (bankCollections.Count == 0)
                {
                    response.Message = "No bank collections found for this meeting";
                }
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetBankCollectionsByMeetingIdAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // ✍️ GET ALL SIGNATURES BY MEETING ID
        public async Task<DbResponse<List<ServiceCollectionSignature>>> GetSignaturesByMeetingIdAsync(int meetingId)
        {
            var response = new DbResponse<List<ServiceCollectionSignature>>();

            try
            {
                var signatures = await _context.ServiceCollectionSignatures
                    .Where(x => x.MeetingAttendancesId == meetingId)
                    .OrderBy(x => x.SignatureOrder)
                    .ToListAsync();

                response.Data = signatures;
                response.Success = true;

                if (signatures.Count == 0)
                {
                    response.Message = "No signatures found for this meeting";
                }
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetSignaturesByMeetingIdAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // ✍️ GET SIGNATURE BY SIGNATURE ID (for individual signature details)
        public async Task<DbResponse<ServiceCollectionSignature>> GetSignatureByIdAsync(int signatureId)
        {
            var response = new DbResponse<ServiceCollectionSignature>();

            try
            {
                var signature = await _context.ServiceCollectionSignatures
                    .FirstOrDefaultAsync(x => x.ServiceCollectionSignatureId == signatureId);

                if (signature == null)
                {
                    response.Success = false;
                    response.Message = "Signature not found";
                    return response;
                }

                response.Data = signature;
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetSignatureByIdAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 📊 GET COMPLETE FINANCIAL DATA FOR A MEETING (Combined)
        public async Task<DbResponse<MeetingFinancialData>> GetCompleteFinancialDataByMeetingIdAsync(int meetingId)
        {
            var response = new DbResponse<MeetingFinancialData>();

            try
            {
                var financialData = new MeetingFinancialData
                {
                    MeetingId = meetingId,
                    Summary = await _context.ServiceCollectionSummaries
                        .FirstOrDefaultAsync(x => x.MeetingAttendancesId == meetingId),
                    CashBreakdowns = await _context.ServiceCashBreakdowns
                        .Where(x => x.MeetingAttendancesId == meetingId)
                        .OrderBy(x => x.Denomination)
                        .ToListAsync(),
                    BankCollections = await _context.ServiceBankCollections
                        .Where(x => x.MeetingAttendancesId == meetingId)
                        .OrderByDescending(x => x.ServiceBankCollectionId)
                        .ToListAsync(),
                    Signatures = await _context.ServiceCollectionSignatures
                        .Where(x => x.MeetingAttendancesId == meetingId)
                        .OrderBy(x => x.SignatureOrder)
                        .ToListAsync()
                };

                response.Data = financialData;
                response.Success = true;

                if (financialData.Summary == null)
                {
                    response.Message = "Meeting found but no financial summary available";
                }
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetCompleteFinancialDataByMeetingIdAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 📊 GET SIGNATURE STATUS SUMMARY BY MEETING ID
        public async Task<DbResponse<SignatureStatusSummary>> GetSignatureStatusSummaryByMeetingIdAsync(int meetingId)
        {
            var response = new DbResponse<SignatureStatusSummary>();

            try
            {
                var signatures = await _context.ServiceCollectionSignatures
                    .Where(x => x.MeetingAttendancesId == meetingId)
                    .ToListAsync();

                var summary = new SignatureStatusSummary
                {
                    MeetingId = meetingId,
                    TotalSignatures = signatures.Count,
                    SignedCount = signatures.Count(x => x.IsSigned),
                    VerifiedCount = signatures.Count(x => x.IsVerified),
                    PendingCount = signatures.Count(x => !x.IsSigned),
                    CompletionPercentage = signatures.Count > 0
                        ? (decimal)signatures.Count(x => x.IsSigned) / signatures.Count * 100
                        : 0,
                    VerificationPercentage = signatures.Count > 0
                        ? (decimal)signatures.Count(x => x.IsVerified) / signatures.Count * 100
                        : 0,
                    Signatures = signatures.OrderBy(x => x.SignatureOrder).ToList()
                };

                response.Data = summary;
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetSignatureStatusSummaryByMeetingIdAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 💰 GET TOTAL COLLECTIONS SUMMARY BY MEETING ID
        public async Task<DbResponse<TotalCollectionsSummary>> GetTotalCollectionsSummaryByMeetingIdAsync(int meetingId)
        {
            var response = new DbResponse<TotalCollectionsSummary>();

            try
            {
                var summary = await _context.ServiceCollectionSummaries
                    .FirstOrDefaultAsync(x => x.MeetingAttendancesId == meetingId);

                if (summary == null)
                {
                    response.Success = false;
                    response.Message = "No collection summary found for this meeting";
                    return response;
                }

                var cashTotal = await _context.ServiceCashBreakdowns
                    .Where(x => x.MeetingAttendancesId == meetingId)
                    .SumAsync(x => x.Total);

                var bankTotal = await _context.ServiceBankCollections
                    .Where(x => x.MeetingAttendancesId == meetingId)
                    .SumAsync(x => x.Amount);

                var totalCollections = new TotalCollectionsSummary
                {
                    MeetingId = meetingId,
                    CashTotal = cashTotal,
                    BankTotal = bankTotal,
                    GrandTotal = cashTotal + bankTotal,
                    SummaryBreakdown = new SummaryBreakdown
                    {
                        Tithes = summary.Tithes,
                        Offerings = summary.Offerings,
                        SundaySchool = summary.SundaySchool,
                        Thanksgiving = summary.Thanksgiving,
                        Missions = summary.Missions,
                        Projects = summary.Projects,
                        Youth = summary.Youth,
                        WidowsOrphans = summary.WidowsOrphans,
                        Others = summary.Others
                    }
                };

                response.Data = totalCollections;
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MeetingsRepository->GetTotalCollectionsSummaryByMeetingIdAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }

   
}