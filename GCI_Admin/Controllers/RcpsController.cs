using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Text;
using Utils;
using Microsoft.EntityFrameworkCore;

namespace GCI_Admin.Controllers
{
    public class RcpsController : Controller
    {
        private readonly IRcpsService _rcpsService;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IMembersService _membersService;
        private readonly GCI_Admin.DBOperations.AppDbContext _context;

        public RcpsController(IRcpsService rcpsService, ICompositeViewEngine viewEngine, IMembersService membersService, GCI_Admin.DBOperations.AppDbContext context)
        {
            _rcpsService = rcpsService;
            _viewEngine = viewEngine;
            _membersService = membersService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                RcpsViewModel rcpsViewModel = new RcpsViewModel();

                var rcpsRes = await _rcpsService.GetAllRcpsAsync();

                if (rcpsRes != null && rcpsRes.Data != null)
                {
                    rcpsViewModel.Rcps = rcpsRes.Data;

                    // Calculate stats
                    rcpsViewModel.TotalRcps = rcpsRes.Data.Count;
                    rcpsViewModel.ActiveRcps = rcpsRes.Data.Count(r => r.IsActive && r.Status == "Ongoing");
                    rcpsViewModel.CompletedRcps = rcpsRes.Data.Count(r => r.Status == "Completed");
                    rcpsViewModel.TotalRaised = rcpsRes.Data.Sum(r => r.AmountRaised);
                    rcpsViewModel.CurrentActiveRcps = rcpsRes.Data.FirstOrDefault(r => r.Status == "Ongoing" && r.IsActive);
                }
                else
                {
                    rcpsViewModel.Rcps = new List<Rcps>();
                }

                var coordinatorsRes = await _rcpsService.GetAllRcpsCountyCoordinatorsAsync();
                if (coordinatorsRes != null && coordinatorsRes.IsSuccess)
                {
                    rcpsViewModel.CountyCoordinators = coordinatorsRes.Data;
                }
                else
                {
                    rcpsViewModel.CountyCoordinators = new List<RcpsCountyCoordinator>();
                }
                var countiesRes = await _rcpsService.GetAllCountiesAsync();
                if (countiesRes != null && countiesRes.IsSuccess)
                {
                    rcpsViewModel.Counties = countiesRes.Data.Select(c => new SelectListItem
                    {
                        Value = c.CountyCode,
                        Text = c.CountyName
                    }).ToList();
                }
                else
                {
                    rcpsViewModel.Counties = new List<SelectListItem>();
                }

                return View(rcpsViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View(new RcpsViewModel());
            }
        }

