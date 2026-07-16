
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
    public class PaymentsRepository
    {
        private readonly AppDbContext _context;

        public PaymentsRepository(AppDbContext context)
        {
            _context = context;
        }

       
    
      
        public async Task<DbResponse<List<Payment>>> GetAllAsync()
        {
            try
            {
                var payments = await _context.Payments
                    .Where(x => x.PaymentStatusId == 2)
                    .OrderBy(x => x.Id)
                    .ToListAsync();

                return new DbResponse<List<Payment>>
                {
                    Success = true,
                    Data = payments
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Payment>>
                {
                    Success = false,
                    Message = $"Error fetching payments: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<Payment>>> GetByMemberIdAsync(int memberId)
        {
            try
            {
                var payments = await _context.Payments
                    .OrderBy(x => x.Id)
                    .ToListAsync();

                return new DbResponse<List<Payment>>
                {
                    Success = true,
                    Data = payments
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Payment>>
                {
                    Success = false,
                    Message = $"Error fetching member payments: {ex.Message}"
                };
            }
        }
        public async Task<DbResponse<List<AccountReferenceSummaryDto>>> GetAccountReferenceSummaryAsync()
        {
            try
            {
                var result = await _context.Payments
                    .Where(x => !string.IsNullOrEmpty(x.AccountReference)&& x.PaymentStatusId==2)
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
    }
}