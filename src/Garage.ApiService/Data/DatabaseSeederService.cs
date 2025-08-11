using System.Text.Json;
using Garage.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Garage.ApiService.Data;

public class DatabaseSeederService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseSeederService> _logger;
    private readonly IWebHostEnvironment _environment;

    public DatabaseSeederService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseSeederService> logger,
        IWebHostEnvironment environment)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _environment = environment;
    }

    public async Task SeedDatabaseAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GarageDbContext>();

        try
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (await context.Winners.AnyAsync())
            {
                _logger.LogInformation("Database already contains winner data, skipping seed");
                return;
            }

            // Read JSON data
            var jsonFilePath = Path.Combine(_environment.ContentRootPath, "Data", "winners.json");
            var jsonData = await File.ReadAllTextAsync(jsonFilePath);
            var winners = JsonSerializer.Deserialize<Winner[]>(jsonData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (winners == null || winners.Length == 0)
            {
                _logger.LogWarning("No winner data found in JSON file");
                return;
            }

            // Add winners to database
            await context.Winners.AddRangeAsync(winners);
            await context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded database with {Count} Le Mans winners", winners.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed database with winner data");
            throw;
        }
    }
}
