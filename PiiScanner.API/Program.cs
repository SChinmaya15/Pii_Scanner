using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PII Scanner API",
        Version = "v1"
    });
});

var app = builder.Build();

// TEST ROOT
app.MapGet("/", () => "API RUNNING SUCCESSFULLY");

// Swagger
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PII Scanner API V1");
});

// Controllers
app.MapControllers();

app.Run();
