using Concert.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using DotEnv.Core;
using Concert.DataAccess.Repository.IRepository;
using Concert.DataAccess.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Concert.Utility;

var builder = WebApplication.CreateBuilder(args);

// Read environment variables.
new EnvLoader().Load();
var envVarReader = new EnvReader();
// Get connectionString
string serverName = envVarReader["AppSettings_DefaultConnection_ServerName"];
string databaseName = envVarReader["AppSettings_DefaultConnection_DatabaseName"];
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
connectionString = connectionString?.Replace("ServerName", serverName);
connectionString = connectionString?.Replace("DatabaseName", databaseName);

// Add services to the container.

// Add the MVC service.
builder.Services.AddControllersWithViews();

// Add the Razor Pages service. 
builder.Services.AddRazorPages();

// Add the database service.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity service
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// Add IUnitOfWork service
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add IEmailSender service
builder.Services.AddScoped<IEmailSender, EmailSender>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();