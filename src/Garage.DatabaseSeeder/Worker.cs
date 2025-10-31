using System.Diagnostics;

namespace Garage.DatabaseSeeder;

public class Worker : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    private readonly ILogger<Worker> _logger;
    private readonly DatabaseSeederService _databaseSeederService;

    public Worker(ILogger<Worker> logger, DatabaseSeederService databaseSeederService)
    {
        _logger = logger;
        _databaseSeederService = databaseSeederService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);
        try
        {
            await _databaseSeederService.SeedDatabaseAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database");
            activity?.AddException(ex);
            throw;
        }
    }
}
