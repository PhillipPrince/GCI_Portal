using GCI_Admin.DBOperations;
using Utils;
using Microsoft.EntityFrameworkCore;
using GCI_Admin.Services.IService;
using GCI_Admin.Services.Service;
using GCI_Admin.DBOperations.Repositories;
using Repo_GCI;
using GCI_Admin.Services;
using GCI_Admin.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

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

// ================= AUTH SERVICES =================
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.Redirect("/Auth/Index");
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            context.Response.Redirect("/Auth/Unauthorized");
            return Task.CompletedTask;
        }
    };
});

// ================= INFRASTRUCTURE =================
builder.Services.AddHttpClient<CommunicationService>();
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
builder.Services.AddScoped<PaymentsRepository>();
builder.Services.AddScoped<BenevolenceRepository>();
builder.Services.AddScoped<LeadershipRepository>();
builder.Services.AddScoped<SystemConfigRepository>();
builder.Services.AddScoped<RolesRepository>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<RcpsRepository>();
builder.Services.AddScoped<ReportsRepository>();

// ================= SERVICES =================
builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddScoped<IMembersService, MembersService>();
builder.Services.AddScoped<IGECMemberService, GECMemberService>();
builder.Services.AddScoped<IAssembliesService, AssembliesService>();
builder.Services.AddScoped<IMinistriesService, MinistriesService>();
builder.Services.AddScoped<IGrowthCentersService, GrowthCentersService>();
builder.Services.AddScoped<IAnnouncementsService, AnnouncementsService>();
builder.Services.AddScoped<IPaymentsService, PaymentsService>();
builder.Services.AddScoped<IBenevolenceService, BenevolenceService>();
builder.Services.AddScoped<ILeadershipService, LeadershipService>();
builder.Services.AddScoped<ISystemConfigService, SystemConfigService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRcpsService, RcpsService>();
builder.Services.AddScoped<IReportsService, ReportsService>();

builder.Services.AddScoped<SessionManager>();

// Build app
var app = builder.Build();

// ================= PIPELINE =================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ⚠️ IMPORTANT: Authentication must come BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();