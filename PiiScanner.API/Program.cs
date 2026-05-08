var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "API WORKING");

app.Run();
