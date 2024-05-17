using Microsoft.EntityFrameworkCore;
using DotEnv.Core;
using ConcertRazor.Data;

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

// Add the Razor Pages service. 
builder.Services.AddRazorPages();

// Add the database service.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
