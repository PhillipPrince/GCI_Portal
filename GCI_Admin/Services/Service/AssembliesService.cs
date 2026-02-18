using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class AssembliesService : IAssembliesService
    {
        private readonly AssembliesRepository _assembliesRepository;
        private readonly AppDbContext _context;

        public AssembliesService(AssembliesRepository assembliesRepository, AppDbContext context)
        {
            _assembliesRepository = assembliesRepository;
            _context = context;
        }

        // ✅ CREATE ASSEMBLY
        public async Task<ApiResponse<Assembly>> CreateAssemblyAsync(AssemblyDto dto)
        {
            var response = new ApiResponse<Assembly>();

            try
            {
                var result = await _assembliesRepository.CreateAssemblyAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to create assembly";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Assembly created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET ALL ASSEMBLIES
        public async Task<ApiResponse<List<Assembly>>> GetAllAssembliesAsync()
        {
            var response = new ApiResponse<List<Assembly>>();

            try
            {
                var result = await _assembliesRepository.GetAllAssembliesAsync();

                response.Data = result.Data;
                response.Message = "Assemblies retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET ASSEMBLY BY ID
        public async Task<ApiResponse<Assembly>> GetAssemblyByIdAsync(int assemblyId)
        {
            var response = new ApiResponse<Assembly>();

            try
            {
                var result = await _assembliesRepository.GetAssemblyByIdAsync(assemblyId);

                if (result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Assembly not found";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Assembly retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ UPDATE ASSEMBLY
        public async Task<ApiResponse<Assembly>> UpdateAssemblyAsync(int assemblyId, AssemblyDto dto)
        {
            var response = new ApiResponse<Assembly>();

            try
            {
                var result = await _assembliesRepository.UpdateAssemblyAsync(assemblyId, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Assembly not found or update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Assembly updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ DELETE ASSEMBLY (soft-delete)
        public async Task<ApiResponse<bool>> DeleteAssemblyAsync(int assemblyId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _assembliesRepository.DeleteAssemblyAsync(assemblyId);

                if (!result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Assembly not found or delete failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = result.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ TOGGLE ACTIVE STATUS
        public async Task<ApiResponse<bool>> ToggleAssemblyStatusAsync(int assemblyId, bool isActive)
        {
            var assembly = await _context.Assemblies.FindAsync(assemblyId);

            if (assembly == null)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "404",
                    Message = "Assembly not found"
                };
            }

            //assembly.IsActive = isActive;
            //assembly.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                IsSuccess = true,
                Code = "200",
                Message = isActive ? "Assembly activated successfully." : "Assembly deactivated successfully.",
                Data = true
            };
        }
    }
}
