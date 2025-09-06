namespace Garage.ServiceDefaults.Services;

public class FeatureFlags : IFeatureFlags
{
    public int SlowOperationDelay => int.TryParse(Environment.GetEnvironmentVariable("SLOW_OPERATION_DELAY"), out var delay) ? delay : 1000;
    public bool EnableDatabaseWinners => bool.TryParse(Environment.GetEnvironmentVariable("ENABLE_DATABASE_WINNERS"), out var enable) && enable;
    public bool EnableStatsHeader => !bool.TryParse(Environment.GetEnvironmentVariable("ENABLE_STATS_HEADER"), out var enable) || enable; // Default to true
}
