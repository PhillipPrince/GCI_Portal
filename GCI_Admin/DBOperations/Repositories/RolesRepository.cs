using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class RolesRepository
    {
        private readonly AppDbContext _context;

        public RolesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DbResponse<Role>> CreateRoleAsync(RoleDto dto)
        {
            try
            {
                var newRole = new Role
                {
                    RoleName = dto.RoleName,
                    Description = dto.Description,
                    CreatedAt = DateTime.Now
                };

                _context.Roles.Add(newRole);
                await _context.SaveChangesAsync();

                return new DbResponse<Role>
                {
                    Success = true,
                    Message = "Role created successfully",
                    Data = newRole
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in CreateRoleAsync: {ex}");
                return new DbResponse<Role>
                {
                    Success = false,
                    Message = $"Error creating role: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<Role>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _context.Roles
                    .OrderBy(r => r.RoleId)
                    .ToListAsync();

                return new DbResponse<List<Role>>
                {
                    Success = true,
                    Data = roles
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Role>>
                {
                    Success = false,
                    Message = $"Error fetching roles: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<Role>> GetRoleByIdAsync(int roleId)
        {
            try
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleId == roleId);

                if (role == null)
                {
                    return new DbResponse<Role>
                    {
                        Success = false,
                        Message = "Role not found"
                    };
                }

                return new DbResponse<Role>
                {
                    Success = true,
                    Data = role
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<Role>
                {
                    Success = false,
                    Message = $"Error fetching role: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<Role>> UpdateRoleAsync(int roleId, RoleDto dto)
        {
            try
            {
                var existingRole = await _context.Roles.FindAsync(roleId);

                if (existingRole == null)
                {
                    return new DbResponse<Role>
                    {
                        Success = false,
                        Message = "Role not found"
                    };
                }

                existingRole.RoleName = dto.RoleName;
                existingRole.Description = dto.Description;

                await _context.SaveChangesAsync();

                return new DbResponse<Role>
                {
                    Success = true,
                    Message = "Role updated successfully",
                    Data = existingRole
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<Role>
                {
                    Success = false,
                    Message = $"Error updating role: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<bool>> DeleteRoleAsync(int roleId)
        {
            try
            {
                var role = await _context.Roles.FindAsync(roleId);

                if (role == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Role not found"
                    };
                }

                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Role deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting role: {ex.Message}"
                };
            }
        }
    }
}