using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Services;
using GCI_Admin.Services.IService;
using GCI_Admin.Services.Service;
using GCI_Admin.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repo_GCI;
using System.Security.Claims;
using System.Text;
using Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ConnectionStringProvider>();

// DbContext
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connProvider = serviceProvider.GetRequiredService<ConnectionStringProvider>();
    var connectionString = connProvider.BuildConnectionString().GetAwaiter().GetResult();

    options.UseSqlServer(connectionString);
});

// ================= CONFIGURATION =================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.Configure<SmsConfig>(builder.Configuration.GetSection("SmsConfig"));
builder.Services.Configure<DevelopmentSettings>(builder.Configuration.GetSection("DevelopmentSettings"));
builder.Services.Configure<JwtSettings>(jwtSettings);
builder.Services.Configure<CloudinaryConfig>(builder.Configuration.GetSection("Cloudinary"));

// ================= AUTH SERVICES =================
builder.Services.AddScoped<JwtTokenService>();

// Add BOTH Cookie and JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "GCI_Auth_Cookie";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.LoginPath = "/Auth/Index";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/Unauthorized";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),

        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Try to get token from cookie or authorization header
            var token = context.Request.Cookies["GCI_Token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = 401;
            }
            else
            {
                context.Response.Redirect("/Auth/Index");
            }
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = 403;
            }
            else
            {
                context.Response.Redirect("/Auth/Unauthorized");
            }
            return Task.CompletedTask;
        }
    };
});

// ================= INFRASTRUCTURE =================
builder.Services.AddHttpClient<CommunicationService>();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ================= REPOSITORIES =================
builder.Services.AddScoped<EventsRepository>();
builder.Services.AddScoped<MembersRepository>();
builder.Services.AddScoped<GECMemberRepository>();
builder.Services.AddScoped<AssembliesRepository>();
builder.Services.AddScoped<MinistriesRepository>();
builder.Services.AddScoped<GrowthCentersRepository>();
builder.Services.AddScoped<AnnouncementsRepository>();
builder.Services.AddScoped<CollectionsRepository>();
builder.Services.AddScoped<BenevolenceRepository>();
builder.Services.AddScoped<LeadershipRepository>();
builder.Services.AddScoped<SystemConfigRepository>();
builder.Services.AddScoped<RolesRepository>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<RcpsRepository>();
builder.Services.AddScoped<ReportsRepository>();
builder.Services.AddScoped<MeetingsRepository>();
builder.Services.AddScoped<GalleryRepository>();
builder.Services.AddScoped<GECPositionRepository>();
builder.Services.AddScoped<TitlePrefixRepository>();
builder.Services.AddScoped<ChurchDailyActivitiesRepository>();

// ================= SERVICES =================
builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddScoped<IMembersService, MembersService>();
builder.Services.AddScoped<IGECMemberService, GECMemberService>();
builder.Services.AddScoped<IAssembliesService, AssembliesService>();
builder.Services.AddScoped<IMinistriesService, MinistriesService>();
builder.Services.AddScoped<IGrowthCentersService, GrowthCentersService>();
builder.Services.AddScoped<IAnnouncementsService, AnnouncementsService>();
builder.Services.AddScoped<ICollectionsService, CollectionsService>();
builder.Services.AddScoped<IChurchDailyActivitiesService, ChurchDailyActivitiesService>();
builder.Services.AddScoped<IBenevolenceService, BenevolenceService>();
builder.Services.AddScoped<ILeadershipService, LeadershipService>();
builder.Services.AddScoped<ITitlePrefixService, TitlePrefixService>();
builder.Services.AddScoped<ISystemConfigService, SystemConfigService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRcpsService, RcpsService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<IMeetingsService, MeetingsService>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<IGECPositionService, GECPositionService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddScoped<SessionManager>();

// Build app
var app = builder.Build();

// Run database schema update to add NotificationGroupId and QrCode columns to Events table if not present
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.ExecuteSqlRaw("IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Events') AND name = 'QrCode') ALTER TABLE Events ADD QrCode NVARCHAR(255) NULL;");
        db.Database.ExecuteSqlRaw("IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EventRegistrations') AND name = 'HasAttended') ALTER TABLE EventRegistrations ADD HasAttended BIT NULL;");
        db.Database.ExecuteSqlRaw("IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AnnualThemes') AND name = 'Assembly') ALTER TABLE AnnualThemes ADD Assembly NVARCHAR(150) NULL;");
        
        // Populate existing events with a QrCode if they don't have one
        db.Database.ExecuteSqlRaw("UPDATE Events SET QrCode = LOWER(REPLACE(NEWID(), '-', '')) WHERE QrCode IS NULL OR QrCode = '';");
        db.Database.ExecuteSqlRaw("UPDATE EventRegistrations SET HasAttended = 0 WHERE HasAttended IS NULL;");
        
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration Error: {ex.Message}");
    }
}

// ================= PIPELINE =================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseMiddleware<SessionCleanupMiddleware>();


// ⚠️ IMPORTANT: Authentication must come BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();