using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GCI_Admin.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupIdToEventRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnnualEventCalendar",
                columns: table => new
                {
                    CalendarEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EventStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualEventCalendar", x => x.CalendarEventId);
                });

            migrationBuilder.CreateTable(
                name: "AnnualThemes",
                columns: table => new
                {
                    ThemeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Theme = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Verse = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Assembly = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualThemes", x => x.ThemeId);
                });

            migrationBuilder.CreateTable(
                name: "Assemblies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assemblies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BenevolenceBeneficiaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BenevolenceMemberId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenevolenceBeneficiaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CareRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAnonymous = table.Column<bool>(type: "bit", nullable: false),
                    IsPicked = table.Column<bool>(type: "bit", nullable: false),
                    PickedByPastorId = table.Column<int>(type: "int", nullable: true),
                    PickedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChurchDailyActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfWeek = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ActivityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChurchDailyActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountyCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeaconDutySummaryReports",
                columns: table => new
                {
                    DeaconDutySummaryReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeaconId = table.Column<int>(type: "int", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TuesdayPrayersObservation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThursdayBibleStudyObservation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FridayKeshaObservation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SundayServicesObservation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtherWeeklyEventsObservation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KeyIssuesForAttention = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeaconDutySummaryReports", x => x.DeaconDutySummaryReportId);
                });

            migrationBuilder.CreateTable(
                name: "EventAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAttendances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RequireRegistration = table.Column<bool>(type: "bit", nullable: false),
                    AllowWalkIns = table.Column<bool>(type: "bit", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    MinistryId = table.Column<int>(type: "int", nullable: true),
                    QrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowedAgeGroups = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "GECPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GECPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrowthCenters",
                columns: table => new
                {
                    GrowthCenterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CenterName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthCenters", x => x.GrowthCenterId);
                });

            migrationBuilder.CreateTable(
                name: "MeetingAttendances",
                columns: table => new
                {
                    MeetingAttendancesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeetingType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAttendees = table.Column<int>(type: "int", nullable: false),
                    MaleCount = table.Column<int>(type: "int", nullable: true),
                    FemaleCount = table.Column<int>(type: "int", nullable: true),
                    ChildrenCount = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingAttendances", x => x.MeetingAttendancesId);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtherNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Assembly = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    SocialMediaName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResidentialAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfChildren = table.Column<int>(type: "int", nullable: true),
                    UserRole = table.Column<int>(type: "int", nullable: false),
                    SpouseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MustChangePassword = table.Column<bool>(type: "bit", nullable: false),
                    GoogleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthProvider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsProfileComplete = table.Column<bool>(type: "bit", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    LockedUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ministries",
                columns: table => new
                {
                    MinistryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinistryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ministries", x => x.MinistryId);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyThemes",
                columns: table => new
                {
                    ThemeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Theme = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Assembly = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyThemes", x => x.ThemeId);
                });

            migrationBuilder.CreateTable(
                name: "NotificationGroups",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationGroups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    IsChurchWide = table.Column<bool>(type: "bit", nullable: false),
                    MinistryId = table.Column<int>(type: "int", nullable: true),
                    RcpsId = table.Column<int>(type: "int", nullable: true),
                    GrowthCenterId = table.Column<int>(type: "int", nullable: true),
                    NotificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiresReminder = table.Column<bool>(type: "bit", nullable: false),
                    SendSMS = table.Column<bool>(type: "bit", nullable: false),
                    SendPushNotification = table.Column<bool>(type: "bit", nullable: false),
                    SendEmail = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NotificationGroupId = table.Column<int>(type: "int", nullable: true),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    SmsSentCount = table.Column<int>(type: "int", nullable: true),
                    PushSentCount = table.Column<int>(type: "int", nullable: true),
                    PushNotificationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeepLinkScreen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeepLinkId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                });

            migrationBuilder.CreateTable(
                name: "OTPs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailOrPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OTPCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    MerchantRequestID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CheckoutRequestID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccountReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MpesaReceiptNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Paybill = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentStatusId = table.Column<int>(type: "int", nullable: false),
                    ResultCode = table.Column<int>(type: "int", nullable: true),
                    ResultDesc = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rcps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountRaised = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CountyCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rcps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceBankCollections",
                columns: table => new
                {
                    ServiceBankCollectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeetingAttendancesId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBankCollections", x => x.ServiceBankCollectionId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCashBreakdowns",
                columns: table => new
                {
                    ServiceCashBreakdownId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeetingAttendancesId = table.Column<int>(type: "int", nullable: false),
                    Denomination = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCashBreakdowns", x => x.ServiceCashBreakdownId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCollectionSignatures",
                columns: table => new
                {
                    ServiceCollectionSignatureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeetingAttendancesId = table.Column<int>(type: "int", nullable: false),
                    SignerMemberId = table.Column<int>(type: "int", nullable: false),
                    SignatureOrder = table.Column<int>(type: "int", nullable: true),
                    IsSigned = table.Column<bool>(type: "bit", nullable: false),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OtpSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OtpVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OtpChannel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCollectionSignatures", x => x.ServiceCollectionSignatureId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCollectionSummaries",
                columns: table => new
                {
                    ServiceCollectionSummaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeetingAttendancesId = table.Column<int>(type: "int", nullable: false),
                    Tithes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Offerings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SundaySchool = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Thanksgiving = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Missions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Projects = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Youth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WidowsOrphans = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Others = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedBy = table.Column<int>(type: "int", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCollectionSummaries", x => x.ServiceCollectionSummaryId);
                });

            migrationBuilder.CreateTable(
                name: "SpecialNotificationMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    IsNotified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialNotificationMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfigValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEditable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrowthCenterMeetings",
                columns: table => new
                {
                    GrowthCenterMeetingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrowthCenterId = table.Column<int>(type: "int", nullable: false),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BibleStudyTopic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    FinishingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalMembers = table.Column<int>(type: "int", nullable: false),
                    TotalVisitors = table.Column<int>(type: "int", nullable: false),
                    NumberOfChildren = table.Column<int>(type: "int", nullable: false),
                    OfferingCollected = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthCenterMeetings", x => x.GrowthCenterMeetingId);
                    table.ForeignKey(
                        name: "FK_GrowthCenterMeetings_GrowthCenters_GrowthCenterId",
                        column: x => x.GrowthCenterId,
                        principalTable: "GrowthCenters",
                        principalColumn: "GrowthCenterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssemblyLeaders",
                columns: table => new
                {
                    AssemblyLeaderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    AssemblyId = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyLeaders", x => x.AssemblyLeaderId);
                    table.ForeignKey(
                        name: "FK_AssemblyLeaders_Assemblies_AssemblyId",
                        column: x => x.AssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssemblyLeaders_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BenevolenceMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreferredCoverAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextOfKinName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextOfKinPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfDependants = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalAmountDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenevolenceMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BenevolenceMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deacons",
                columns: table => new
                {
                    DeaconId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Ministry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OnDuty = table.Column<bool>(type: "bit", nullable: false),
                    HasSpecialDuties = table.Column<bool>(type: "bit", nullable: false),
                    IsEmeritus = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deacons", x => x.DeaconId);
                    table.ForeignKey(
                        name: "FK_Deacons_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Elders",
                columns: table => new
                {
                    ElderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOrdained = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elders", x => x.ElderId);
                    table.ForeignKey(
                        name: "FK_Elders_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventFeedback",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    AttendanceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NonAttendanceReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SpeakerRating = table.Column<int>(type: "int", nullable: true),
                    ProgramRating = table.Column<int>(type: "int", nullable: true),
                    FacilitiesRating = table.Column<int>(type: "int", nullable: true),
                    MessageRating = table.Column<int>(type: "int", nullable: true),
                    MealsRating = table.Column<int>(type: "int", nullable: true),
                    ScheduleRating = table.Column<int>(type: "int", nullable: true),
                    LikedMost = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Improvements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Insights = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventFeedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventFeedback_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventFeedback_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventRegistrations",
                columns: table => new
                {
                    RegistrationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    GuestName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuestEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuestPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuestAssembly = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuestAgeGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentStatusId = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HasAttended = table.Column<bool>(type: "bit", nullable: true),
                    CheckoutRequestID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRegistrations", x => x.RegistrationId);
                    table.ForeignKey(
                        name: "FK_EventRegistrations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventRegistrations_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaithPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaithPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaithPosts_Members_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FaithPosts_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GECMembers",
                columns: table => new
                {
                    GECId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    GECPositionId = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GECMembers", x => x.GECId);
                    table.ForeignKey(
                        name: "FK_GECMembers_GECPositions_GECPositionId",
                        column: x => x.GECPositionId,
                        principalTable: "GECPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GECMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrowthCenterLeaders",
                columns: table => new
                {
                    GrowthCenterLeaderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    GrowthCenterId = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthCenterLeaders", x => x.GrowthCenterLeaderId);
                    table.ForeignKey(
                        name: "FK_GrowthCenterLeaders_GrowthCenters_GrowthCenterId",
                        column: x => x.GrowthCenterId,
                        principalTable: "GrowthCenters",
                        principalColumn: "GrowthCenterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrowthCenterLeaders_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrowthCenterMembers",
                columns: table => new
                {
                    GrowthCenterMemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrowthCenterId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    MembershipStatusId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthCenterMembers", x => x.GrowthCenterMemberId);
                    table.ForeignKey(
                        name: "FK_GrowthCenterMembers_GrowthCenters_GrowthCenterId",
                        column: x => x.GrowthCenterId,
                        principalTable: "GrowthCenters",
                        principalColumn: "GrowthCenterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrowthCenterMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberAdditionalInformations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    MembershipYear = table.Column<int>(type: "int", nullable: true),
                    Cohort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMemberOfAnotherChurch = table.Column<bool>(type: "bit", nullable: false),
                    FormerChurchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReasonForLeavingFormerChurch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateBeganAttendingGCI = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SeekingMembership = table.Column<bool>(type: "bit", nullable: false),
                    IsBornAgain = table.Column<bool>(type: "bit", nullable: false),
                    DateOfConversion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlaceOfConversion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasEternalLifeAssurance = table.Column<bool>(type: "bit", nullable: false),
                    HeavenReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MeaningOfChristsDeath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBaptizedByImmersion = table.Column<bool>(type: "bit", nullable: false),
                    BaptismDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BaptismPlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WillingToBeBaptizedAtGCI = table.Column<bool>(type: "bit", nullable: false),
                    PreviousMinistryExperience = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecialGiftsOrServiceInterest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsInformationConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    County = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ReasonsForMembership = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SpousePhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    YearMarried = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberAdditionalInformations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberAdditionalInformations_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RcpsPledges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    RcpsId = table.Column<int>(type: "int", nullable: false),
                    PledgedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PledgeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentRecieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RcpsPledges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RcpsPledges_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MinistryLeaders",
                columns: table => new
                {
                    MinistryLeaderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    MinistryId = table.Column<int>(type: "int", nullable: false),
                    PositionTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinistryLeaders", x => x.MinistryLeaderId);
                    table.ForeignKey(
                        name: "FK_MinistryLeaders_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinistryLeaders_Ministries_MinistryId",
                        column: x => x.MinistryId,
                        principalTable: "Ministries",
                        principalColumn: "MinistryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MinistryMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinistryId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MembershipStatusId = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinistryMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinistryMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinistryMembers_Ministries_MinistryId",
                        column: x => x.MinistryId,
                        principalTable: "Ministries",
                        principalColumn: "MinistryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RcpCountyMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RcpsId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    IsLeader = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RcpCountyMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RcpCountyMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RcpCountyMembers_Rcps_RcpsId",
                        column: x => x.RcpsId,
                        principalTable: "Rcps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RcpsCountyCoordinators",
                columns: table => new
                {
                    RcpsCountyCoordinatorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    RcpsId = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RcpsCountyCoordinators", x => x.RcpsCountyCoordinatorId);
                    table.ForeignKey(
                        name: "FK_RcpsCountyCoordinators_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RcpsCountyCoordinators_Rcps_RcpsId",
                        column: x => x.RcpsId,
                        principalTable: "Rcps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RcpsPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RcpsId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountRaised = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RcpsPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RcpsPlans_Rcps_RcpsId",
                        column: x => x.RcpsId,
                        principalTable: "Rcps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrowthCenterMeetingAttendees",
                columns: table => new
                {
                    GrowthCenterMeetingAttendeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrowthCenterMeetingId = table.Column<int>(type: "int", nullable: false),
                    MemberName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthCenterMeetingAttendees", x => x.GrowthCenterMeetingAttendeeId);
                    table.ForeignKey(
                        name: "FK_GrowthCenterMeetingAttendees_GrowthCenterMeetings_GrowthCenterMeetingId",
                        column: x => x.GrowthCenterMeetingId,
                        principalTable: "GrowthCenterMeetings",
                        principalColumn: "GrowthCenterMeetingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrowthCenterMeetingVisitors",
                columns: table => new
                {
                    GrowthCenterMeetingVisitorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrowthCenterMeetingId = table.Column<int>(type: "int", nullable: false),
                    VisitorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthCenterMeetingVisitors", x => x.GrowthCenterMeetingVisitorId);
                    table.ForeignKey(
                        name: "FK_GrowthCenterMeetingVisitors_GrowthCenterMeetings_GrowthCenterMeetingId",
                        column: x => x.GrowthCenterMeetingId,
                        principalTable: "GrowthCenterMeetings",
                        principalColumn: "GrowthCenterMeetingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaithPostComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaithPostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaithPostComments_FaithPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "FaithPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaithPostComments_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MinistryLeaderReports",
                columns: table => new
                {
                    MinistryLeaderReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinistryId = table.Column<int>(type: "int", nullable: false),
                    SubmittedByMinistryLeaderId = table.Column<int>(type: "int", nullable: false),
                    ReportingMonth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HadCalendarActivity = table.Column<bool>(type: "bit", nullable: false),
                    CalendarActivity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportedPillar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PillarSupportDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalendarActivityAttendance = table.Column<int>(type: "int", nullable: true),
                    HadOtherMeetings = table.Column<bool>(type: "bit", nullable: false),
                    OtherMeetingDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtherMeetingAttendance = table.Column<int>(type: "int", nullable: true),
                    TotalRegisteredMembers = table.Column<int>(type: "int", nullable: false),
                    LeadershipSupportComments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinistryLeaderReports", x => x.MinistryLeaderReportId);
                    table.ForeignKey(
                        name: "FK_MinistryLeaderReports_Ministries_MinistryId",
                        column: x => x.MinistryId,
                        principalTable: "Ministries",
                        principalColumn: "MinistryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinistryLeaderReports_MinistryLeaders_SubmittedByMinistryLeaderId",
                        column: x => x.SubmittedByMinistryLeaderId,
                        principalTable: "MinistryLeaders",
                        principalColumn: "MinistryLeaderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RcpsInvites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RcpsPlanId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    CustomName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueLinkCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TotalRaised = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContributorsCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RcpsInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RcpsInvites_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RcpsInvites_RcpsPlans_RcpsPlanId",
                        column: x => x.RcpsPlanId,
                        principalTable: "RcpsPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RcpsFriendContributions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RcpsInviteId = table.Column<int>(type: "int", nullable: false),
                    FriendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FriendPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsAnonymous = table.Column<bool>(type: "bit", nullable: false),
                    CheckoutRequestID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentStatusId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RcpsFriendContributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RcpsFriendContributions_RcpsInvites_RcpsInviteId",
                        column: x => x.RcpsInviteId,
                        principalTable: "RcpsInvites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyLeaders_AssemblyId",
                table: "AssemblyLeaders",
                column: "AssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyLeaders_MemberId",
                table: "AssemblyLeaders",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_BenevolenceMembers_MemberId",
                table: "BenevolenceMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Deacons_MemberId",
                table: "Deacons",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Elders_MemberId",
                table: "Elders",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_EventFeedback_EventId",
                table: "EventFeedback",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventFeedback_MemberId",
                table: "EventFeedback",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_EventId",
                table: "EventRegistrations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_MemberId",
                table: "EventRegistrations",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_FaithPostComments_MemberId",
                table: "FaithPostComments",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_FaithPostComments_PostId",
                table: "FaithPostComments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_FaithPosts_ApprovedBy",
                table: "FaithPosts",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FaithPosts_MemberId",
                table: "FaithPosts",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_GECMembers_GECPositionId",
                table: "GECMembers",
                column: "GECPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_GECMembers_MemberId",
                table: "GECMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthCenterLeaders_GrowthCenterId",
                table: "GrowthCenterLeaders",
                column: "GrowthCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthCenterLeaders_MemberId",
                table: "GrowthCenterLeaders",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthCenterMeetingAttendees_GrowthCenterMeetingId",
                table: "GrowthCenterMeetingAttendees",
                column: "GrowthCenterMeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthCenterMeetings_GrowthCenterId",
                table: "GrowthCenterMeetings",
                column: "GrowthCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthCenterMeetingVisitors_GrowthCenterMeetingId",
                table: "GrowthCenterMeetingVisitors",
                column: "GrowthCenterMeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthCenterMembers_GrowthCenterId",
                table: "GrowthCenterMembers",
                column: "GrowthCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthCenterMembers_MemberId",
                table: "GrowthCenterMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAdditionalInformations_MemberId",
                table: "MemberAdditionalInformations",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MinistryLeaderReports_MinistryId",
                table: "MinistryLeaderReports",
                column: "MinistryId");

            migrationBuilder.CreateIndex(
                name: "IX_MinistryLeaderReports_SubmittedByMinistryLeaderId",
                table: "MinistryLeaderReports",
                column: "SubmittedByMinistryLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_MinistryLeaders_MemberId",
                table: "MinistryLeaders",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MinistryLeaders_MinistryId",
                table: "MinistryLeaders",
                column: "MinistryId");

            migrationBuilder.CreateIndex(
                name: "IX_MinistryMembers_MemberId",
                table: "MinistryMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MinistryMembers_MinistryId",
                table: "MinistryMembers",
                column: "MinistryId");

            migrationBuilder.CreateIndex(
                name: "IX_RcpCountyMembers_MemberId",
                table: "RcpCountyMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RcpCountyMembers_RcpsId",
                table: "RcpCountyMembers",
                column: "RcpsId");

            migrationBuilder.CreateIndex(
                name: "IX_RcpsCountyCoordinators_MemberId",
                table: "RcpsCountyCoordinators",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RcpsCountyCoordinators_RcpsId",
                table: "RcpsCountyCoordinators",
                column: "RcpsId");

            migrationBuilder.CreateIndex(
                name: "IX_RcpsFriendContributions_RcpsInviteId",
                table: "RcpsFriendContributions",
                column: "RcpsInviteId");

            migrationBuilder.CreateIndex(
                name: "IX_RcpsInvites_MemberId",
                table: "RcpsInvites",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RcpsInvites_RcpsPlanId",
                table: "RcpsInvites",
                column: "RcpsPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_RcpsPlans_RcpsId",
                table: "RcpsPlans",
                column: "RcpsId");

            migrationBuilder.CreateIndex(
                name: "IX_RcpsPledges_MemberId",
                table: "RcpsPledges",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnualEventCalendar");

            migrationBuilder.DropTable(
                name: "AnnualThemes");

            migrationBuilder.DropTable(
                name: "AssemblyLeaders");

            migrationBuilder.DropTable(
                name: "BenevolenceBeneficiaries");

            migrationBuilder.DropTable(
                name: "BenevolenceMembers");

            migrationBuilder.DropTable(
                name: "CareRequests");

            migrationBuilder.DropTable(
                name: "ChurchDailyActivities");

            migrationBuilder.DropTable(
                name: "Counties");

            migrationBuilder.DropTable(
                name: "DeaconDutySummaryReports");

            migrationBuilder.DropTable(
                name: "Deacons");

            migrationBuilder.DropTable(
                name: "Elders");

            migrationBuilder.DropTable(
                name: "EventAttendances");

            migrationBuilder.DropTable(
                name: "EventFeedback");

            migrationBuilder.DropTable(
                name: "EventRegistrations");

            migrationBuilder.DropTable(
                name: "FaithPostComments");

            migrationBuilder.DropTable(
                name: "GECMembers");

            migrationBuilder.DropTable(
                name: "GrowthCenterLeaders");

            migrationBuilder.DropTable(
                name: "GrowthCenterMeetingAttendees");

            migrationBuilder.DropTable(
                name: "GrowthCenterMeetingVisitors");

            migrationBuilder.DropTable(
                name: "GrowthCenterMembers");

            migrationBuilder.DropTable(
                name: "MeetingAttendances");

            migrationBuilder.DropTable(
                name: "MemberAdditionalInformations");

            migrationBuilder.DropTable(
                name: "MinistryLeaderReports");

            migrationBuilder.DropTable(
                name: "MinistryMembers");

            migrationBuilder.DropTable(
                name: "MonthlyThemes");

            migrationBuilder.DropTable(
                name: "NotificationGroups");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OTPs");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RcpCountyMembers");

            migrationBuilder.DropTable(
                name: "RcpsCountyCoordinators");

            migrationBuilder.DropTable(
                name: "RcpsFriendContributions");

            migrationBuilder.DropTable(
                name: "RcpsPledges");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "ServiceBankCollections");

            migrationBuilder.DropTable(
                name: "ServiceCashBreakdowns");

            migrationBuilder.DropTable(
                name: "ServiceCollectionSignatures");

            migrationBuilder.DropTable(
                name: "ServiceCollectionSummaries");

            migrationBuilder.DropTable(
                name: "SpecialNotificationMembers");

            migrationBuilder.DropTable(
                name: "SystemConfig");

            migrationBuilder.DropTable(
                name: "Assemblies");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "FaithPosts");

            migrationBuilder.DropTable(
                name: "GECPositions");

            migrationBuilder.DropTable(
                name: "GrowthCenterMeetings");

            migrationBuilder.DropTable(
                name: "MinistryLeaders");

            migrationBuilder.DropTable(
                name: "RcpsInvites");

            migrationBuilder.DropTable(
                name: "GrowthCenters");

            migrationBuilder.DropTable(
                name: "Ministries");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "RcpsPlans");

            migrationBuilder.DropTable(
                name: "Rcps");
        }
    }
}
