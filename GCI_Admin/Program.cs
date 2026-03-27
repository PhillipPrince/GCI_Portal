using GCI_Admin.DBOperations;
using Utils;
using Microsoft.EntityFrameworkCore;
using GCI_Admin.Services.IService;
using GCI_Admin.Services.Service;
using GCI_Admin.DBOperations.Repositories;
using Repo_GCI;
using GCI_Admin.Services;
using GCI_Admin.Models;

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
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();