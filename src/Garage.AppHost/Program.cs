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
var goff = builder.AddGoFeatureFlag("goff")
    .WithGoffBindMount("./goff")
    .WithDataVolume();

var apiService = builder.AddProject<Projects.Garage_ApiService>("apiservice")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(goff)
    .WaitFor(goff)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Garage_React>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(goff)
    .WaitFor(goff)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
