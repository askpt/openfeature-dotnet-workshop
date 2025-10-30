var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var database = builder.AddSqlite("garage-db");

var flagd = builder.AddFlagd("flagd", 8013)
    .WithBindFileSync("./flags");

var apiService = builder.AddProject<Projects.Garage_ApiService>("apiservice")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(cache)
    .WaitFor(cache)
    .WaitFor(flagd)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Garage_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(flagd)
    .WaitFor(apiService);

builder.Build().Run();
