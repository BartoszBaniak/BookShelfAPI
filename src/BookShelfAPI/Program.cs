using BookShelfAPI.Application;
using BookShelfAPI.Infrastructure;
using BookShelfAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("BookShelfDb")
                       ?? throw new InvalidOperationException("Connection string 'BookShelfDb' is not configured.");

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapControllers();

app.Run();
