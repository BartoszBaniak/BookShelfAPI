using BookShelfAPI.Application;
using BookShelfAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("BookShelfDb")
                       ?? throw new InvalidOperationException("Connection string 'BookShelfDb' is not configured.");

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

app.MapControllers();

app.Run();
