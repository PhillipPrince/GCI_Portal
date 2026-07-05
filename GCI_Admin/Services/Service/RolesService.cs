using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class RolesService : IRolesService
    {
        private readonly RolesRepository _rolesRepository;
        private readonly AppDbContext _context;

        public RolesService(RolesRepository rolesRepository, AppDbContext context)
        {
            _rolesRepository = rolesRepository;
            _context = context;
        }

        public async Task<ApiResponse<Role>> CreateRoleAsync(RoleDto dto)
        {
            var response = new ApiResponse<Role>();

            try
            {
                var result = await _rolesRepository.CreateRoleAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to create role";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Role created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<List<Role>>> GetAllRolesAsync()
        {
            var response = new ApiResponse<List<Role>>();

            try
            {
                var result = await _rolesRepository.GetAllRolesAsync();

                response.Data = result.Data;
                response.Message = "Roles retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<Role>> GetRoleByIdAsync(int roleId)
        {
            var response = new ApiResponse<Role>();

            try
            {
                var result = await _rolesRepository.GetRoleByIdAsync(roleId);

                if (result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Role not found";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Role retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<Role>> UpdateRoleAsync(int roleId, RoleDto dto)
        {
            var response = new ApiResponse<Role>();

            try
            {
                var result = await _rolesRepository.UpdateRoleAsync(roleId, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Role not found or update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Role updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<bool>> DeleteRoleAsync(int roleId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _rolesRepository.DeleteRoleAsync(roleId);

                if (!result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Role not found or delete failed";
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
    }
}
