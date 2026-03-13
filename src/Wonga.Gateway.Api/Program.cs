using Scalar.AspNetCore;
using Wonga.Gateway.Api.Features.Identity.V1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddHttpLogging(_ => { });
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi("v1");
builder.Services.AddHttpClient("identity-service", (serviceProvider, httpClient) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl =
        configuration["Gateway:Routes:Identity"]
        ?? "http://identity-service:8080";

    httpClient.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/{documentName}.json");
    app.MapScalarApiReference("/scalar", options =>
    {
        options.WithTitle("Wonga Gateway API");
        options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
    })
    .AllowAnonymous();
}

app.MapGet("/", (IWebHostEnvironment environment) => Results.Ok(new
{
    service = "gateway",
    status = "initialized",
    routes = new[]
    {
        "/identity/register",
        "/identity/login"
    },
    developerUi = environment.IsDevelopment()
        ? new[] { "/scalar", "/openapi/v1.json" }
        : []
}))
.ExcludeFromDescription();

app.MapRegisterEndpoint();
app.MapLoginEndpoint();
app.MapHealthChecks("/health").ExcludeFromDescription();

app.Run();

public partial class Program;
