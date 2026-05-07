using Microsoft.OpenApi.Models;
using PiiScanner.Application.Service;
using PiiScanner.Domain.Interface;
using PiiScanner.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IDataConnector>(sp => new LocalFileConnector("C:\\Develop"));
builder.Services.AddScoped<IPiiDetector, RegexPiiDetector>();
builder.Services.AddScoped<ScanService>();

// Configure CORS to resolve cross-origin issues during development/testing.
// This policy is permissive (allows any origin/method/header). For production
// consider restricting the origins and enabling credentials if needed.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                      });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Add JWT Bearer definition so Swagger UI shows an Authorize button
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer abcdef12345\"",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "Bearer" } }
    };

    c.AddSecurityRequirement(securityRequirement);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Enable CORS globally using the defined policy. Must be placed before UseAuthorization and MapControllers.
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
