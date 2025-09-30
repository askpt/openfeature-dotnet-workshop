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

// Only add goff service for local development (not during publishing/deployment)
var isLocalDevelopment = !builder.ExecutionContext.IsPublishMode;
var goff = isLocalDevelopment
    ? builder.AddGoFeatureFlag("goff")
        .WithGoffBindMount("./goff")
    : null;

var serverKey = builder.AddParameter("devcycle-server-key", secret: true);
var devcycleUrl = builder.Configuration["DevCycle:Url"] ?? "null";

var apiServiceBuilder = builder.AddProject<Projects.Garage_ApiService>("apiservice")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(cache)
    .WithEnvironment("DEVCYCLE__URL", devcycleUrl)
    .WithEnvironment("DEVCYCLE__SERVERKEY", serverKey)
    .WaitFor(cache);

// Only reference goff in development
if (isLocalDevelopment && goff != null)
{
    apiServiceBuilder = apiServiceBuilder
        .WithReference(goff)
        .WaitFor(goff);
}

var apiService = apiServiceBuilder.WithHttpHealthCheck("/health");

var webFrontendBuilder = builder.AddNpmApp("webfrontend", "../Garage.React/");

// Only reference goff in development
if (isLocalDevelopment && goff != null)
{
    webFrontendBuilder = webFrontendBuilder
        .WithReference(goff)
        .WaitFor(goff);
}

webFrontendBuilder
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithEnvironment("DEVCYCLE__URL", devcycleUrl)
    .WithEnvironment("DEVCYCLE__SERVERKEY", serverKey)
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
