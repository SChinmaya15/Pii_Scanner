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
// Dependency Injection
// ---------------------------------------------------
builder.Services.AddScoped<IDataConnector>(
    sp => new LocalFileConnector(dataPath));

builder.Services.AddScoped<IPiiDetector, RegexPiiDetector>();
builder.Services.AddScoped<ScanService>();

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
// Swagger / OpenAPI
// ---------------------------------------------------
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PII Scanner API",
        Version = "v1"
    });

    // JWT Auth Support in Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            Array.Empty<string>()
        }
    });
});

// ---------------------------------------------------
// Build App
// ---------------------------------------------------
var app = builder.Build();

// ---------------------------------------------------
// Middleware Pipeline
// ---------------------------------------------------

// TEMP TEST ROUTE
app.MapGet("/", () => "PII Scanner API Running Successfully");

// Swagger
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PII Scanner API V1");
    c.RoutePrefix = "swagger";
});

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
