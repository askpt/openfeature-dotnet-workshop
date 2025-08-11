namespace Garage.ServiceDefaults.Services;

public class FeatureFlags : IFeatureFlags
{
    private readonly Dictionary<string, object> _flags = new()
    {
        { nameof(SlowOperationDelay), 1000 },
        { nameof(EnableDatabaseWinners), false },
        { nameof(EnableStatsHeader), true }
    };
    
    public int SlowOperationDelay => _flags.TryGetValue(nameof(SlowOperationDelay), out var value) ? (int)value : 1000;
    public bool EnableDatabaseWinners => _flags.TryGetValue(nameof(EnableDatabaseWinners), out var value) && (bool)value;
    public bool EnableStatsHeader => _flags.TryGetValue(nameof(EnableStatsHeader), out var value) && (bool)value;
}