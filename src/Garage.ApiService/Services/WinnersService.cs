using System.Text.Json;
using Garage.ApiService.Data;
using Garage.ServiceDefaults.Services;
using Garage.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Garage.ApiService.Services;

public class WinnersService(
    GarageDbContext context,
    IWebHostEnvironment environment,
    ILogger<WinnersService> logger,
    IFeatureFlags featureFlags)
    : IWinnersService
{
    public async Task<IEnumerable<Winner>> GetAllWinnersAsync()
    {
        return featureFlags.EnableDatabaseWinners ? await GetAllDatabaseWinnersAsync() : await GetAllJsonWinnersAsync();
    }

    private async Task<IEnumerable<Winner>> GetAllDatabaseWinnersAsync()
    {
        try
        {
            var winnersDatabase = await context.Winners
                .OrderByDescending(w => w.Year)
                .ToListAsync();

            var mapper = new WinnerMapper();

            return winnersDatabase.Select(mapper.WinnerToWinnerDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve all Le Mans winners");
            return [];
        }
    }

    private async Task<IEnumerable<Winner>> GetAllJsonWinnersAsync()
    {
        SlowDown();
        var dataFilePath = Path.Combine(environment.ContentRootPath, "Data", "winners.json");
        try
        {
            var jsonData = await File.ReadAllTextAsync(dataFilePath);
            var winners = JsonSerializer.Deserialize<Winner[]>(jsonData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return winners?.OrderByDescending(w => w.Year) ?? Enumerable.Empty<Winner>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read winners data from JSON file: {FilePath}", dataFilePath);
            return [];
        }
    }

    private void SlowDown()
    {
        // Simulate a slow operation
        Thread.Sleep(featureFlags.SlowOperationDelay);
    }
}
