using Garage.ServiceDefaults;
using Garage.React.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<WinnersApiClient>(client =>
{
    // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
    client.BaseAddress = new("https+http://apiservice");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Custom middleware to inject environment variables into the index.html
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/" || context.Request.Path == "/index.html")
    {
        var config = app.Services.GetRequiredService<IConfiguration>();
        var goffConnectionString = config.GetConnectionString("goff");
        var endpoint = goffConnectionString?.Replace("Endpoint=", "") ?? "http://localhost:1031";
        
        // Ensure the endpoint has the proper OFREP path
        if (!endpoint.EndsWith("/ofrep/v1/") && !endpoint.EndsWith("/ofrep/v1"))
        {
            endpoint = endpoint.TrimEnd('/') + "/ofrep/v1/";
        }

        var html = await File.ReadAllTextAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
        var scriptTag = $@"<script>
                window.env = {{
                    OFREP_ENDPOINT: '{endpoint}'
                }};
            </script>
            </head>";
        html = html.Replace("</head>", scriptTag);

        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(html);
        return;
    }
    await next();
});

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.MapDefaultEndpoints();

app.Run();
