using GCI_Admin.Models;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class ReportsRepository
    {
        private readonly AppDbContext _context;

        public ReportsRepository(AppDbContext context)
        {
            _context = context;
        }

        #region GROWTH CENTER MEETINGS

        public async Task<DbResponse<List<GrowthCenterMeeting>>> GetAllGrowthCenterMeetingsAsync()
        {
            try
            {
                var meetings = await _context.GrowthCenterMeetings
                    .Include(x => x.GrowthCenter)
                    .OrderByDescending(x => x.MeetingDate)
                    .ToListAsync();
                return new DbResponse<List<GrowthCenterMeeting>>
                {
                    Success = true,
                    Data = meetings
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetAllGrowthCenterMeetingsAsync: {ex}");
                return new DbResponse<List<GrowthCenterMeeting>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<GrowthCenterMeeting>> GetGrowthCenterMeetingByIdAsync(int meetingId)
        {
            try
            {
                var meeting = await _context.GrowthCenterMeetings
                    .Include(x => x.GrowthCenter)
                    .FirstOrDefaultAsync(x => x.GrowthCenterMeetingId == meetingId);
                if (meeting == null)
                {
                    return new DbResponse<GrowthCenterMeeting>
                    {
                        Success = false,
                        Message = "Meeting not found"
                    };
                }
                return new DbResponse<GrowthCenterMeeting>
                {
                    Success = true,
                    Data = meeting
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetGrowthCenterMeetingByIdAsync: {ex}");
                return new DbResponse<GrowthCenterMeeting>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<List<GrowthCenterMeeting>>> GetGrowthCenterMeetingsByCenterIdAsync(int centerId)
        {
            try
            {
                var meetings = await _context.GrowthCenterMeetings
                    .Where(x => x.GrowthCenterId == centerId)
                    .Include(x => x.GrowthCenter)
                    .OrderByDescending(x => x.MeetingDate)
                    .ToListAsync();
                return new DbResponse<List<GrowthCenterMeeting>>
                {
                    Success = true,
                    Data = meetings
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetGrowthCenterMeetingsByCenterIdAsync: {ex}");
                return new DbResponse<List<GrowthCenterMeeting>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<List<GrowthCenterMeetingAttendee>>> GetAttendanceByMeetingIdAsync(int meetingId)
        {
            try
            {
                var attendees = await _context.GrowthCenterMeetingAttendees
                    .Where(x => x.GrowthCenterMeetingId == meetingId)
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();
                return new DbResponse<List<GrowthCenterMeetingAttendee>>
                {
                    Success = true,
                    Data = attendees
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetAttendanceByMeetingIdAsync: {ex}");
                return new DbResponse<List<GrowthCenterMeetingAttendee>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<List<GrowthCenterMeetingVisitor>>> GetVisitorsByMeetingIdAsync(int meetingId)
        {
            try
            {
                var visitors = await _context.GrowthCenterMeetingVisitors
                    .Where(x => x.GrowthCenterMeetingId == meetingId)
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();
                return new DbResponse<List<GrowthCenterMeetingVisitor>>
                {
                    Success = true,
                    Data = visitors
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetVisitorsByMeetingIdAsync: {ex}");
                return new DbResponse<List<GrowthCenterMeetingVisitor>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region MINISTRY LEADER REPORTS

        public async Task<DbResponse<List<MinistryLeaderReport>>> GetAllMinistryLeaderReportsAsync()
        {
            try
            {
                var reports = await _context.MinistryLeaderReports
                    .Include(x => x.Ministry)
                    .Include(x => x.SubmittedByMinistryLeader)
                        .ThenInclude(l => l.Member)
                    .OrderByDescending(x => x.ReportingMonth)
                    .ToListAsync();

                var memberIds = reports
                    .Where(x => x.SubmittedByMinistryLeader != null)
                    .Select(x => x.SubmittedByMinistryLeader.MemberId)
                    .Distinct()
                    .ToList();

                var members = await _context.Members
                    .Where(x => memberIds.Contains(x.Id))
                    .ToListAsync();

                foreach (var report in reports)
                {
                    var member = members.FirstOrDefault(x =>
                        x.Id == report.SubmittedByMinistryLeader?.MemberId);

                    report.SubmittedByMinistryLeaderName = member != null
                        ? $"{member.FirstName} {member.OtherNames}"
                        : "Unknown Leader";
                }

                return new DbResponse<List<MinistryLeaderReport>>
                {
                    Success = true,
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetAllMinistryLeaderReportsAsync: {ex}");
                return new DbResponse<List<MinistryLeaderReport>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<MinistryLeaderReport>> GetMinistryLeaderReportByIdAsync(int reportId)
        {
            try
            {
                var report = await _context.MinistryLeaderReports
                    .Include(x => x.Ministry)
                    .Include(x => x.SubmittedByMinistryLeader)
                        .ThenInclude(l => l.Member)
                    .FirstOrDefaultAsync(x => x.MinistryLeaderReportId == reportId);

                if (report == null)
                {
                    return new DbResponse<MinistryLeaderReport>
                    {
                        Success = false,
                        Message = "Report not found"
                    };
                }

                return new DbResponse<MinistryLeaderReport>
                {
                    Success = true,
                    Data = report
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetMinistryLeaderReportByIdAsync: {ex}");
                return new DbResponse<MinistryLeaderReport>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<List<MinistryLeaderReport>>> GetMinistryLeaderReportsByMinistryIdAsync(int ministryId)
        {
            try
            {
                var reports = await _context.MinistryLeaderReports
                    .Where(x => x.MinistryId == ministryId)
                    .Include(x => x.Ministry)
                    .Include(x => x.SubmittedByMinistryLeader)
                        .ThenInclude(l => l.Member)
                    .OrderByDescending(x => x.ReportingMonth)
                    .ToListAsync();

                return new DbResponse<List<MinistryLeaderReport>>
                {
                    Success = true,
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetMinistryLeaderReportsByMinistryIdAsync: {ex}");
                return new DbResponse<List<MinistryLeaderReport>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<List<MinistryLeaderReport>>> GetMinistryLeaderReportsByLeaderIdAsync(int leaderId)
        {
            try
            {
                var reports = await _context.MinistryLeaderReports
                    .Where(x => x.SubmittedByMinistryLeaderId == leaderId)
                    .Include(x => x.Ministry)
                    .Include(x => x.SubmittedByMinistryLeader)
                        .ThenInclude(l => l.Member)
                    .OrderByDescending(x => x.ReportingMonth)
                    .ToListAsync();

                return new DbResponse<List<MinistryLeaderReport>>
                {
                    Success = true,
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetMinistryLeaderReportsByLeaderIdAsync: {ex}");
                return new DbResponse<List<MinistryLeaderReport>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<List<MinistryLeaderReport>>> GetMinistryLeaderReportsByDateRangeAsync(DateTime from, DateTime to)
        {
            try
            {
                var reports = await _context.MinistryLeaderReports
                    .Where(x => x.ReportingMonth >= from && x.ReportingMonth <= to)
                    .Include(x => x.Ministry)
                    .Include(x => x.SubmittedByMinistryLeader)
                    .OrderByDescending(x => x.ReportingMonth)
                    .ToListAsync();

                return new DbResponse<List<MinistryLeaderReport>>
                {
                    Success = true,
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetMinistryLeaderReportsByDateRangeAsync: {ex}");
                return new DbResponse<List<MinistryLeaderReport>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region DEACON DUTY REPORTS

        public async Task<DbResponse<List<DeaconDutySummaryReport>>> GetAllDeaconDutyReportsAsync()
        {
            try
            {
                var reports = await _context.DeaconDutySummaryReports
                    .OrderByDescending(x => x.ReportDate)
                    .ToListAsync();
                return new DbResponse<List<DeaconDutySummaryReport>>
                {
                    Success = true,
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetAllDeaconDutyReportsAsync: {ex}");
                return new DbResponse<List<DeaconDutySummaryReport>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<List<DeaconDutySummaryReport>>> GetDeaconDutyReportsByDateRangeAsync(DateTime from, DateTime to)
        {
            try
            {
                var reports = await _context.DeaconDutySummaryReports
                    .Where(x => x.ReportDate >= from && x.ReportDate <= to)
                    .OrderByDescending(x => x.ReportDate)
                    .ToListAsync();
                return new DbResponse<List<DeaconDutySummaryReport>>
                {
                    Success = true,
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetDeaconDutyReportsByDateRangeAsync: {ex}");
                return new DbResponse<List<DeaconDutySummaryReport>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<DeaconDutySummaryReport>> GetDeaconDutyReportByIdAsync(int reportId)
        {
            try
            {
                var report = await _context.DeaconDutySummaryReports
                    .FirstOrDefaultAsync(x => x.DeaconDutySummaryReportId == reportId);
                if (report == null)
                {
                    return new DbResponse<DeaconDutySummaryReport>
                    {
                        Success = false,
                        Message = "Report not found"
                    };
                }
                return new DbResponse<DeaconDutySummaryReport>
                {
                    Success = true,
                    Data = report
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetDeaconDutyReportByIdAsync: {ex}");
                return new DbResponse<DeaconDutySummaryReport>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<List<DeaconDutySummaryReport>>> GetDeaconDutyReportsByDeaconNameAsync(string deaconName)
        {
            try
            {
                var reports = await _context.DeaconDutySummaryReports
                    .Where(x => x.KeyIssuesForAttention.Contains(deaconName))
                    .OrderByDescending(x => x.ReportDate)
                    .ToListAsync();
                return new DbResponse<List<DeaconDutySummaryReport>>
                {
                    Success = true,
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetDeaconDutyReportsByDeaconNameAsync: {ex}");
                return new DbResponse<List<DeaconDutySummaryReport>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region GROWTH CENTER MONTHLY REPORT

        public async Task<DbResponse<List<object>>> GetMonthlyGrowthCenterReportAsync(int year, int month)
        {
            try
            {
                var data = await _context.GrowthCenterMeetings
                    .Include(x => x.GrowthCenter)
                    .Where(x => x.MeetingDate.Year == year && x.MeetingDate.Month == month)
                    .Select(x => new
                    {
                        x.GrowthCenter.CenterName,
                        x.MeetingDate,
                        x.BibleStudyTopic,
                        x.TotalMembers,
                        x.TotalVisitors,
                        x.NumberOfChildren,
                        x.OfferingCollected
                    })
                    .ToListAsync();

                return new DbResponse<List<object>>
                {
                    Success = true,
                    Data = data.Cast<object>().ToList()
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetMonthlyGrowthCenterReportAsync: {ex}");
                return new DbResponse<List<object>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region ATTENDANCE REPORT

        public async Task<DbResponse<List<object>>> GetAttendanceReportAsync(int meetingId)
        {
            try
            {
                var data = await _context.GrowthCenterMeetingAttendees
                    .Where(x => x.GrowthCenterMeetingId == meetingId)
                    .Select(x => new
                    {
                        x.MemberName,
                        x.CreatedAt
                    })
                    .ToListAsync();

                return new DbResponse<List<object>>
                {
                    Success = true,
                    Data = data.Cast<object>().ToList()
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetAttendanceReportAsync: {ex}");
                return new DbResponse<List<object>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region VISITOR REPORT

        public async Task<DbResponse<List<object>>> GetVisitorReportAsync(int meetingId)
        {
            try
            {
                var data = await _context.GrowthCenterMeetingVisitors
                    .Where(x => x.GrowthCenterMeetingId == meetingId)
                    .Select(x => new
                    {
                        x.VisitorName,
                        x.CreatedAt
                    })
                    .ToListAsync();

                return new DbResponse<List<object>>
                {
                    Success = true,
                    Data = data.Cast<object>().ToList()
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetVisitorReportAsync: {ex}");
                return new DbResponse<List<object>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region DEACON DUTY REPORT SUMMARY

        public async Task<DbResponse<List<object>>> GetDeaconDutyReportAsync(DateTime from, DateTime to)
        {
            try
            {
                var data = await _context.DeaconDutySummaryReports
                    .Where(x => x.ReportDate >= from && x.ReportDate <= to)
                    .Select(x => new
                    {
                        x.ReportDate,
                        x.TuesdayPrayersObservation,
                        x.ThursdayBibleStudyObservation,
                        x.FridayKeshaObservation,
                        x.SundayServicesObservation,
                        x.KeyIssuesForAttention
                    })
                    .ToListAsync();

                return new DbResponse<List<object>>
                {
                    Success = true,
                    Data = data.Cast<object>().ToList()
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetDeaconDutyReportAsync: {ex}");
                return new DbResponse<List<object>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region CHURCH REPORTS

        public async Task<DbResponse<ChurchReportViewModel>> GetChurchReportsAsync()
        {
            try
            {
                // Get current date
                var currentDate = DateTime.Now;
                var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var startOfLastMonth = startOfMonth.AddMonths(-1);
                var endOfLastMonth = startOfMonth.AddDays(-1);

                // Total Members (from Members table)
                var totalMembers = await _context.Members.CountAsync(x => x.StatusId!=7||x.StatusId!=6);
                var newMembersThisMonth = await _context.Members
                    .CountAsync(x => x.CreatedAt >= startOfMonth && !(x.StatusId == 7 || x.StatusId == 6));

                // Total Ministries
                var totalMinistries = await _context.Ministries.CountAsync(x => x.IsActive);

                // Total Events
                var totalEvents = await _context.Events.CountAsync();
                var monthlyEvents = await _context.Events
                    .CountAsync(x => x.EventDate.Year == currentDate.Year && x.EventDate.Month == currentDate.Month);

                // Total Offerings from Growth Center Meetings
                var currentMonthOfferings = await _context.GrowthCenterMeetings
                    .Where(x => x.MeetingDate >= startOfMonth)
                    .SumAsync(x => (decimal?)x.OfferingCollected) ?? 0;

                var lastMonthOfferings = await _context.GrowthCenterMeetings
                    .Where(x => x.MeetingDate >= startOfLastMonth && x.MeetingDate <= endOfLastMonth)
                    .SumAsync(x => (decimal?)x.OfferingCollected) ?? 0;

                var offeringGrowth = lastMonthOfferings > 0
                    ? ((currentMonthOfferings - lastMonthOfferings) / lastMonthOfferings) * 100
                    : 0;

                // Gender Distribution
                var maleCount = await _context.Members.CountAsync(x => x.Gender == "Male" && !(x.StatusId == 7 || x.StatusId == 6));
                var femaleCount = await _context.Members.CountAsync(x => x.Gender == "Female" && !(x.StatusId == 7 || x.StatusId == 6));

                // Age Demographics (based on MemberAdditionalInformation or calculate from DateOfBirth)
                var ageDemographics = await GetAgeDemographicsAsync();

                // Marital Status
                var maritalStatus = await GetMaritalStatusAsync();

                // Education Level
               // var educationLevel = await GetEducationLevelAsync();

                // Employment Status
               // var employmentStatus = await GetEmploymentStatusAsync();

                // Top Ministries
                var topMinistries = await GetTopMinistriesAsync();

                // Top Events
                var topEvents = await GetTopEventsAsync();

                // Growth Trend (Last 12 months)
                var growthTrend = await GetMemberGrowthTrendAsync();

                // Highest Attendance Overall
                var highestAttendance = await GetHighestAttendanceOverallAsync();

                var report = new ChurchReportViewModel
                {
                    TotalMembers = totalMembers,
                    NewMembersThisMonth = newMembersThisMonth,
                    TotalMinistries = totalMinistries,
                    TotalEvents = totalEvents,
                    MonthlyEvents = monthlyEvents,
                    TotalOfferings = currentMonthOfferings,
                    OfferingGrowth = offeringGrowth,
                    HighestAttendance = highestAttendance,
                    GenderDistribution = new GenderDistributionModel { Male = maleCount, Female = femaleCount },
                    AgeDemographics = ageDemographics,
                    MaritalStatus = maritalStatus,
                    //EducationLevel = educationLevel,
                    //EmploymentStatus = employmentStatus,
                    TopMinistries = topMinistries,
                    TopEvents = topEvents,
                    GrowthTrend = growthTrend
                };

                return new DbResponse<ChurchReportViewModel>
                {
                    Success = true,
                    Data = report
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetChurchReportsAsync: {ex}");
                return new DbResponse<ChurchReportViewModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region MINISTRY REPORTS

        public async Task<DbResponse<MinistryReportViewModel>> GetMinistryReportsAsync()
        {
            try
            {
                var activeMinistries = await _context.Ministries.CountAsync(x => x.IsActive);

                var ministries = await _context.Ministries
                    //.Include(x => x.MinistryLeaders)
                    .Where(x => x.IsActive)
                    .ToListAsync();

                var ministryDetails = new List<MinistryDetailModel>();
                var ministryGenderData = new List<MinistryGenderDataModel>();
                var ministryPerformance = new List<MinistryPerformanceModel>();

                int totalMinistryMembers = 0;
                int totalMale = 0;
                int totalFemale = 0;

                foreach (var ministry in ministries)
                {
                    // Get members associated with this ministry (you may need to adjust based on your schema)
                    var ministryMembers = await _context.Members
                        .Where(x => x.Id == ministry.MinistryId && x.StatusId != 7 && x.StatusId != 6)
                        .ToListAsync();

                    var maleCount = ministryMembers.Count(x => x.Gender == "Male");
                    var femaleCount = ministryMembers.Count(x => x.Gender == "Female");
                    var youthCount = ministryMembers.Count(x => x.DateOfBirth.HasValue &&
                        (DateTime.Now.Year - x.DateOfBirth.Value.Year) <= 25);

                    totalMinistryMembers += ministryMembers.Count;
                    totalMale += maleCount;
                    totalFemale += femaleCount;

                    ministryDetails.Add(new MinistryDetailModel
                    {
                        MinistryId = ministry.MinistryId,
                        MinistryName = ministry.MinistryName,
                        LeaderName = "N/A", // Adjust based on your data
                        TotalMembers = ministryMembers.Count,
                        MaleCount = maleCount,
                        FemaleCount = femaleCount,
                        YouthCount = youthCount,
                        WeeklyMeetings = 1, // Default value, adjust based on your data
                        AverageAttendance = ministryMembers.Count, // Default value
                        IsActive = ministry.IsActive,
                        CreatedAt = ministry.CreatedAt
                    });

                    ministryGenderData.Add(new MinistryGenderDataModel
                    {
                        MinistryId = ministry.MinistryId,
                        MinistryName = ministry.MinistryName,
                        Male = maleCount,
                        Female = femaleCount
                    });

                    ministryPerformance.Add(new MinistryPerformanceModel
                    {
                        MinistryId = ministry.MinistryId,
                        MinistryName = ministry.MinistryName,
                        TotalMembers = ministryMembers.Count,
                        AverageAttendance = ministryMembers.Count,
                        Growth = 0 // Calculate growth if you have historical data
                    });
                }

                // Get highest attendance ministry
                var highestAttendance = ministryPerformance
                    .OrderByDescending(x => x.AverageAttendance)
                    .FirstOrDefault();

                var highestAttendanceModel = highestAttendance != null ? new HighestAttendanceMinistryModel
                {
                    MinistryId = highestAttendance.MinistryId,
                    MinistryName = highestAttendance.MinistryName,
                    Attendance = (int)highestAttendance.AverageAttendance,
                    AverageAttendance = highestAttendance.AverageAttendance
                } : null;

                var report = new MinistryReportViewModel
                {
                    ActiveMinistries = activeMinistries,
                    TotalMinistryMembers = totalMinistryMembers,
                    AverageMembersPerMinistry = activeMinistries > 0 ? (double)totalMinistryMembers / activeMinistries : 0,
                    MalePercentage = totalMinistryMembers > 0 ? (double)totalMale / totalMinistryMembers * 100 : 0,
                    FemalePercentage = totalMinistryMembers > 0 ? (double)totalFemale / totalMinistryMembers * 100 : 0,
                    HighestAttendance = highestAttendanceModel,
                    GenderDistribution = new GenderDistributionModel { Male = totalMale, Female = totalFemale },
                    MinistryGenderData = ministryGenderData,
                    Ministries = ministryDetails,
                    Performance = ministryPerformance
                };

                return new DbResponse<MinistryReportViewModel>
                {
                    Success = true,
                    Data = report
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetMinistryReportsAsync: {ex}");
                return new DbResponse<MinistryReportViewModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region GROWTH CENTER REPORTS

        public async Task<DbResponse<GrowthCenterReportViewModel>> GetGrowthCenterReportsAsync()
        {
            try
            {
                var growthCenters = await _context.GrowthCenters
                    .Where(x => x.IsActive)
                    .ToListAsync();

                var allMeetings = await _context.GrowthCenterMeetings
                    .Include(x => x.GrowthCenter)
                    .ToListAsync();

                var totalCenters = growthCenters.Count;
                var totalMeetings = allMeetings.Count;
                var totalAttendance = allMeetings.Sum(x => x.TotalMembers + x.TotalVisitors);
                var totalOfferings = allMeetings.Sum(x => x.OfferingCollected);

                var centersPerformance = new List<GrowthCenterPerformanceModel>();
                var highestAttendanceCenter = new HighestAttendanceCenterModel { Attendance = 0 };

                foreach (var center in growthCenters)
                {
                    var centerMeetings = allMeetings.Where(x => x.GrowthCenterId == center.GrowthCenterId).ToList();
                    var totalCenterAttendance = centerMeetings.Sum(x => x.TotalMembers + x.TotalVisitors);
                    var avgAttendance = centerMeetings.Count > 0 ? totalCenterAttendance / centerMeetings.Count : 0;

                    centersPerformance.Add(new GrowthCenterPerformanceModel
                    {
                        CenterId = center.GrowthCenterId,
                        CenterName = center.CenterName,
                        TotalMeetings = centerMeetings.Count,
                        TotalAttendance = totalCenterAttendance,
                        AverageAttendance = avgAttendance,
                        TotalMembers = centerMeetings.Sum(x => x.TotalMembers),
                        TotalVisitors = centerMeetings.Sum(x => x.TotalVisitors),
                        TotalChildren = centerMeetings.Sum(x => x.NumberOfChildren),
                        TotalOfferings = centerMeetings.Sum(x => x.OfferingCollected)
                    });

                    if (totalCenterAttendance > highestAttendanceCenter.Attendance)
                    {
                        highestAttendanceCenter = new HighestAttendanceCenterModel
                        {
                            CenterId = center.GrowthCenterId,
                            CenterName = center.CenterName,
                            Attendance = totalCenterAttendance,
                            TotalMeetings = centerMeetings.Count,
                            AverageAttendance = avgAttendance
                        };
                    }
                }

                // Sort by attendance
                centersPerformance = centersPerformance.OrderByDescending(x => x.TotalAttendance).ToList();

                // Get distribution
                var distribution = new AttendanceDistributionModel
                {
                    Members = allMeetings.Sum(x => x.TotalMembers),
                    Visitors = allMeetings.Sum(x => x.TotalVisitors),
                    Children = allMeetings.Sum(x => x.NumberOfChildren)
                };

                // Get trends (last 6 months)
                var trends = await GetAttendanceTrendsAsync();

                var report = new GrowthCenterReportViewModel
                {
                    TotalCenters = totalCenters,
                    TotalMeetings = totalMeetings,
                    TotalAttendance = totalAttendance,
                    TotalOfferings = totalOfferings,
                    HighestAttendance = highestAttendanceCenter,
                    Distribution = distribution,
                    Centers = centersPerformance,
                    Trends = trends
                };

                return new DbResponse<GrowthCenterReportViewModel>
                {
                    Success = true,
                    Data = report
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetGrowthCenterReportsAsync: {ex}");
                return new DbResponse<GrowthCenterReportViewModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region EVENTS REPORTS

        public async Task<DbResponse<EventsReportViewModel>> GetEventsReportsAsync()
        {
            try
            {
                var events = await _context.Events.ToListAsync();
                var eventRegistrations = await _context.EventRegistrations.ToListAsync();

                var totalEvents = events.Count;
                var upcomingEvents = events.Count(x => x.EventDate >= DateTime.Now);
                var totalAttendance = eventRegistrations.Count(x => x.HasAttended ?? false);
                var averageAttendance = totalEvents > 0 ? totalAttendance / totalEvents : 0;

                // Top events by attendance
                var topEvents = new List<TopEventChartModel>();
                var allEventsDetails = new List<EventDetailModel>();
                var highestAttendanceEvent = new HighestAttendanceEventModel { Attendance = 0 };

                foreach (var ev in events)
                {
                    var attendance = eventRegistrations.Count(x => x.EventId == ev.EventId && x.HasAttended == true);

                    topEvents.Add(new TopEventChartModel
                    {
                        EventId = ev.EventId,
                        EventName = ev.Title,
                        Attendance = attendance,
                    });

                    allEventsDetails.Add(new EventDetailModel
                    {
                        EventId = ev.EventId,
                        EventName = ev.Title,
                        EventDate = ev.EventDate,
                        Location = ev.Location,
                        Attendance = attendance,
                        Status = ev.EventDate < DateTime.Now ? "Completed" : "Upcoming",
                        Description = ev.Description,
                        CreatedAt = ev.CreatedAt
                    });

                    if (attendance > highestAttendanceEvent.Attendance)
                    {
                        highestAttendanceEvent = new HighestAttendanceEventModel
                        {
                            EventId = ev.EventId,
                            EventName = ev.Title,
                            Attendance = attendance,
                            EventDate = ev.EventDate,
                        };
                    }
                }

                // Get categories
               

                // Get monthly attendance trends
                var monthlyAttendance = await GetMonthlyEventAttendanceAsync();

                // Get event trends
                var eventTrends = await GetEventTrendsAsync();

                var report = new EventsReportViewModel
                {
                    TotalEvents = totalEvents,
                    TotalAttendance = totalAttendance,
                    UpcomingEvents = upcomingEvents,
                    AverageAttendance = averageAttendance,
                    HighestAttendance = highestAttendanceEvent,
                    TopEvents = topEvents.OrderByDescending(x => x.Attendance).Take(5).ToList(),
                    Trends = eventTrends,
                    MonthlyAttendance = monthlyAttendance,
                    AllEvents = allEventsDetails.OrderByDescending(x => x.EventDate).ToList()
                };

                return new DbResponse<EventsReportViewModel>
                {
                    Success = true,
                    Data = report
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetEventsReportsAsync: {ex}");
                return new DbResponse<EventsReportViewModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region DASHBOARD SUMMARY

        public async Task<DbResponse<DashboardSummaryModel>> GetDashboardSummaryAsync()
        {
            try
            {
                var totalCenters = await _context.GrowthCenters.CountAsync(x => x.IsActive);
                var totalMeetings = await _context.GrowthCenterMeetings.CountAsync();
                var totalMembers = await _context.Members.CountAsync(x => !(x.StatusId == 7 || x.StatusId == 6));
                var totalMinistries = await _context.Ministries.CountAsync(x => x.IsActive);
                var totalEvents = await _context.Events.CountAsync();

                var totalAttendance = await _context.GrowthCenterMeetings
                    .SumAsync(x => (int?)x.TotalMembers + x.TotalVisitors) ?? 0;

                var totalOfferings = await _context.GrowthCenterMeetings
                    .SumAsync(x => (decimal?)x.OfferingCollected) ?? 0;

                var maleMembers = await _context.Members.CountAsync(x => x.Gender == "Male" && !(x.StatusId == 7 || x.StatusId == 6));
                var femaleMembers = await _context.Members.CountAsync(x => x.Gender == "Female" && !(x.StatusId == 7 || x.StatusId == 6));

                var totalChildren = await _context.GrowthCenterMeetings.SumAsync(x => x.NumberOfChildren);
                var totalVisitors = await _context.GrowthCenterMeetings.SumAsync(x => x.TotalVisitors);

                var monthlyMeetings = await _context.GrowthCenterMeetings
                    .CountAsync(x => x.MeetingDate.Year == DateTime.Now.Year && x.MeetingDate.Month == DateTime.Now.Month);

                var averageAttendance = totalMeetings > 0 ? totalAttendance / totalMeetings : 0;

                // Calculate growth percentage
                var lastMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
                var lastMonthEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);

                var lastMonthAttendance = await _context.GrowthCenterMeetings
                    .Where(x => x.MeetingDate >= lastMonthStart && x.MeetingDate <= lastMonthEnd)
                    .SumAsync(x => (int?)x.TotalMembers + x.TotalVisitors) ?? 0;

                var growthPercentage = lastMonthAttendance > 0
                    ? ((totalAttendance - lastMonthAttendance) / (double)lastMonthAttendance) * 100
                    : 0;

                // Get trends data for charts
                var trendsData = new List<TrendDataPoint>();
                for (int i = 5; i >= 0; i--)
                {
                    var monthDate = DateTime.Now.AddMonths(-i);
                    var monthStart = new DateTime(monthDate.Year, monthDate.Month, 1);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                    var monthAttendance = await _context.GrowthCenterMeetings
                        .Where(x => x.MeetingDate >= monthStart && x.MeetingDate <= monthEnd)
                        .SumAsync(x => (int?)x.TotalMembers + x.TotalVisitors) ?? 0;

                    var monthOfferings = await _context.GrowthCenterMeetings
                        .Where(x => x.MeetingDate >= monthStart && x.MeetingDate <= monthEnd)
                        .SumAsync(x => (decimal?)x.OfferingCollected) ?? 0;

                    trendsData.Add(new TrendDataPoint
                    {
                        Month = monthDate.ToString("MMM yyyy"),
                        TotalAttendance = monthAttendance,
                        TotalOfferings = monthOfferings
                    });
                }

                var summary = new DashboardSummaryModel
                {
                    TotalMembers = totalMembers,
                    TotalGrowthCenters = totalCenters,
                    TotalMinistries = totalMinistries,
                    TotalEvents = totalEvents,
                    TotalAttendance = totalAttendance,
                    TotalMeetings = totalMeetings,
                    TotalOfferings = totalOfferings,
                    AverageAttendance = averageAttendance,
                    MonthlyMeetings = monthlyMeetings,
                    GrowthPercentage = growthPercentage,
                    TotalMembersMale = maleMembers,
                    TotalMembersFemale = femaleMembers,
                    TotalChildren = totalChildren,
                    TotalVisitors = totalVisitors,
                    TrendsData = trendsData
                };

                return new DbResponse<DashboardSummaryModel>
                {
                    Success = true,
                    Data = summary
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error GetDashboardSummaryAsync: {ex}");
                return new DbResponse<DashboardSummaryModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        #endregion

        #region HELPER METHODS

        private async Task<AgeDemographicsModel> GetAgeDemographicsAsync()
        {
            var members = await _context.Members
                .Where(x => !(x.StatusId == 7 || x.StatusId == 6) && x.DateOfBirth.HasValue)
                .ToListAsync();

            var demographics = new AgeDemographicsModel();

            foreach (var member in members)
            {
                if (!member.DateOfBirth.HasValue) continue;

                var age = DateTime.Now.Year - member.DateOfBirth.Value.Year;
                if (member.DateOfBirth.Value.Date > DateTime.Now.AddYears(-age)) age--;

                if (age <= 12)
                    demographics.Children++;
                else if (age <= 25)
                    demographics.Youth++;
                else if (age <= 55)
                    demographics.Adults++;
                else
                    demographics.Seniors++;
            }

            return demographics;
        }

        private async Task<MaritalStatusModel> GetMaritalStatusAsync()
        {
            var maritalInfo = await _context.MemberAdditionalInformations
                .Include(x => x.Member)
                .Where(x => x.Member.StatusId != 7 || x.Member.StatusId != 6)
                .ToListAsync();

            var status = new MaritalStatusModel();

            foreach (var info in maritalInfo)
            {
                switch (info.Member.MaritalStatus?.ToLower())
                {
                    case "single":
                        status.Single++;
                        break;
                    case "married":
                        status.Married++;
                        break;
                    case "divorced":
                        status.Divorced++;
                        break;
                    case "widowed":
                        status.Widowed++;
                        break;
                }
            }

            return status;
        }

        //private async Task<EducationLevelModel> GetEducationLevelAsync()
        //{
        //    var educationInfo = await _context.MemberAdditionalInformations
        //        .Include(x => x.Member)
        //        .Where(x => x.Member.StatusId != 7 && x.Member.StatusId != 6)
        //        .ToListAsync();

        //    var education = new EducationLevelModel();

        //    foreach (var info in educationInfo)
        //    {
        //        switch (info.EducationLevel?.ToLower())
        //        {
        //            case "none":
        //                education.None++;
        //                break;
        //            case "primary":
        //                education.Primary++;
        //                break;
        //            case "secondary":
        //                education.Secondary++;
        //                break;
        //            case "tertiary":
        //                education.Tertiary++;
        //                break;
        //            case "university":
        //                education.University++;
        //                break;
        //        }
        //    }

        //    return education;
        //}

        //private async Task<EmploymentStatusModel> GetEmploymentStatusAsync()
        //{
        //    var employmentInfo = await _context.MemberAdditionalInformations
        //        .Include(x => x.Member)
        //        .Where(x => x.Member.StatusId != 7 && x.Member.StatusId != 6)
        //        .ToListAsync();

        //    var employment = new EmploymentStatusModel();

        //    foreach (var info in employmentInfo)
        //    {
        //        switch (info.EmploymentStatus?.ToLower())
        //        {
        //            case "employed":
        //                employment.Employed++;
        //                break;
        //            case "self-employed":
        //                employment.SelfEmployed++;
        //                break;
        //            case "unemployed":
        //                employment.Unemployed++;
        //                break;
        //            case "student":
        //                employment.Student++;
        //                break;
        //            case "retired":
        //                employment.Retired++;
        //                break;
        //        }
        //    }

        //    return employment;
        //}

        private async Task<List<TopMinistryModel>> GetTopMinistriesAsync()
        {
            var ministries = await _context.Ministries
                .Where(x => x.IsActive)
                .ToListAsync();

            var topMinistries = new List<TopMinistryModel>();

            foreach (var ministry in ministries)
            {
                var memberCount = await _context.Members
                    .CountAsync(x => x.Id == ministry.MinistryId);

                topMinistries.Add(new TopMinistryModel
                {
                    MinistryId = ministry.MinistryId,
                    MinistryName = ministry.MinistryName,
                    TotalMembers = memberCount,
                    //LeaderName = ministry.LeaderName,
                    Growth = 0 // Calculate growth if you have historical data
                });
            }

            return topMinistries.OrderByDescending(x => x.TotalMembers).Take(5).ToList();
        }

        private async Task<List<TopEventModel>> GetTopEventsAsync()
        {
            var events = await _context.Events.ToListAsync();
            var registrations = await _context.EventRegistrations.ToListAsync();

            var topEvents = new List<TopEventModel>();

            foreach (var ev in events)
            {
                var attendance = registrations.Count(x => x.EventId == ev.EventId && x.HasAttended == true);
                var status = ev.EventDate < DateTime.Now ? "Completed" :
                            (ev.EventDate.Date == DateTime.Now.Date ? "Ongoing" : "Upcoming");

                topEvents.Add(new TopEventModel
                {
                    EventId = ev.EventId,
                    EventName = ev.Title,
                    EventDate = ev.EventDate,
                    Attendance = attendance,
                    Status = status
                });
            }

            return topEvents.OrderByDescending(x => x.Attendance).Take(5).ToList();
        }

        private async Task<List<GrowthTrendModel>> GetMemberGrowthTrendAsync()
        {
            var trends = new List<GrowthTrendModel>();
            var currentDate = DateTime.Now;

            for (int i = 11; i >= 0; i--)
            {
                var monthDate = currentDate.AddMonths(-i);
                var startOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var totalMembers = await _context.Members
                    .CountAsync(x => x.CreatedAt <= endOfMonth && !(x.StatusId == 7 || x.StatusId == 6));

                var newMembers = await _context.Members
                    .CountAsync(x => x.CreatedAt >= startOfMonth && x.CreatedAt <= endOfMonth && !(x.StatusId == 7 || x.StatusId == 6));

                trends.Add(new GrowthTrendModel
                {
                    Month = monthDate.ToString("MMM"),
                    Year = monthDate.Year,
                    NewMembers = newMembers,
                    TotalMembers = totalMembers
                });
            }

            return trends;
        }

        private async Task<HighestAttendanceModel> GetHighestAttendanceOverallAsync()
        {
            // Check Growth Centers
            var growthCenters = await _context.GrowthCenters.ToListAsync();
            var centerAttendances = new List<HighestAttendanceModel>();

            foreach (var center in growthCenters)
            {
                var attendance = await _context.GrowthCenterMeetings
                    .Where(x => x.GrowthCenterId == center.GrowthCenterId)
                    .SumAsync(x => (int?)x.TotalMembers + x.TotalVisitors) ?? 0;

                centerAttendances.Add(new HighestAttendanceModel
                {
                    Name = center.CenterName,
                    Attendance = attendance,
                    Type = "Growth Center"
                });
            }

            // Check Ministries
            var ministries = await _context.Ministries.Where(x => x.IsActive).ToListAsync();
            foreach (var ministry in ministries)
            {
                var memberCount = await _context.Members
                    .CountAsync(x => x.Id == ministry.MinistryId && !(x.StatusId == 7 || x.StatusId == 6));

                centerAttendances.Add(new HighestAttendanceModel
                {
                    Name = ministry.MinistryName,
                    Attendance = memberCount,
                    Type = "Ministry"
                });
            }

            // Check Events
            var events = await _context.Events.ToListAsync();
            var registrations = await _context.EventRegistrations.ToListAsync();

            foreach (var ev in events)
            {
                var attendance = registrations.Count(x => x.EventId == ev.EventId && x.HasAttended==true);
                centerAttendances.Add(new HighestAttendanceModel
                {
                    Name = ev.Title,
                    Attendance = attendance,
                    Type = "Event"
                });
            }

            return centerAttendances.OrderByDescending(x => x.Attendance).FirstOrDefault();
        }

        private async Task<List<AttendanceTrendModel>> GetAttendanceTrendsAsync()
        {
            var trends = new List<AttendanceTrendModel>();
            var currentDate = DateTime.Now;

            for (int i = 5; i >= 0; i--)
            {
                var monthDate = currentDate.AddMonths(-i);
                var startOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var totalAttendance = await _context.GrowthCenterMeetings
                    .Where(x => x.MeetingDate >= startOfMonth && x.MeetingDate <= endOfMonth)
                    .SumAsync(x => (int?)x.TotalMembers + x.TotalVisitors) ?? 0;

                var newVisitors = await _context.GrowthCenterMeetings
                    .Where(x => x.MeetingDate >= startOfMonth && x.MeetingDate <= endOfMonth)
                    .SumAsync(x => x.TotalVisitors);

                trends.Add(new AttendanceTrendModel
                {
                    Month = monthDate.ToString("MMM yyyy"),
                    Year = monthDate.Year,
                    Attendance = totalAttendance,
                    NewVisitors = newVisitors
                });
            }

            return trends;
        }

        private async Task<List<EventTrendModel>> GetEventTrendsAsync()
        {
            var trends = new List<EventTrendModel>();
            var currentDate = DateTime.Now;

            for (int i = 5; i >= 0; i--)
            {
                var monthDate = currentDate.AddMonths(-i);
                var startOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var eventsInMonth = await _context.Events
                    .Where(x => x.EventDate >= startOfMonth && x.EventDate <= endOfMonth)
                    .ToListAsync();

                var registrations = await _context.EventRegistrations
                    .Where(x => eventsInMonth.Select(e => e.EventId).Contains(x.EventId) && x.HasAttended==true)
                    .ToListAsync();

                trends.Add(new EventTrendModel
                {
                    Month = monthDate.ToString("MMM yyyy"),
                    Year = monthDate.Year,
                    Attendance = registrations.Count,
                    EventCount = eventsInMonth.Count
                });
            }

            return trends;
        }

        private async Task<List<MonthlyEventAttendanceModel>> GetMonthlyEventAttendanceAsync()
        {
            var monthlyData = new List<MonthlyEventAttendanceModel>();
            var currentDate = DateTime.Now;

            for (int i = 5; i >= 0; i--)
            {
                var monthDate = currentDate.AddMonths(-i);
                var startOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var eventsInMonth = await _context.Events
                    .Where(x => x.EventDate >= startOfMonth && x.EventDate <= endOfMonth)
                    .ToListAsync();

                var registrations = await _context.EventRegistrations
                    .Where(x => eventsInMonth.Select(e => e.EventId).Contains(x.EventId) && x.HasAttended==true)
                    .ToListAsync();

                monthlyData.Add(new MonthlyEventAttendanceModel
                {
                    Month = monthDate.ToString("MMM"),
                    Year = monthDate.Year,
                    Attendance = registrations.Count,
                    EventCount = eventsInMonth.Count
                });
            }

            return monthlyData;
        }

        #endregion
    }
}