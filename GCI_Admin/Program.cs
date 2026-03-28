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

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ConnectionStringProvider>();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connProvider = serviceProvider.GetRequiredService<ConnectionStringProvider>();

    var connectionString = connProvider.BuildConnectionString().GetAwaiter().GetResult();

    options.UseSqlServer(connectionString);
});
// Configs
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<SmsConfig>(builder.Configuration.GetSection("SmsConfig"));

builder.Services.Configure<JwtSettings>(jwtSettings);
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddHttpClient<CommunicationService>();
builder.Services.AddScoped<SessionManager>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();


// Repositories (Scoped)
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

// Services (Scoped)
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
builder.Services.AddScoped<IRolesService,RolesService>();
builder.Services.AddScoped<IAuthService, AuthService>();




builder.Services.Configure<JwtSettings>(jwtSettings);
builder.Services.AddScoped<JwtTokenService>();


    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(20); // Auto logout after 20 minutes
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    }); ;

var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),

        RoleClaimType = ClaimTypes.Role // 🔥 IMPORTANT for your Roles = "1"
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();

            // 🔥 Not logged in
            context.Response.Redirect("/Auth/Index");
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            // 🔥 Logged in but no role/permission
            context.Response.Redirect("/Auth/Unauthorized");
            return Task.CompletedTask;
        }
    };
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();   

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();