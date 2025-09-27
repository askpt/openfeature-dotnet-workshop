using Aspire.Hosting.ApplicationModel;
using CommunityToolkit.Aspire.Hosting.Flagd;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding flagd resources to an <see cref="IDistributedApplicationBuilder"/>.
/// </summary>
public static class FlagdBuilderExtensions
{
    private const int FlagdPort = 8013;
    private const int HealthCheckPort = 8014;

    /// <summary>
    /// Adds a flagd container to the application model.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
    /// <param name="port">The host port for flagd HTTP endpoint. If not provided, a random port will be assigned.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{FlagdResource}"/>.</returns>
    public static IResourceBuilder<FlagdResource> AddFlagd(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name,
        int? port = null)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));

        var resource = new FlagdResource(name);

        return builder.AddResource(resource)
            .WithImage(FlagdContainerImageTags.Image, FlagdContainerImageTags.Tag)
            .WithImageRegistry(FlagdContainerImageTags.Registry)
            .WithHttpEndpoint(port: port, targetPort: FlagdPort, name: FlagdResource.HttpEndpointName)
            .WithHttpEndpoint(null, HealthCheckPort, FlagdResource.HealthCheckEndpointName)
            .WithHttpHealthCheck("/healthz", endpointName: FlagdResource.HealthCheckEndpointName)
            .WithArgs("start");
    }

    /// <summary>
    /// Configures logging level for flagd. If a flag or targeting rule isn't proceeding the way you'd expect this can be enabled to get more verbose logging.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <returns>The <see cref="IResourceBuilder{FlagdResource}"/>.</returns>
    public static IResourceBuilder<FlagdResource> WithLogging(
        this IResourceBuilder<FlagdResource> builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        return builder.WithEnvironment("FLAGD_DEBUG", "true");
    }

    public static IResourceBuilder<FlagdResource> WithFlagdVolume(
        this IResourceBuilder<FlagdResource> builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        return builder.WithVolume("flags", "/flags_volume").WithArgs("--uri", "file:./flags_volume/flagd.json");
    }
}
