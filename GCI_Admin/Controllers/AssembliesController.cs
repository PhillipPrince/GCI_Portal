using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Utils;

namespace GCI_Admin.Controllers
{
    public class AssembliesController : Controller
    {
        private readonly IAssembliesService _assembliesService;

        public AssembliesController(IAssembliesService assembliesService)
        {
            _assembliesService = assembliesService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ApiResponse<List<Assembly>> response = await _assembliesService.GetAllAssembliesAsync();
                if (!response.IsSuccess)
                    return View(new List<Assembly>());

                return View(response.Data);
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

        // Create view
        [HttpGet]
        public IActionResult CreateAssembly()
        {
            var dto = new AssemblyDto();
            return View("_CreateAssembly", dto);
        }

        // Submit new assembly
        [HttpPost]
        public async Task<IActionResult> SubmitNewAssembly(AssemblyDto dto)
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
    }
}
