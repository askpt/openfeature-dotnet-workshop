var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres");
var database = postgres.AddDatabase("garage-db");

var migration = builder.AddProject<Projects.Garage_DatabaseSeeder>("database-seeder")
    .WithReference(database)
    .WaitFor(database);

var flagd = builder.AddFlagd("flagd", 8013)
    .WithBindFileSync("./flags");

var apiService = builder.AddProject<Projects.Garage_ApiService>("apiservice")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(cache)
    .WaitFor(cache)
    .WaitFor(migration)
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
