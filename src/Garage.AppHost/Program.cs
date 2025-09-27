var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres");
var database = postgres.AddDatabase("garage-db");

// var flagd = builder.AddContainer("flagd", "ghcr.io/open-feature/flagd:latest")
//     // .WithBindMount("flags", "/flags")
//     .WithVolume("flags", "/flags_volume")
//     // .WithArgs("start", "--uri", "file:./flags/flagd.json")
//     .WithArgs("start", "--uri", "file:./flags_volume/flagd.json")
//     .WithEndpoint(8013, 8013);
var flagd = builder.AddFlagd("flagd").WithLogging().WithFlagdVolume();

var apiService = builder.AddProject<Projects.Garage_ApiService>("apiservice")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(flagd)
    .WaitFor(flagd)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Garage_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(flagd)
    .WaitFor(flagd)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