        public async Task<IActionResult> RcpsDetails(int id)
        {
            try
            {
                RcpsDetailsViewModel rcpsDetailsViewModel = new RcpsDetailsViewModel();

                var rcpsRes = await _rcpsService.GetRcpsByIdAsync(id);

                if (rcpsRes != null && rcpsRes.Data != null)
                {
                    rcpsDetailsViewModel.Rcps = rcpsRes.Data;

                    // Get pledges for this RCP
                    var pledgesRes = await _rcpsService.GetPledgesByRcpsIdAsync(id);

                    if (pledgesRes != null && pledgesRes.Data != null)
                    {
                        rcpsDetailsViewModel.Pledges = pledgesRes.Data;

                        rcpsDetailsViewModel.TotalPledges = pledgesRes.Data.Count;
                        rcpsDetailsViewModel.TotalPledgedAmount = pledgesRes.Data.Sum(p => p.PledgedAmount);
                        rcpsDetailsViewModel.TotalPaidAmount = pledgesRes.Data.Sum(p => p.AmountPaid);
                        rcpsDetailsViewModel.ActivePledges = pledgesRes.Data.Count(p => p.Status == "Active" && p.Balance > 0);
                    }
                    else
                    {
                        rcpsDetailsViewModel.Pledges = new List<RcpsPledges>();
                        rcpsDetailsViewModel.TotalPledges = 0;
                        rcpsDetailsViewModel.TotalPledgedAmount = 0;
                        rcpsDetailsViewModel.TotalPaidAmount = 0;
                        rcpsDetailsViewModel.ActivePledges = 0;
                    }
                }
                else
                {
                    return NotFound($"RCP with ID {id} not found");
                }

                var countyMembersResult = await _rcpsService.GetRcpCountyMembersByRcpsAsync(id);
                var countyMembers = countyMembersResult.IsSuccess && countyMembersResult.Data != null 
                    ? countyMembersResult.Data 
                    : new List<RcpCountyMember>();

                rcpsDetailsViewModel.CountyMembers = countyMembers;
                rcpsDetailsViewModel.CountyCoordinators = countyMembers
                    .Where(m => m.IsLeader)
                    .Select(m => new RcpsCountyCoordinator { MemberId = m.MemberId })
                    .ToList(); // this is just a quick projection if needed, but I'll use CountyMembers directly.

                return View(rcpsDetailsViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View(new RcpsDetailsViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(int rcpsId, int memberId)
        {
            var response = await _rcpsService.AddMemberToRcpCountyAsync(rcpsId, memberId);
            return Json(new { success = response.IsSuccess, message = response.Message });
        }


        public async Task<ActionResult> RcpsPledges(int? id)
        {
            var model = new RcpsPledgesViewModel();

            // Load RCPs
            var rcpsResult = await _rcpsService.GetAllRcpsAsync();
            var rcpsList = rcpsResult.Data ?? new List<Rcps>();

            // Get all active RCPs
            var activeRcps = rcpsList.Where(r => r.IsActive && r.Status == "Ongoing").ToList();
            var defaultRcp = activeRcps.FirstOrDefault();

            // If no id provided but there's an active RCP, use the first active one
            if (!id.HasValue && defaultRcp != null)
            {
                id = defaultRcp.Id;
            }

            model.RcpsList = rcpsList.Select(x => new DropdownItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name} ({x.Status})",
                Subtext = $"KES {x.AmountRaised:N0} raised",
                Selected = id.HasValue && id.Value == x.Id,
                Icon = x.Status == "Ongoing" ? "fa fa-play-circle" : (x.Status == "Completed" ? "fa fa-check-circle" : "fa fa-pause-circle"),
                ExtraData = new Dictionary<string, object>
                {
                    ["amountRaised"] = x.AmountRaised,
                    ["targetAmount"] = x.TargetAmount,
                    ["status"] = x.Status,
                    ["startDate"] = x.StartDate?.ToString("yyyy-MM-dd"),
                    ["endDate"] = x.EndDate?.ToString("yyyy-MM-dd")
                }
            }).ToList();


            model.SelectedRcpsId = id;


            // Load pledges and stats
            await LoadPledgesData(model, id);

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetRcpData(int rcpId)
        {
            try
            {
                var model = new RcpsPledgesViewModel();
                await LoadPledgesData(model, rcpId);
                model.SelectedRcpsId = rcpId;

                var data = new
                {
                    statsCardsHtml = await RenderPartialViewToString("_StatsCards", model),
                    rcpSpecificStatsHtml = await RenderPartialViewToString("_RcpSpecificStats", model),
                    pledgesTableHtml = await RenderPartialViewToString("_PledgesTable", model),
                    selectedRcpId = rcpId,
                    selectedRcpName = model.SelectedRcpsName
                };

                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAllData()
        {
            try
            {
                var model = new RcpsPledgesViewModel();
                await LoadAllPledgesData(model);

                var data = new
                {
                    statsCardsHtml = await RenderPartialViewToString("_StatsCards", model),
                    rcpSpecificStatsHtml = (string)null,
                    pledgesTableHtml = await RenderPartialViewToString("_PledgesTable", model),
                    selectedRcpId = 0,
                    selectedRcpName = ""
                };

                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
      
        [HttpGet]
        public async Task<IActionResult> RecordPaymentPartial(int pledgeId)
        {
            var pledge = await _rcpsService.GetRcpsPledgeByIdAsync(pledgeId);
            if (pledge?.Data == null)
                return NotFound();

            return PartialView("_RecordPaymentModal", pledge.Data);
        }

      

        private async Task LoadPledgesData(RcpsPledgesViewModel model, int? rcpId)
        {
            if (rcpId.HasValue)
            {
                var pledgesResult = await _rcpsService.GetPledgesByRcpsIdAsync(rcpId.Value);
                var pledges = pledgesResult.Data ?? new List<RcpsPledges>();
                var rcpsResult = await _rcpsService.GetRcpsByIdAsync(rcpId.Value);
                var rcp = rcpsResult.Data;

                model.Pledges = pledges;
                model.SelectedRcpsName = rcp?.Name ?? "Selected Rcps";
                model.TotalPledges = pledges.Count;
                model.TotalPledgedAmount = pledges.Sum(x => x.PledgedAmount);
                model.TotalPaidAmount = pledges.Sum(x => x.AmountPaid);
                model.ActivePledges = pledges.Count(x => x.Balance > 0 && x.Status != "Completed");

                model.RcpsStats = new RcpsStatisticsViewModel
                {
                    PledgeCount = pledges.Count,
                    TotalPledged = pledges.Sum(x => x.PledgedAmount),
                    TotalRedeemed = pledges.Sum(x => x.AmountPaid),
                    RcpsTarget = rcp.TargetAmount ,
                    FulfillmentRate = pledges.Sum(x => x.PledgedAmount) > 0 ? (pledges.Sum(x => x.AmountPaid) / pledges.Sum(x => x.PledgedAmount)) * 100 : 0,
                    PercentageOfTarget = (rcp?.TargetAmount ) > 0 ? (rcp?.AmountRaised ) / rcp.TargetAmount * 100 : 0,
                    ActivePledgesCount = pledges.Count(x => x.Balance > 0 && x.Status != "Completed"),
                    FulfilledPledgesCount = pledges.Count(x => x.Balance == 0 && x.PaymentRecieved),
                    OverduePledgesCount = pledges.Count(x => x.TargetCompletionDate < DateTime.Now && x.Balance > 0),
                    PartiallyPaidPledgesCount = pledges.Count(x => x.AmountPaid > 0 && x.AmountPaid < x.PledgedAmount),
                    PendingPledgesCount = pledges.Count(x => x.AmountPaid == 0)
                };
            }
            else
            {
                await LoadAllPledgesData(model);
            }
        }

        private async Task LoadAllPledgesData(RcpsPledgesViewModel model)
        {
            var allRcps = await _rcpsService.GetAllRcpsAsync();
            var allPledges = new List<RcpsPledges>();

            foreach (var rcp in allRcps.Data)
            {
                var pledgesResult = await _rcpsService.GetPledgesByRcpsIdAsync(rcp.Id);
                if (pledgesResult.Data != null)
                    allPledges.AddRange(pledgesResult.Data);
            }

            model.Pledges = allPledges;
            model.SelectedRcpsName = "All Rcpss";
            model.TotalPledges = allPledges.Count;
            model.TotalPledgedAmount = allPledges.Sum(x => x.PledgedAmount);
            model.TotalPaidAmount = allPledges.Sum(x => x.AmountPaid);
            model.ActivePledges = allPledges.Count(x => x.Balance > 0 );
        }

        

        private async Task<string> RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = this.ControllerContext.ActionDescriptor.ActionName;

            this.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(this.ControllerContext, viewName, false);
                var viewContext = new ViewContext(
                    this.ControllerContext,
                    viewResult.View,
                    this.ViewData,
                    this.TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAddRcpsPartial()
        {
            var model = new RcpsDto
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(3),
                AmountRaised = 0,
                Status = "Planning"
            };

            var countiesRes = await _rcpsService.GetAllCountiesAsync();
            if (countiesRes != null && countiesRes.IsSuccess)
            {
                ViewBag.Counties = countiesRes.Data.Select(c => new SelectListItem
                {
                    Value = c.CountyCode,
                    Text = c.CountyName
                }).ToList();
            }
            else
            {
                ViewBag.Counties = new List<SelectListItem>();
            }

            return PartialView("_AddRcpsModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CreateRcps([FromBody] RcpsDto model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new { success = false, message = "Invalid data received" });
                }

                // Validate
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    return Json(new { success = false, message = "Rcps name is required" });
                }

                if (model.TargetAmount <= 0)
                {
                    return Json(new { success = false, message = "Target amount must be greater than zero" });
                }

                if (model.EndDate < model.StartDate)
                {
                    return Json(new { success = false, message = "End date cannot be earlier than start date" });
                }

              

                // Call service to create RCP
                var result = await _rcpsService.CreateRcpsAsync(model);

                if (result != null && result.IsSuccess)
                {
                    return Json(new { success = true, message = "Rcps created successfully!", data = result.Data });
                }
                else
                {
                    return Json(new { success = false, message = result?.Message ?? "Failed to create Rcps" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEditRcpsPartial(int id)
        {
            var res = await _rcpsService.GetRcpsByIdAsync(id);
            if (res != null && res.IsSuccess && res.Data != null)
            {
                var rcps = res.Data;
                var model = new RcpsDto
                {
                    Id = rcps.Id,
                    Name = rcps.Name,
                    Description = rcps.Description,
                    TargetAmount = rcps.TargetAmount,
                    AmountRaised = rcps.AmountRaised,
                    StartDate = rcps.StartDate ?? DateTime.Now,
                    EndDate = rcps.EndDate ?? DateTime.Now.AddMonths(3),
                    Status = rcps.Status,
                    CountyCode = rcps.CountyCode
                };

                var countiesRes = await _rcpsService.GetAllCountiesAsync();
                if (countiesRes != null && countiesRes.IsSuccess)
                {
                    ViewBag.Counties = countiesRes.Data.Select(c => new SelectListItem
                    {
                        Value = c.CountyCode,
                        Text = c.CountyName
                    }).ToList();
                }
                else
                {
                    ViewBag.Counties = new List<SelectListItem>();
                }
                
                ViewBag.IsEdit = true;
                return PartialView("_AddRcpsModal", model);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateRcps([FromBody] RcpsDto model)
        {
            try
            {
                if (model == null) return Json(new { success = false, message = "Invalid data received" });
                if (string.IsNullOrWhiteSpace(model.Name)) return Json(new { success = false, message = "Rcps name is required" });

                var res = await _rcpsService.GetRcpsByIdAsync(model.Id);
                if (res != null && res.IsSuccess && res.Data != null)
                {
                    var rcps = res.Data;
                    rcps.Name = model.Name;
                    rcps.Description = model.Description;
                    rcps.TargetAmount = model.TargetAmount;
                    rcps.AmountRaised = model.AmountRaised;
                    rcps.StartDate = model.StartDate;
                    rcps.EndDate = model.EndDate;
                    rcps.Status = model.Status;
                    rcps.CountyCode = model.CountyCode;
                    
                    var updateRes = await _rcpsService.UpdateRcpsAsync(rcps);
                    if (updateRes != null && updateRes.IsSuccess)
                    {
                        return Json(new { success = true, message = "Rcps updated successfully!", data = updateRes.Data });
                    }
                    return Json(new { success = false, message = updateRes?.Message ?? "Failed to update Rcps" });
                }
                return Json(new { success = false, message = "Rcps not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteRcp(int id)
        {
            try
            {
                var result = await _rcpsService.DeleteRcpsAsync(id);
                if (result.IsSuccess)
                {
                    return Json(new { success = true, message = "Rcps deleted successfully" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message ?? "Failed to delete Rcps" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetStats()
        {
            try
            {
                var rcpsRes = await _rcpsService.GetAllRcpsAsync();
                var rcps = rcpsRes.Data ?? new List<Rcps>();

                return Json(new
                {
                    totalRCPs = rcps.Count,
                    activeRCPs = rcps.Count(r => r.IsActive && r.Status == "Ongoing"),
                    completedRCPs = rcps.Count(r => r.Status == "Completed")
                });
            }
            catch (Exception ex)
            {
                return Json(new { totalRCPs = 0, activeRCPs = 0, completedRCPs = 0 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> FilterRCPs(string search, string status)
        {
            try
            {
                var rcpsRes = await _rcpsService.GetAllRcpsAsync();
                var rcps = rcpsRes.Data ?? new List<Rcps>();

                if (!string.IsNullOrEmpty(search))
                {
                    rcps = rcps.Where(r =>
                        r.Name.ToLower().Contains(search.ToLower()) ||
                        (r.Description != null && r.Description.ToLower().Contains(search.ToLower()))
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(status))
                {
                    rcps = rcps.Where(r => r.Status == status).ToList();
                }

                return PartialView("_RcpsTable", rcps);
            }
            catch (Exception ex)
            {
                return PartialView("_RcpsTable", new List<Rcps>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRCPs()
        {
            try
            {
                var rcpsRes = await _rcpsService.GetAllRcpsAsync();
                var rcps = rcpsRes.Data ?? new List<Rcps>();
                return PartialView("_RcpsTable", rcps);
            }
            catch (Exception ex)
            {
                return PartialView("_RcpsTable", new List<Rcps>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportRCPs(string search, string status)
        {
            try
            {
                var rcpsRes = await _rcpsService.GetAllRcpsAsync();
                var rcps = rcpsRes.Data ?? new List<Rcps>();

                if (!string.IsNullOrEmpty(search))
                {
                    rcps = rcps.Where(r =>
                        r.Name.ToLower().Contains(search.ToLower()) ||
                        (r.Description != null && r.Description.ToLower().Contains(search.ToLower()))
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(status))
                {
                    rcps = rcps.Where(r => r.Status == status).ToList();
                }

                var sb = new StringBuilder();
                sb.AppendLine("ID,Name,Description,Target Amount,Amount Raised,Start Date,End Date,Status,Is Active,Created At");

                foreach (var rcp in rcps)
                {
                    sb.AppendLine($"\"{rcp.Id}\",\"{rcp.Name}\",\"{rcp.Description?.Replace("\"", "\"\"")}\",{rcp.TargetAmount},{rcp.AmountRaised},{rcp.StartDate:yyyy-MM-dd},{rcp.EndDate:yyyy-MM-dd},\"{rcp.Status}\",{rcp.IsActive},{rcp.CreatedAt:yyyy-MM-dd}");
                }

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                return File(bytes, "text/csv", $"RCPs_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                return File(Encoding.UTF8.GetBytes("Error generating export"), "text/csv", "error.csv");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ToggleRcpStatus(int id, bool isActive)
        {
            try
            {
                // Get the RCP
                var rcpsResult = await _rcpsService.GetRcpsByIdAsync(id);

                if (rcpsResult?.Data == null)
                {
                    return Json(new { success = false, message = "RCP campaign not found" });
                }

                var rcp = rcpsResult.Data;
                string oldStatus = rcp.IsActive ? "Active" : "Inactive";
                string newStatus = isActive ? "Active" : "Inactive";

                rcp.IsActive = isActive;

                if (isActive && rcp.Status != "Ongoing")
                {
                    rcp.Status = "Ongoing";
                }
                else if (!isActive && rcp.Status == "Ongoing")
                {
                    rcp.Status = "On Hold";
                }

                rcp.UpdatedAt = DateTime.Now;

                var updateResult = await _rcpsService.UpdateRcpsAsync(rcp);

                if (updateResult.IsSuccess)
                {
                    Loggers.EventLogs("RCP Status Change: " + $"RCP ID {id} changed from {oldStatus} to {newStatus}");

                    return Json(new
                    {
                        success = true,
                        message = $"Campaign has been {(isActive ? "activated" : "deactivated")} successfully.",
                        data = new
                        {
                            isActive = isActive,
                            status = rcp.Status,
                            updatedAt = rcp.UpdatedAt
                        }
                    });
                }
                else
                {
                    return Json(new { success = false, message = updateResult.Message ?? "Failed to update campaign status" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // =========================================================
        // COUNTY COORDINATORS
        // =========================================================

        public async Task<IActionResult> LoadCoordinatorCreateForm()
        {
            CreateRcpsCountyCoordinatorDto dto = new CreateRcpsCountyCoordinatorDto();

            var membersResult = await _membersService.GetAllMembersAsync();
            if (membersResult.IsSuccess && membersResult.Data != null)
            {
                dto.Members = membersResult.Data;
            }

            var rcpsResult = await _rcpsService.GetAllRcpsAsync();
            if (rcpsResult.IsSuccess && rcpsResult.Data != null)
            {
                dto.RcpsList = rcpsResult.Data.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList();
            }

            dto.Coordinator = new RcpsCountyCoordinatorDto
            {
                IsActive = true
            };

            ViewBag.IsEdit = false;
            return PartialView("_CreateCountyCoordinatorPartial", dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCountyCoordinator([FromBody] RcpsCountyCoordinatorDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, message = "Validation errors.", errors = errors });
                }

                var existing = await _rcpsService.GetRcpsCountyCoordinatorsByRcpsAsync(model.RcpsId);
                if (existing.IsSuccess && existing.Data != null)
                {
                    if (existing.Data.Any(c => c.MemberId == model.MemberId && c.IsActive))
                    {
                        return Json(new { success = false, message = "Member is already an active coordinator for this RCP." });
                    }
                }

                var result = await _rcpsService.CreateRcpsCountyCoordinatorAsync(model);

                if (result.IsSuccess)
                {
                    return Json(new { success = true, message = result.Message, data = result.Data });
                }
                return Json(new { success = false, message = result.Message ?? "Failed to create coordinator." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> LoadCoordinatorEditForm(int id)
        {
            try
            {
                var coordResult = await _rcpsService.GetRcpsCountyCoordinatorByIdAsync(id);
                if (!coordResult.IsSuccess || coordResult.Data == null)
                {
                    return Json(new { success = false, message = "Coordinator not found." });
                }

                var leader = coordResult.Data;
                CreateRcpsCountyCoordinatorDto dto = new CreateRcpsCountyCoordinatorDto();
                RcpsCountyCoordinatorDto rcpsDto = new RcpsCountyCoordinatorDto
                {
                    Id = leader.RcpsCountyCoordinatorId,
                    MemberId = leader.MemberId,
                    RcpsId = leader.RcpsId,
                    Bio = leader.Bio,
                    IsActive = leader.IsActive
                };

                var membersResult = await _membersService.GetAllMembersAsync();
                var rcpsResult = await _rcpsService.GetAllRcpsAsync();
                if (rcpsResult.IsSuccess && rcpsResult.Data != null)
                {
                    dto.RcpsList = rcpsResult.Data.Where(r => r.IsActive).Select(r => new SelectListItem
                    {
                        Value = r.Id.ToString(),
                        Text = r.Name,
                        Selected = (r.Id == leader.RcpsId)
                    }).ToList();
                }

                dto.Coordinator = rcpsDto;
                dto.Members = membersResult.IsSuccess ? membersResult.Data : new List<Member>();
                
                ViewBag.IsEdit = true;
                return PartialView("_CreateCountyCoordinatorPartial", dto);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCountyCoordinator([FromBody] RcpsCountyCoordinatorDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, message = "Validation errors.", errors = errors });
                }

                var result = await _rcpsService.UpdateRcpsCountyCoordinatorAsync(model);

                if (result.IsSuccess)
                {
                    return Json(new { success = true, message = result.Message });
                }
                return Json(new { success = false, message = result.Message ?? "Failed to update coordinator." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCountyCoordinator(int id)
        {
            try
            {
                var result = await _rcpsService.DeleteRcpsCountyCoordinatorAsync(id);
                return Json(new { success = result.IsSuccess, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleCoordinatorStatus(int id, bool isActive)
        {
            try
            {
                var result = await _rcpsService.ToggleCountyCoordinatorStatusAsync(id, isActive);
                return Json(new { success = result.IsSuccess, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}