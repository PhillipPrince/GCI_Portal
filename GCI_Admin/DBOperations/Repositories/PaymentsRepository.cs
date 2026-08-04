
using GCI_Admin.DBOperations;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Repo_GCI
{
    public class CollectionsRepository
    {
        private readonly AppDbContext _context;

        public CollectionsRepository(AppDbContext context)
        {
            _context = context;
        }

       
    
      
        public async Task<DbResponse<List<Collection>>> GetAllAsync()
        {
            try
            {
                var Collections = await _context.Collections
                    .Where(x => x.PaymentStatusId == 2)
                    .OrderBy(x => x.Id)
                    .ToListAsync();

                return new DbResponse<List<Collection>>
                {
                    Success = true,
                    Data = Collections
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Collection>>
                {
                    Success = false,
                    Message = $"Error fetching Collections: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<Collection>>> GetByMemberIdAsync(int memberId)
        {
            try
            {
                var Collections = await _context.Collections
                    .OrderBy(x => x.Id)
                    .ToListAsync();

                return new DbResponse<List<Collection>>
                {
                    Success = true,
                    Data = Collections
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Collection>>
                {
                    Success = false,
                    Message = $"Error fetching member Collections: {ex.Message}"
                };
            }
        }
        public async Task<DbResponse<List<AccountReferenceSummaryDto>>> GetAccountReferenceSummaryAsync()
        {
            try
            {
                var result = await _context.Collections
                    .Where(x => !string.IsNullOrEmpty(x.AccountReference)&& x.PaymentStatusId == 2)
                    .GroupBy(x => x.AccountReference)
                    .Select(g => new AccountReferenceSummaryDto
                    {
                        AccountReference = g.Key,
                        TotalAmount = g.Sum(x => x.Amount),
                        TransactionCount = g.Count()
                    })
                    .OrderByDescending(x => x.TotalAmount)
                    .ToListAsync();

                return new DbResponse<List<AccountReferenceSummaryDto>>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<AccountReferenceSummaryDto>>
                {
                    Success = false,
                    Message = $"Error fetching account summaries: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<MeetingAttendance>>> GetActiveMeetingsAsync()
        {
            try
            {
                var meetings = await _context.MeetingAttendances
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.MeetingDate)
                    .Take(50)
                    .ToListAsync();

                return new DbResponse<List<MeetingAttendance>>
                {
                    Success = true,
                    Data = meetings
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<MeetingAttendance>>
                {
                    Success = false,
                    Message = $"Error fetching meetings: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<Collection>> SaveManualCollectionWithReconciliationAsync(Collection Collection)
        {
            try
            {
                Collection.CreatedAt = DateTime.UtcNow;
                Collection.MerchantRequestID = "MANUAL";
                Collection.CheckoutRequestID = "MANUAL";
                if (string.IsNullOrEmpty(Collection.Paybill))
                {
                    Collection.Paybill = "CASH";
                }

                if (Collection.MeetingId.HasValue && Collection.MeetingId.Value > 0)
                {
                    var meetingId = Collection.MeetingId.Value;
                    var meeting = await _context.MeetingAttendances.FirstOrDefaultAsync(m => m.MeetingAttendancesId == meetingId);
                    var summary = await _context.ServiceCollectionSummaries
                        .FirstOrDefaultAsync(s => s.MeetingAttendancesId == meetingId && s.IsVerified);

                    if (summary != null)
                    {
                        var bulkRecord = await _context.Collections
                            .FirstOrDefaultAsync(c => c.MeetingId == meetingId && c.AccountReference == Collection.AccountReference && c.MemberId == 0);

                        if (bulkRecord == null && meeting != null)
                        {
                            var meetingDate = meeting.MeetingDate.Date;
                            bulkRecord = await _context.Collections
                                .FirstOrDefaultAsync(c => c.MemberId == 0 && c.AccountReference == Collection.AccountReference && c.TransactionDate.HasValue && c.TransactionDate.Value.Date == meetingDate);
                            if (bulkRecord != null)
                            {
                                bulkRecord.MeetingId = meetingId;
                            }
                        }

                        if (bulkRecord != null)
                        {
                            bulkRecord.Amount = Math.Max(0, bulkRecord.Amount - Collection.Amount);
                            Collection.TransactionDate = bulkRecord.TransactionDate ?? (meeting?.MeetingDate ?? DateTime.UtcNow);
                        }
                    }
                }

                if (!Collection.TransactionDate.HasValue)
                {
                    Collection.TransactionDate = DateTime.UtcNow;
                }

                _context.Collections.Add(Collection);
                await _context.SaveChangesAsync();

                return new DbResponse<Collection>
                {
                    Success = true,
                    Data = Collection,
                    Message = "Collection saved successfully"
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<Collection>
                {
                    Success = false,
                    Message = $"Error saving Collection: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<bool>> CheckAndUpdateResendOtpLimitAsync(int meetingId)
        {
            try
            {
                if (meetingId > 0)
                {
                    var sig = await _context.ServiceCollectionSignatures
                        .FirstOrDefaultAsync(s => s.MeetingAttendancesId == meetingId && !s.IsVerified);
                    if (sig != null)
                    {
                        int currentResends = sig.ResendCount ?? 0;
                        if (currentResends >= 3)
                        {
                            return new DbResponse<bool>
                            {
                                Success = false,
                                Message = "Maximum OTP resend limit (3 times) reached for this signature confirmation."
                            };
                        }
                        sig.ResendCount = currentResends + 1;
                        sig.UpdatedAt = DateTime.Now;
                        _context.ServiceCollectionSignatures.Update(sig);
                        await _context.SaveChangesAsync();
                    }
                }

                return new DbResponse<bool> { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<DbResponse<List<Collection>>> GetGBICollectionsAsync()
        {
            try
            {
                var collections = await _context.Collections
                    .Where(x => x.Paybill == "4099245" && x.PaymentStatusId == 2)
                    .OrderByDescending(x => x.TransactionDate ?? x.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<Collection>> { Success = true, Data = collections };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Collection>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<DbResponse<List<AccountReferenceSummaryDto>>> GetGBIAccountReferenceSummaryAsync()
        {
            try
            {
                var result = await _context.Collections
                    .Where(x => x.Paybill == "4099245" && !string.IsNullOrEmpty(x.AccountReference) && x.PaymentStatusId == 2)
                    .GroupBy(x => x.AccountReference)
                    .Select(g => new AccountReferenceSummaryDto
                    {
                        AccountReference = g.Key,
                        TotalAmount = g.Sum(x => x.Amount),
                        TransactionCount = g.Count()
                    })
                    .OrderByDescending(x => x.TotalAmount)
                    .ToListAsync();

                return new DbResponse<List<AccountReferenceSummaryDto>> { Success = true, Data = result };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<AccountReferenceSummaryDto>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<DbResponse<List<Collection>>> GetChurchCollectionsAsync()
        {
            try
            {
                var collections = await _context.Collections
                    .Where(x => x.Paybill != "4099245" && x.PaymentStatusId == 2)
                    .OrderByDescending(x => x.TransactionDate ?? x.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<Collection>> { Success = true, Data = collections };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Collection>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<DbResponse<List<AccountReferenceSummaryDto>>> GetChurchAccountReferenceSummaryAsync()
        {
            try
            {
                var result = await _context.Collections
                    .Where(x => x.Paybill != "4099245" && !string.IsNullOrEmpty(x.AccountReference) && x.PaymentStatusId == 2)
                    .GroupBy(x => x.AccountReference)
                    .Select(g => new AccountReferenceSummaryDto
                    {
                        AccountReference = g.Key,
                        TotalAmount = g.Sum(x => x.Amount),
                        TransactionCount = g.Count()
                    })
                    .OrderByDescending(x => x.TotalAmount)
                    .ToListAsync();

                return new DbResponse<List<AccountReferenceSummaryDto>> { Success = true, Data = result };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<AccountReferenceSummaryDto>> { Success = false, Message = ex.Message };
            }
        }
    }
}