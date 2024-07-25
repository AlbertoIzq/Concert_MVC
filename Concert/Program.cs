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
    connectionString = envVarReader["DataBase_ConnectionString_Development"];
}
else if (envName == SD.ENVIRONMENT_PRODUCTION)
{
    connectionString = Environment.GetEnvironmentVariable("DataBase_ConnectionString_Production");
}

// Get Stripe keys
string publishableKey = string.Empty;
string secretKey = string.Empty;
publishableKey = envVarReader["Stripe_PublishableKey"];
secretKey = envVarReader["Stripe_SecretKey"];
if (envName == SD.ENVIRONMENT_DEVELOPMENT)
{
    publishableKey = envVarReader["Stripe_PublishableKey"];
    secretKey = envVarReader["Stripe_SecretKey"];
}
else if (envName == SD.ENVIRONMENT_PRODUCTION)
{
    publishableKey = Environment.GetEnvironmentVariable("Stripe_PublishableKey");
    secretKey = Environment.GetEnvironmentVariable("Stripe_SecretKey");
}

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
    string email = string.Empty;
    string name = string.Empty;
    string surname = string.Empty;
    string streetAddress = string.Empty;
    string city = string.Empty;
    string state = string.Empty;
    string country = string.Empty;
    string postalCode = string.Empty;
    string phoneNumber = string.Empty;
    string password = string.Empty;

    if (envName == SD.ENVIRONMENT_DEVELOPMENT)
    {
        email = envVarReader["AdminUser_Email"];
        name = envVarReader["AdminUser_Name"];
        surname = envVarReader["AdminUser_Surname"];
        streetAddress = envVarReader["AdminUser_StreetAddress"];
        city = envVarReader["AdminUser_City"];
        state = envVarReader["AdminUser_State"];
        country = envVarReader["AdminUser_Country"];
        postalCode = envVarReader["AdminUser_PostalCode"];
        phoneNumber = envVarReader["AdminUser_PhoneNumber"];
        password = envVarReader["AdminUser_Password"];
    }
    else if (envName == SD.ENVIRONMENT_PRODUCTION)
    {
        email = Environment.GetEnvironmentVariable("AdminUser_Email");
        name = Environment.GetEnvironmentVariable("AdminUser_Name");
        surname = Environment.GetEnvironmentVariable("AdminUser_Surname"); ;
        streetAddress = Environment.GetEnvironmentVariable("AdminUser_StreetAddress");
        city = Environment.GetEnvironmentVariable("AdminUser_City");
        state = Environment.GetEnvironmentVariable("AdminUser_State");
        country = Environment.GetEnvironmentVariable("AdminUser_Country");
        postalCode = Environment.GetEnvironmentVariable("AdminUser_PostalCode");
        phoneNumber = Environment.GetEnvironmentVariable("AdminUser_PhoneNumber");
        password = Environment.GetEnvironmentVariable("AdminUser_Password");
    }

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