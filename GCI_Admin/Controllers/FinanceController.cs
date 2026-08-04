using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.Utils;
using Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]
    [PermissionAuthorize("VIEW_FINANCE")]
    public class FinanceController : Controller
    {
        private readonly ICollectionsService _collectionsService;

        public FinanceController(ICollectionsService collectionsService)
        {
            _collectionsService = collectionsService;
        }

        public async Task<IActionResult> Index()
        {
            var sessionManager = HttpContext.RequestServices.GetService(typeof(SessionManager)) as SessionManager;
            var currentUser = sessionManager?.GetUserSession<Member>();
            if (currentUser == null || currentUser.UserRole != 6)
            {
                return RedirectToAction("UnauthorizedAccess", "Auth");
            }

            var finance = new Finance();

            try
            {
                var collectionsTask = _collectionsService.GetChurchCollectionsAsync();
                var accountsSummaryTask = _collectionsService.GetChurchAccountReferenceSummaryAsync();
                var meetingsTask = _collectionsService.GetActiveMeetingsAsync();

                await Task.WhenAll(collectionsTask, accountsSummaryTask, meetingsTask);

                var collectionsResponse = await collectionsTask;
                var accountsSummaryResponse = await accountsSummaryTask;
                var meetingsResponse = await meetingsTask;

                finance.Collections = collectionsResponse?.Data ?? new List<Collection>();
                finance.AccountReferenceSummaries = accountsSummaryResponse?.Data ?? new List<AccountReferenceSummaryDto>();

                ViewBag.Members = new List<Member>();
                ViewBag.Meetings = meetingsResponse?.Data ?? new List<MeetingAttendance>();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading church finance data: {ex.Message}";
                finance.Collections = new List<Collection>();
                finance.AccountReferenceSummaries = new List<AccountReferenceSummaryDto>();
                ViewBag.Members = new List<Member>();
                ViewBag.Meetings = new List<MeetingAttendance>();
            }

            return View(finance);
        }

        public async Task<IActionResult> GBICollections()
        {
            var sessionManager = HttpContext.RequestServices.GetService(typeof(SessionManager)) as SessionManager;
            var currentUser = sessionManager?.GetUserSession<Member>();
            if (currentUser == null || currentUser.UserRole != 6)
            {
                return RedirectToAction("UnauthorizedAccess", "Auth");
            }

            var finance = new Finance();

            try
            {
                var collectionsTask = _collectionsService.GetGBICollectionsAsync();
                var accountsSummaryTask = _collectionsService.GetGBIAccountReferenceSummaryAsync();
                var meetingsTask = _collectionsService.GetActiveMeetingsAsync();

                await Task.WhenAll(collectionsTask, accountsSummaryTask, meetingsTask);

                var collectionsResponse = await collectionsTask;
                var accountsSummaryResponse = await accountsSummaryTask;
                var meetingsResponse = await meetingsTask;

                finance.Collections = collectionsResponse?.Data ?? new List<Collection>();
                finance.AccountReferenceSummaries = accountsSummaryResponse?.Data ?? new List<AccountReferenceSummaryDto>();

                ViewBag.Members = new List<Member>();
                ViewBag.Meetings = meetingsResponse?.Data ?? new List<MeetingAttendance>();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading GBI finance data: {ex.Message}";
                finance.Collections = new List<Collection>();
                finance.AccountReferenceSummaries = new List<AccountReferenceSummaryDto>();
                ViewBag.Members = new List<Member>();
                ViewBag.Meetings = new List<MeetingAttendance>();
            }

            return View(finance);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredCollections(
            string search = null,
            string accountReference = null,
            string dateRange = null,
            string PaymentStatus = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? filterYear = null,
            int? filterMonth = null,
            string paybill = null)
        {
            try
            {
                var response = await _collectionsService.GetFilteredCollectionsAsync(
                    search, accountReference, dateRange, PaymentStatus, fromDate, toDate, filterYear, filterMonth, paybill);

                var filteredCollections = response?.Data ?? new List<Collection>();

                return PartialView("_GivingsTablePartial", filteredCollections);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveMembers()
        {
            try
            {
                var cachedMembersJson = HttpContext.Session.GetString("ActiveMembers");
                if (!string.IsNullOrEmpty(cachedMembersJson))
                {
                    return Content("{\"success\":true,\"data\":" + cachedMembersJson + "}", "application/json");
                }

                var activeMembersResponse = await _collectionsService.GetActiveMembersDtoAsync();
                var activeMembers = activeMembersResponse?.Data ?? new List<object>();

                var jsonStr = JsonSerializer.Serialize(activeMembers);
                HttpContext.Session.SetString("ActiveMembers", jsonStr);

                return Content("{\"success\":true,\"data\":" + jsonStr + "}", "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveManualCollection([FromBody] Collection collection)
        {
            try
            {
                var response = await _collectionsService.SaveManualCollectionAsync(collection);
                if (response.IsSuccess)
                {
                    return Ok(new { success = true, message = response.Message ?? "Collection saved successfully" });
                }

                return StatusCode(500, new { message = response.Message ?? "Error saving Collection" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error saving Collection: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            try
            {
                var response = await _collectionsService.SendOtpAsync(request);
                return Ok(new { isSuccess = response.IsSuccess, message = response.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { isSuccess = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCollection([FromBody] VerifyCollectionRequest request)
        {
            try
            {
                var response = await _collectionsService.VerifyCollection(request);
                return Ok(new { isSuccess = response.IsSuccess, message = response.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { isSuccess = false, message = ex.Message });
            }
        }
    }
}