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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AssemblyLeader>(entity =>
            {
                entity.HasOne(al => al.Assembly)
                      .WithMany()
                      .HasForeignKey(al => al.AssemblyId);

                entity.HasOne(al => al.Member)
                      .WithMany()
                      .HasForeignKey(al => al.MemberId);
            });

            modelBuilder.Entity<Assembly>(entity =>
            {
                entity.HasOne<AssemblyLeader>()
                      .WithMany()
                      .HasForeignKey(a => a.AssemblyLeaderId)
                      .IsRequired(false);
            });
        }
        public DbSet<Member> Members { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<AnnualEventCalendar> AnnualEventCalendars { get; set; }
        public DbSet<GECMember> GECMembers { get; set; }
        public DbSet<GECPosition> GECPositions { get; set; }
        public DbSet<TitlePrefix> TitlePrefixes { get; set; }
        public DbSet<SystemConfig> SystemConfig { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }
        public DbSet<MemberAdditionalInformation> MemberAdditionalInformations { get; set; }
        public DbSet<Assembly> Assemblies { get; set; }
        public DbSet<AssemblyLeader> AssembliesLeaders { get; set; }
        public DbSet<Ministry> Ministries { get; set; }
        public DbSet<MinistryLeader> MinistryLeaders { get; set;}
        public DbSet<MinistryMember> MinistryMembers { get; set; }
        public DbSet<GrowthCenter> GrowthCenters { get; set; }
        public DbSet<GrowthCenterLeader> GrowthCenterLeaders { get; set; }
        public DbSet<GrowthCenterMember> GrowthCenterMembers { get; set; }
        public DbSet<AnnualTheme> AnnualThemes { get; set; }
        public DbSet<MonthlyTheme> MonthlyThemes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<BenevolenceMember> BenevolenceMembers { get; set; }
        public DbSet<BenevolenceBeneficiary> BenevolenceBeneficiaries { get; set; }
        public DbSet<Deacon> Deacons { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Elder> Elders { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<NotificationGroup> NotificationGroups { get; set; }
        public DbSet<SpecialNotificationMember> SpecialNotificationMembers { get; set; }
        public DbSet<CareRequest> CareRequests { get; set; }
        public DbSet<Rcps> Rcps { get; set; }
        public DbSet<RcpsPledges> RcpsPledges { get; set; }
        public DbSet<RcpsPlan> RcpsPlans { get; set; }
        public DbSet<RcpsInvite> RcpsInvites { get; set; }
        public DbSet<RcpsFriendContribution> RcpsFriendContributions { get; set; }
        public DbSet<RcpsCountyCoordinator> RcpsCountyCoordinators { get; set; }
        public DbSet<RcpCountyMember> RcpCountyMembers { get; set; }
        public DbSet<GrowthCenterMeeting> GrowthCenterMeetings { get; set; }
        public DbSet<GrowthCenterMeetingAttendee> GrowthCenterMeetingAttendees { get; set; }
        public DbSet<GrowthCenterMeetingVisitor> GrowthCenterMeetingVisitors { get; set; }
        public DbSet<DeaconDutySummaryReport> DeaconDutySummaryReports { get; set; }
        public DbSet<MinistryLeaderReport> MinistryLeaderReports { get; set; }
        public DbSet<MeetingAttendance> MeetingAttendances { get; set; }
        public DbSet<ServiceCollectionSummary> ServiceCollectionSummaries { get; set; }
        public DbSet<ServiceCashBreakdown> ServiceCashBreakdowns { get; set; }
        public DbSet<ServiceBankCollection> ServiceBankCollections { get; set; }
        public DbSet<ServiceCollectionSignature> ServiceCollectionSignatures { get; set; }
        public DbSet<EventFeedback> EventFeedbacks { get; set; }
        public DbSet<FaithPost> FaithPosts { get; set; }
        public DbSet<FaithPostComment> FaithPostComments { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<EventAttendance> EventAttendances { get; set; }
        public DbSet<ChurchDailyActivity> ChurchDailyActivities { get; set; }
        public DbSet<EventSponsor> EventSponsors { get; set; }

    }
}
