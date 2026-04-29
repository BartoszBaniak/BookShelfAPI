using BookShelfAPI.Application;
using BookShelfAPI.Application.Common;
using BookShelfAPI.Domain.Repositories;
using BookShelfAPI.Domain.Services;
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "BookShelf API",
        Version = "v1",
        Description = "Personal book library management API.",
    });
});

var app = builder.Build();

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

if (app.Configuration.GetValue("SeedData", true))
{
    using var scope = app.Services.CreateScope();
    var repository = scope.ServiceProvider.GetRequiredService<IBookRepository>();
    var isbnChecker = scope.ServiceProvider.GetRequiredService<IBookIsbnUniquenessChecker>();
    await BookSeeder.SeedAsync(repository, isbnChecker);
}

app.Run();

public partial class Program;
