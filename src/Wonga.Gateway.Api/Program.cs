using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddHttpLogging(_ => { });
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi("v1");

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
    routes = Array.Empty<string>(),
    developerUi = environment.IsDevelopment()
        ? new[] { "/scalar", "/openapi/v1.json" }
        : []
}))
.ExcludeFromDescription();

app.MapHealthChecks("/health").ExcludeFromDescription();

app.Run();

public partial class Program;
