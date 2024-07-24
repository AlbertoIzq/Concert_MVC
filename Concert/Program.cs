using Concert.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using DotEnv.Core;
using Concert.DataAccess.Repository.IRepository;
using Concert.DataAccess.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Concert.Utility;
using Stripe;
using Concert.DataAccess.DbInitializer;
using Concert.Models;
using Concert.Utility.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Environment variables management

string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

// Read environment variables.
new EnvLoader().Load();
var envVarReader = new EnvReader();

// Get connectionString
string connectionString = string.Empty;

if (envName == SD.ENVIRONMENT_DEVELOPMENT)
{
    connectionString = envVarReader[SD.DATABASE_CONNECTION_STRING_ENV_NAME + envName];
}
else if (envName == SD.ENVIRONMENT_PRODUCTION)
{
    connectionString = Environment.GetEnvironmentVariable(SD.ENV_CONNECTION_STRING);
}

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
/// @todo create app in facebook.com and get keys, now they're only an example
builder.Services.AddAuthentication().AddFacebook(option => {
    option.AppId = "193813826680436";
    option.AppSecret = "8fc42ae3f4f2a4986143461d4e2da919";
});

// Add IUnitOfWork service
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add IEmailSender service
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Add IDbInitializer service
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

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

SeedDataBase(envVarReader);

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDataBase(EnvReader envReader)
{
    // Get AdminUser properties from env file
    string email = envVarReader["AdminUser_Email"];
    string name = envVarReader["AdminUser_Name"];
    string surname = envVarReader["AdminUser_Surname"];
    string streetAddress = envVarReader["AdminUser_StreetAddress"];
    string city = envVarReader["AdminUser_City"];
    string state = envVarReader["AdminUser_State"];
    string country = envVarReader["AdminUser_Country"];
    string postalCode = envVarReader["AdminUser_PostalCode"];
    string phoneNumber = envVarReader["AdminUser_PhoneNumber"];
    string password = envVarReader["AdminUser_Password"];

    using (var scope = app.Services.CreateScope())
    {
        ApplicationUser adminUser = new()
        {
            UserName = email,
            Email = email,
            Name = name,
            Surname = surname,
            StreetAddress = streetAddress,
            City = city,
            State = state,
            Country = country,
            PostalCode = postalCode,
            PhoneNumber = phoneNumber
        };

        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize(adminUser, password);
    }
}