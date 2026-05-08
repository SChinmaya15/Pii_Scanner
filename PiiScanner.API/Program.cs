using Microsoft.OpenApi.Models;
using PiiScanner.Application.Service;
using PiiScanner.Domain.Interface;
using PiiScanner.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var dataPath = Path.Combine(
    Environment.GetEnvironmentVariable("HOME") ?? "/home",
    "site",
    "wwwroot",
    "Develop");

builder.Services.AddScoped<IDataConnector>(
    sp => new LocalFileConnector(dataPath));

builder.Services.AddScoped<IPiiDetector, RegexPiiDetector>();
builder.Services.AddScoped<ScanService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
