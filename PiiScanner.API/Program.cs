using Microsoft.OpenApi.Models;
using PiiScanner.Infrastructure;
using PiiScanner.Domain.Interface;
using PiiScanner.Application.Service;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------
// Configure writable Azure path
// ---------------------------------------------------
var dataPath = Path.Combine(
    Environment.GetEnvironmentVariable("HOME") ?? Directory.GetCurrentDirectory(),
    "site",
    "wwwroot",
    "Develop");

// Ensure directory exists
Directory.CreateDirectory(dataPath);

// ---------------------------------------------------
// CORS
// ---------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ---------------------------------------------------
// Controllers
// ---------------------------------------------------
builder.Services.AddControllers();


// ---------------------------------------------------
// Build App
// ---------------------------------------------------
var app = builder.Build();

// ---------------------------------------------------
// Middleware Pipeline
// ---------------------------------------------------

// TEMP TEST ROUTE
app.MapGet("/", () => "PII Scanner API Running Successfully");


// HTTPS
app.UseHttpsRedirection();

// Static Files
app.UseDefaultFiles();
app.UseStaticFiles();

// Routing
app.UseRouting();

// CORS
app.UseCors("AllowAll");

// Authentication
// REMOVE THIS unless authentication is configured
// app.UseAuthentication();

// Authorization
app.UseAuthorization();

// Controllers
app.MapControllers();

// ---------------------------------------------------
// Run App
// ---------------------------------------------------
app.Run();