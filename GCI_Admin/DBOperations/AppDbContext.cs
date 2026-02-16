using GCI_Admin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GCI_Admin.DBOperations
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            
        }
        public DbSet<Member> Members { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<AnnualEventCalendar> AnnualEventCalendars { get; set; }
        public DbSet<GECMember> GECMembers { get; set; }
        public DbSet<SystemConfig> SystemConfig { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }

        //public DbSet<Assembly> Assemblies { get; set; }
    }
}
