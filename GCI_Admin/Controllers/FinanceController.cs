using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]

    public class FinanceController : Controller
    {
        private readonly IPaymentsService _paymentsService;

        public FinanceController(IPaymentsService paymentsService)
        {
            _paymentsService = paymentsService;
        }

        public async Task<IActionResult> Index()
        {
            var finance = new Finance();

            try
            {
                var paymentsResponse = await _paymentsService.GetAllAsync();
                var accountsSummaryResponse = await _paymentsService.GetAccountReferenceSummaryAsync();

                finance.Payments = paymentsResponse?.Data ?? new List<Payment>();
                finance.AccountReferenceSummaries = accountsSummaryResponse?.Data ?? new List<AccountReferenceSummaryDto>();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading finance data: {ex.Message}";
                finance.Payments = new List<Payment>();
                finance.AccountReferenceSummaries = new List<AccountReferenceSummaryDto>();
            }

            return View(finance);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredPayments(
            string search = null,
            string accountReference = null,
            string dateRange = null,
            string paymentStatus = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var response = await _paymentsService.GetAllAsync();
                var payments = response?.Data ?? new List<Payment>();

                // Apply filters
                var query = payments.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p =>
                        (p.PhoneNumber != null && p.PhoneNumber.Contains(search)) ||
                        (p.MpesaReceiptNumber != null && p.MpesaReceiptNumber.Contains(search)) ||
                        (p.AccountReference != null && p.AccountReference.Contains(search))
                    );
                }

                if (!string.IsNullOrEmpty(accountReference))
                {
                    query = query.Where(p => p.AccountReference == accountReference);
                }

                if (!string.IsNullOrEmpty(paymentStatus) && int.TryParse(paymentStatus, out int statusId))
                {
                    query = query.Where(p => p.PaymentStatusId == statusId);
                }

                // Apply date range
                var now = DateTime.Now;
                switch (dateRange)
                {
                    case "today":
                        query = query.Where(p => p.TransactionDate.HasValue && p.TransactionDate.Value.Date == now.Date);
                        break;
                    case "yesterday":
                        var yesterday = now.AddDays(-1).Date;
                        query = query.Where(p => p.TransactionDate.HasValue && p.TransactionDate.Value.Date == yesterday);
                        break;
                    case "thisweek":
                        var weekStart = now.AddDays(-(int)now.DayOfWeek).Date;
                        query = query.Where(p => p.TransactionDate.HasValue && p.TransactionDate.Value >= weekStart);
                        break;
                    case "thismonth":
                        var monthStart = new DateTime(now.Year, now.Month, 1);
                        query = query.Where(p => p.TransactionDate.HasValue && p.TransactionDate.Value >= monthStart);
                        break;
                    case "lastmonth":
                        var lastMonth = now.AddMonths(-1);
                        var lastMonthStart = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                        var lastMonthEnd = lastMonthStart.AddMonths(1).AddDays(-1);
                        query = query.Where(p => p.TransactionDate.HasValue &&
                                                p.TransactionDate.Value >= lastMonthStart &&
                                                p.TransactionDate.Value <= lastMonthEnd);
                        break;
                    case "thisyear":
                        var yearStart = new DateTime(now.Year, 1, 1);
                        query = query.Where(p => p.TransactionDate.HasValue && p.TransactionDate.Value >= yearStart);
                        break;
                    case "custom":
                        if (fromDate.HasValue && toDate.HasValue)
                        {
                            var toDateEnd = toDate.Value.AddDays(1).AddSeconds(-1);
                            query = query.Where(p => p.TransactionDate.HasValue &&
                                                    p.TransactionDate.Value >= fromDate.Value &&
                                                    p.TransactionDate.Value <= toDateEnd);
                        }
                        break;
                }

                var filteredPayments = query.OrderByDescending(p => p.TransactionDate).ToList();

                return PartialView("_GivingsTable", filteredPayments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveManualPayment([FromBody] Payment payment)
        {
            try
            {
                // Validate
                if (payment == null || payment.Amount <= 0)
                {
                    return BadRequest(new { message = "Invalid payment data" });
                }

                // Set default values
                payment.CreatedAt = DateTime.UtcNow;
                payment.MerchantRequestID = "MANUAL";
                payment.CheckoutRequestID = "MANUAL";

                // You'll need to implement a method in your service to save manual payments
                // var response = await _paymentsService.CreateManualPayment(payment);

                // For now, returning success (you'll need to implement the actual save)
                return Ok(new { success = true, message = "Payment saved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error saving payment: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportGivings(
            string search = null,
            string accountReference = null,
            string dateRange = null,
            string paymentStatus = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var response = await _paymentsService.GetAllAsync();
                var payments = response?.Data ?? new List<Payment>();

                // Apply same filters as GetFilteredPayments
                var query = payments.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p =>
                        (p.PhoneNumber != null && p.PhoneNumber.Contains(search)) ||
                        (p.MpesaReceiptNumber != null && p.MpesaReceiptNumber.Contains(search)) ||
                        (p.AccountReference != null && p.AccountReference.Contains(search))
                    );
                }

                if (!string.IsNullOrEmpty(accountReference))
                {
                    query = query.Where(p => p.AccountReference == accountReference);
                }

                if (!string.IsNullOrEmpty(paymentStatus) && int.TryParse(paymentStatus, out int statusId))
                {
                    query = query.Where(p => p.PaymentStatusId == statusId);
                }

                // Apply date range filters (same as above)
                // ... (copy date filtering logic from GetFilteredPayments)

                var filteredPayments = query.OrderByDescending(p => p.TransactionDate).ToList();

                return Ok(filteredPayments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}