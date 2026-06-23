using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Utils;

using Microsoft.EntityFrameworkCore;
using GCI_Admin.DBOperations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]

    public class AssembliesController : Controller
    {
        private readonly IAssembliesService _assembliesService;
        private readonly IMembersService _membersService;
        private readonly AppDbContext _context;

        public AssembliesController(IAssembliesService assembliesService, IMembersService membersService, AppDbContext context)
        {
            _assembliesService = assembliesService;
            _membersService = membersService;
            _context = context;
        }

        // Index view
        public async Task<IActionResult> Index()
        {
            try
            {
                AssembliesData assembliesData= new AssembliesData();
               var assemblyRes = await _assembliesService.GetAllAssembliesAsync();
                var leaderRes=await _assembliesService.GetAllAssemblyLeadersAsync();

                if (assemblyRes != null)
                {
                    assembliesData.Assembly = assemblyRes.Data;
                }
                if (leaderRes != null)
                {
                    assembliesData.AssemblyLeader = leaderRes.Data;
                }
               

                return View(assembliesData);
            }
            catch
            {
                return View(new List<Assembly>());
            }
        }

        // Partial table view
        [HttpGet]
        public async Task<IActionResult> AssembliesTable()
        {
            try
            {
                ApiResponse<List<Assembly>> response = await _assembliesService.GetAllAssembliesAsync();
                return PartialView("_AssembliesTable", response.Data ?? new List<Assembly>());
            }
            catch
            {
                return PartialView("_AssembliesTable", new List<Assembly>());
            }
        }

        // Partial leaders table view
        [HttpGet]
        public async Task<IActionResult> LeadersTable()
        {
            try
            {
                var response = await _assembliesService.GetAllAssemblyLeadersAsync();
                return PartialView("_AssemblLeadersTable", response.Data ?? new List<AssemblyLeader>());
            }
            catch
            {
                return PartialView("_AssemblLeadersTable", new List<AssemblyLeader>());
            }
        }

        // Create view
        [HttpGet]
        public IActionResult CreateAssembly()
        {
            var dto = new AssemblyDto();
            return View("_CreateAssembly", dto);
        }

        // Submit new assembly
        [HttpPost]
        public async Task<IActionResult> SubmitNewAssembly([FromBody] AssemblyDto dto)
        {
            try
            {
                ApiResponse<Assembly> response = await _assembliesService.CreateAssemblyAsync(dto);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Assembly>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }

        // Get by ID
        [HttpGet]
        public async Task<IActionResult> GetById(int assemblyId)
        {
            try
            {
                ApiResponse<Assembly> response = await _assembliesService.GetAssemblyByIdAsync(assemblyId);

                if (!response.IsSuccess)
                    return NotFound(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Assembly>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }

        // Update
        [HttpPut]
        public async Task<IActionResult> Update(int assemblyId, AssemblyDto dto)
        {
            try
            {
                ApiResponse<Assembly> response = await _assembliesService.UpdateAssemblyAsync(assemblyId, dto);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Assembly>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }

        // Delete / soft-delete
        [HttpDelete]
        public async Task<IActionResult> Delete(int assemblyId)
        {
            try
            {
                ApiResponse<bool> response = await _assembliesService.DeleteAssemblyAsync(assemblyId);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }

        // Toggle active status
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int assemblyId, bool isActive)
        {
            try
            {
                ApiResponse<bool> response = await _assembliesService.ToggleAssemblyStatusAsync(assemblyId, isActive);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                });
            }
        }
        // GET: Load Create Form
        public async Task<IActionResult> LoadCreateForm()
        {
            CreateAssemblyLeaderDto dto = new CreateAssemblyLeaderDto();

            // Get all members (active members only)
            var membersResult = await _membersService.GetAllMembersAsync();
            if (membersResult.IsSuccess && membersResult.Data != null)
            {
                dto.Members = membersResult.Data;
            }

            // Get all active assemblies
            var assembliesResult = await _assembliesService.GetAllAssembliesAsync();
            if (assembliesResult.IsSuccess && assembliesResult.Data != null)
            {
                dto.Assemblies = assembliesResult.Data.ToList();
            }

            dto.AssemblyLeader = new AssemblyLeaderDto
            {
                StartDate = DateTime.Today,
                IsActive = true
            };

            ViewBag.IsEdit = false;
            return PartialView("_CreateAssemblyLeaderPartial", dto);
        }

        // POST: Create Assembly Leader
        [HttpPost]
        public async Task<IActionResult> CreateAssemblyLeader([FromBody] AssemblyLeaderDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, message = "Please correct the validation errors.", errors = errors });
                }

                var existingLeaders = await _assembliesService.GetAllAssemblyLeadersAsync();
                if (existingLeaders.IsSuccess && existingLeaders.Data != null)
                {
                    var isExisting = existingLeaders.Data.Any(l => l.MemberId == model.MemberId && l.AssemblyId == model.AssemblyId && l.IsActive);
                    if (isExisting)
                    {
                        return Json(new { success = false, message = "This member is already an active leader in this assembly." });
                    }
                }

                var result = await _assembliesService.CreateAssemblyLeaderAsync(model);

                if (result.IsSuccess)
                    return Json(new { success = true, message = "Assembly Leader assigned successfully!", data = result.Data });
                else
                    return Json(new { success = false, message = result.Message ?? "Failed to assign assembly leader." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        // GET: Load Edit Form
        public async Task<IActionResult> LoadEditForm(int id)
        {
            try
            {
                var leaderResult = await _assembliesService.GetAssemblyLeaderByIdAsync(id);

                if (!leaderResult.IsSuccess || leaderResult.Data == null)
                    return Json(new { success = false, message = "Assembly leader not found." });

                var leader = leaderResult.Data;
                CreateAssemblyLeaderDto dto = new CreateAssemblyLeaderDto();
                AssemblyLeaderDto assemblyLeaderDto = new AssemblyLeaderDto
                {
                    AssemblyLeaderId = leader.AssemblyLeaderId,
                    MemberId = leader.MemberId,
                    AssemblyId = leader.AssemblyId,
                    Bio = leader.Bio,
                    StartDate = leader.StartDate,
                    EndDate = leader.EndDate,
                    IsActive = leader.IsActive
                };

                var membersResult = await _membersService.GetAllMembersAsync();
                var assembliesResult = await _assembliesService.GetAllAssembliesAsync();
                if (assembliesResult.IsSuccess && assembliesResult.Data != null)
                {
                    dto.Assemblies = assembliesResult.Data.ToList();
                }

                dto.AssemblyLeader = assemblyLeaderDto;
                ViewBag.IsEdit = true;
                return PartialView("_CreateAssemblyLeaderPartial", dto);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error loading form: {ex.Message}" });
            }
        }

        // POST: Update Assembly Leader
        [HttpPost]
        public async Task<IActionResult> UpdateAssemblyLeader([FromBody] AssemblyLeaderDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, message = "Please correct the validation errors.", errors = errors });
                }

                var existingLeaders = await _assembliesService.GetAllAssemblyLeadersAsync();
                if (existingLeaders.IsSuccess && existingLeaders.Data != null)
                {
                    var isExisting = existingLeaders.Data.Any(l => l.MemberId == model.MemberId
                                                                 && l.AssemblyId == model.AssemblyId
                                                                 && l.IsActive
                                                                 && l.AssemblyLeaderId != model.AssemblyLeaderId);
                    if (isExisting)
                    {
                        return Json(new { success = false, message = "This member is already an active leader in this assembly." });
                    }
                }

                var result = await _assembliesService.UpdateAssemblyLeaderAsync(model.AssemblyLeaderId, model);

                if (result.IsSuccess)
                    return Json(new { success = true, message = "Assembly Leader updated successfully!", data = result.Data });
                else
                    return Json(new { success = false, message = result.Message ?? "Failed to update assembly leader." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        // POST: Delete Assembly Leader
        [HttpPost]
        public async Task<IActionResult> DeleteAssemblyLeader(int id)
        {
            try
            {
                var result = await _assembliesService.DeleteAssemblyLeaderAsync(id);

                if (result.IsSuccess)
                    return Json(new { success = true, message = "Assembly Leader deleted successfully!" });
                else
                    return Json(new { success = false, message = result.Message ?? "Failed to delete assembly leader." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        // POST: Toggle Assembly Leader Status
        [HttpPost]
        public async Task<IActionResult> ToggleLeaderStatus(int id, bool isActive)
        {
            try
            {
                var result = await _assembliesService.ToggleAssemblyLeaderStatusAsync(id, isActive);

                if (result.IsSuccess)
                    return Json(new { success = true, message = result.Message ?? "Status toggled successfully." });
                else
                    return Json(new { success = false, message = result.Message ?? "Failed to toggle status." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        // GET: Get Members by Assembly
        [HttpGet]
        public async Task<IActionResult> GetMembersByAssembly(int assemblyId)
        {
            try
            {
                var assemblyRes = await _assembliesService.GetAssemblyByIdAsync(assemblyId);
                if (!assemblyRes.IsSuccess || assemblyRes.Data == null)
                    return BadRequest(new { success = false, message = "Assembly not found." });

                string assemblyName = assemblyRes.Data.Name;

                var members = await _context.Members
                    .Where(m => m.Assembly == assemblyName && m.StatusId == 1)
                    .Select(m => new {
                        id = m.Id,
                        firstName = m.FirstName,
                        otherNames = m.OtherNames,
                        email = m.Email,
                        phone = m.Phone,
                        gender = m.Gender
                    })
                    .ToListAsync();
                return Ok(new { success = true, data = members });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
