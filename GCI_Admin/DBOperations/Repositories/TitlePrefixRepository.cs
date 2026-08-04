using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class TitlePrefixRepository
    {
        private readonly AppDbContext _context;

        public TitlePrefixRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DbResponse<List<TitlePrefix>>> GetAllPrefixesAsync()
        {
            try
            {
                var prefixes = await _context.TitlePrefixes
                    .OrderBy(p => p.Id)
                    .ToListAsync();

                return new DbResponse<List<TitlePrefix>>
                {
                    Success = true,
                    Data = prefixes
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<TitlePrefix>>
                {
                    Success = false,
                    Message = $"Error fetching title prefixes: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<TitlePrefix>>> GetActivePrefixesAsync()
        {
            try
            {
                var prefixes = await _context.TitlePrefixes
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Id)
                    .ToListAsync();

                return new DbResponse<List<TitlePrefix>>
                {
                    Success = true,
                    Data = prefixes
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<TitlePrefix>>
                {
                    Success = false,
                    Message = $"Error fetching active title prefixes: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<TitlePrefix>> GetPrefixByIdAsync(int id)
        {
            try
            {
                var prefix = await _context.TitlePrefixes.FirstOrDefaultAsync(x => x.Id == id);
                if (prefix == null)
                {
                    return new DbResponse<TitlePrefix>
                    {
                        Success = false,
                        Message = "Title prefix not found"
                    };
                }

                return new DbResponse<TitlePrefix>
                {
                    Success = true,
                    Data = prefix
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<TitlePrefix>
                {
                    Success = false,
                    Message = $"Error fetching title prefix: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<TitlePrefix>> CreatePrefixAsync(TitlePrefixDto dto)
        {
            try
            {
                bool exists = await _context.TitlePrefixes.AnyAsync(p => p.Title.ToLower() == dto.Title.Trim().ToLower());
                if (exists)
                {
                    return new DbResponse<TitlePrefix>
                    {
                        Success = false,
                        Message = "A title prefix with this title already exists"
                    };
                }

                var entity = new TitlePrefix
                {
                    Title = dto.Title.Trim(),
                    Description = dto.Description?.Trim(),
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now
                };

                _context.TitlePrefixes.Add(entity);
                await _context.SaveChangesAsync();

                return new DbResponse<TitlePrefix>
                {
                    Success = true,
                    Message = "Title prefix created successfully",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<TitlePrefix>
                {
                    Success = false,
                    Message = $"Error creating title prefix: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<TitlePrefix>> UpdatePrefixAsync(TitlePrefixDto dto)
        {
            try
            {
                var entity = await _context.TitlePrefixes.FirstOrDefaultAsync(x => x.Id == dto.Id);
                if (entity == null)
                {
                    return new DbResponse<TitlePrefix>
                    {
                        Success = false,
                        Message = "Title prefix not found"
                    };
                }

                bool exists = await _context.TitlePrefixes.AnyAsync(p => p.Id != dto.Id && p.Title.ToLower() == dto.Title.Trim().ToLower());
                if (exists)
                {
                    return new DbResponse<TitlePrefix>
                    {
                        Success = false,
                        Message = "A title prefix with this title already exists"
                    };
                }

                entity.Title = dto.Title.Trim();
                entity.Description = dto.Description?.Trim();
                entity.IsActive = dto.IsActive;
                entity.UpdatedAt = DateTime.Now;

                _context.TitlePrefixes.Update(entity);
                await _context.SaveChangesAsync();

                return new DbResponse<TitlePrefix>
                {
                    Success = true,
                    Message = "Title prefix updated successfully",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<TitlePrefix>
                {
                    Success = false,
                    Message = $"Error updating title prefix: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<bool>> ToggleStatusAsync(int id, bool isActive)
        {
            try
            {
                var entity = await _context.TitlePrefixes.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Title prefix not found"
                    };
                }

                entity.IsActive = isActive;
                entity.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = $"Title prefix status updated to {(isActive ? "Active" : "Inactive")}",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error toggling title prefix status: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<bool>> DeletePrefixAsync(int id)
        {
            try
            {
                var entity = await _context.TitlePrefixes.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Title prefix not found"
                    };
                }

                _context.TitlePrefixes.Remove(entity);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Title prefix deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting title prefix: {ex.Message}"
                };
            }
        }
    }
}
