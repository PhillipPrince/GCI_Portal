using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class AssembliesRepository
    {
        private readonly AppDbContext _context;

        public AssembliesRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ CREATE ASSEMBLY
        public async Task<DbResponse<Assembly>> CreateAssemblyAsync(AssemblyDto dto)
        {
            try
            {
                var newAssembly = new Assembly
                {
                    Name = dto.Name,
                    Location = dto.Location,
                    ContactPhone = dto.ContactPhone,
                    ContactEmail = dto.ContactEmail,
                    CreatedAt = DateTime.Now
                };

                _context.Assemblies.Add(newAssembly);
                await _context.SaveChangesAsync();

                return new DbResponse<Assembly>
                {
                    Success = true,
                    Message = "Assembly created successfully",
                    Data = newAssembly
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in CreateAssemblyAsync: {ex}");
                return new DbResponse<Assembly>
                {
                    Success = false,
                    Message = $"Error creating assembly: {ex.Message}"
                };
            }
        }

        // ✅ GET ALL ASSEMBLIES
        public async Task<DbResponse<List<Assembly>>> GetAllAssembliesAsync()
        {
            try
            {
                var assemblies = await _context.Assemblies
                    .OrderBy(a => a.Id)
                    .ToListAsync();

                return new DbResponse<List<Assembly>>
                {
                    Success = true,
                    Data = assemblies
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Assembly>>
                {
                    Success = false,
                    Message = $"Error fetching assemblies: {ex.Message}"
                };
            }
        }

        // ✅ GET ASSEMBLY BY ID
        public async Task<DbResponse<Assembly>> GetAssemblyByIdAsync(int assemblyId)
        {
            try
            {
                var assembly = await _context.Assemblies
                    .FirstOrDefaultAsync(a => a.Id == assemblyId);

                if (assembly == null)
                {
                    return new DbResponse<Assembly>
                    {
                        Success = false,
                        Message = "Assembly not found"
                    };
                }

                return new DbResponse<Assembly>
                {
                    Success = true,
                    Data = assembly
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<Assembly>
                {
                    Success = false,
                    Message = $"Error fetching assembly: {ex.Message}"
                };
            }
        }

        // ✅ UPDATE ASSEMBLY
        public async Task<DbResponse<Assembly>> UpdateAssemblyAsync(int assemblyId, AssemblyDto dto)
        {
            try
            {
                var existingAssembly = await _context.Assemblies.FindAsync(assemblyId);

                if (existingAssembly == null)
                {
                    return new DbResponse<Assembly>
                    {
                        Success = false,
                        Message = "Assembly not found"
                    };
                }

                existingAssembly.Name = dto.Name;
                existingAssembly.Location = dto.Location;
                existingAssembly.ContactPhone = dto.ContactPhone;
                existingAssembly.ContactEmail = dto.ContactEmail;

                await _context.SaveChangesAsync();

                return new DbResponse<Assembly>
                {
                    Success = true,
                    Message = "Assembly updated successfully",
                    Data = existingAssembly
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<Assembly>
                {
                    Success = false,
                    Message = $"Error updating assembly: {ex.Message}"
                };
            }
        }

        // ✅ DELETE ASSEMBLY
        public async Task<DbResponse<bool>> DeleteAssemblyAsync(int assemblyId)
        {
            try
            {
                var assembly = await _context.Assemblies.FindAsync(assemblyId);

                if (assembly == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Assembly not found"
                    };
                }

                _context.Assemblies.Remove(assembly);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Assembly deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting assembly: {ex.Message}"
                };
            }
        }
    }
}
