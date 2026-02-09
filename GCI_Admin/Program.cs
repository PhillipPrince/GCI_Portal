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
var tmpProvider = builder.Services.BuildServiceProvider();
var connStringProvider = tmpProvider.GetRequiredService<ConnectionStringProvider>();
var connectionString = await connStringProvider.BuildConnectionString();
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddScoped<EventsRepository>();


builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
