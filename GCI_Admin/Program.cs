using GCI_Admin.DBOperations;
using Utils;
using Microsoft.EntityFrameworkCore;
using GCI_Admin.Services.IService;
using GCI_Admin.Services.Service;
using GCI_Admin.DBOperations.Repositories;

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

// Configure JWT settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

// Repositories (Scoped)
builder.Services.AddScoped<EventsRepository>();
builder.Services.AddScoped<MembersRepository>();
builder.Services.AddScoped<GECMemberRepository>();
builder.Services.AddScoped<AssembliesRepository>();
builder.Services.AddScoped<MinistriesRepository>();
builder.Services.AddScoped<GrowthCentersRepository>();

// Services (Scoped)
builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddScoped<IMembersService, MembersService>();
builder.Services.AddScoped<IGECMemberService, GECMemberService>();
builder.Services.AddScoped<IAssembliesService, AssembliesService>();
builder.Services.AddScoped<IMinistriesService, MinistriesService>();
builder.Services.AddScoped<IGrowthCentersService, GrowthCentersService>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();