using Concert.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using DotEnv.Core;
using Concert.DataAccess.Repository.IRepository;
using Concert.DataAccess.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Concert.Utility;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Read environment variables.
new EnvLoader().Load();
var envVarReader = new EnvReader();
// Get connectionString
string serverName = envVarReader["ConnectionString_DefaultConnection_ServerName"];
string databaseName = envVarReader["ConnectionString_DefaultConnection_DatabaseName"];
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
connectionString = connectionString?.Replace("ServerName", serverName);
connectionString = connectionString?.Replace("DatabaseName", databaseName);
// Get Stripe keys
string publishableKey = envVarReader["Stripe_PublishableKey"];
string secretKey = envVarReader["Stripe_SecretKey"];

// Add services to the container.

// Add the MVC service.
builder.Services.AddControllersWithViews();

// Add Session service
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

// Add an external login like Facebook
/// @todo create app in facebook.com and get keys
builder.Services.AddAuthentication().AddFacebook(option => {
    option.AppId = "";
    option.AppSecret = "";
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
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>()?.Replace("SecretKey", secretKey);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();