var builder = DistributedApplication.CreateBuilder(args);

// Add Azure Container App Environment for publishing
var containerAppEnvironment = builder
    .AddAzureContainerAppEnvironment("cae");

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres");
var database = postgres.AddDatabase("garage-db");

// var flagd = builder.AddContainer("flagd", "ghcr.io/open-feature/flagd:latest")
//     // .WithBindMount("flags", "/flags")
//     .WithVolume("flags", "/flags_volume")
//     // .WithArgs("start", "--uri", "file:./flags/flagd.json")
//     .WithArgs("start", "--uri", "file:./flags_volume/flagd.json")
//     .WithEndpoint(8013, 8013);

// Only add flagd service for local development (not during publishing/deployment)
var isLocalDevelopment = !builder.ExecutionContext.IsPublishMode;
var flagd = isLocalDevelopment
    ? builder.AddFlagd("flagd")
        .WithBindFileSync("./flags")
    : null;

var ofrepEndpoint = flagd?.GetEndpoint("ofrep");

var serverKey = builder.AddParameter("devcycle-server-key", secret: true);
var devcycleUrl = builder.Configuration["DevCycle:Url"] ?? "null";

var apiServiceBuilder = builder.AddProject<Projects.Garage_ApiService>("apiservice")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(cache)
    .WithEnvironment("DEVCYCLE__URL", devcycleUrl)
    .WithEnvironment("DEVCYCLE__SERVERKEY", serverKey)
    .WaitFor(cache)
    .PublishAsAzureContainerApp((infra, app) =>
    {

    });

// Only reference flagd in development
if (isLocalDevelopment && flagd != null && ofrepEndpoint != null)
{
    apiServiceBuilder = apiServiceBuilder
        .WithReference(ofrepEndpoint)
        .WaitFor(flagd);
}

var apiService = apiServiceBuilder.WithHttpHealthCheck("/health");

var webFrontendBuilder = builder.AddNpmApp("webfrontend", "../Garage.React/");

// Only reference flagd in development
if (isLocalDevelopment && flagd != null && ofrepEndpoint != null)
{
    webFrontendBuilder = webFrontendBuilder
        .WithReference(ofrepEndpoint)
        .WaitFor(flagd);
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
