using GCI_Admin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GCI_Admin.DBOperations.Repositories
{
    public class ChurchDailyActivitiesRepository
    {
        private readonly AppDbContext _context;

        public ChurchDailyActivitiesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChurchDailyActivity>> GetAllAsync()
        {
            return await _context.ChurchDailyActivities
                .OrderBy(a => a.DayOfWeek)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<ChurchDailyActivity> GetByIdAsync(int id)
        {
            return await _context.ChurchDailyActivities.FindAsync(id);
        }

        public async Task<ChurchDailyActivity> CreateAsync(ChurchDailyActivity activity)
        {
            activity.CreatedAt = DateTime.UtcNow;
            activity.UpdatedAt = DateTime.UtcNow;
            
            _context.ChurchDailyActivities.Add(activity);
            await _context.SaveChangesAsync();
            return activity;
        }

        public async Task<ChurchDailyActivity> UpdateAsync(ChurchDailyActivity activity)
        {
            var existing = await _context.ChurchDailyActivities.FindAsync(activity.Id);
            if (existing == null) return null;

            existing.DayOfWeek = activity.DayOfWeek;
            existing.ActivityName = activity.ActivityName;
            existing.Description = activity.Description;
            existing.StartTime = activity.StartTime;
            existing.EndTime = activity.EndTime;
            existing.IsActive = activity.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.ChurchDailyActivities.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var activity = await _context.ChurchDailyActivities.FindAsync(id);
            if (activity == null) return false;

            _context.ChurchDailyActivities.Remove(activity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id, bool isActive)
        {
            var activity = await _context.ChurchDailyActivities.FindAsync(id);
            if (activity == null) return false;

            activity.IsActive = isActive;
            activity.UpdatedAt = DateTime.UtcNow;

            _context.ChurchDailyActivities.Update(activity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
