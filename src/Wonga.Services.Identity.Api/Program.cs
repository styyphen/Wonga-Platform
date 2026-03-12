using Npgsql;
using Wonga.Shared.Security;
using Wonga.Services.Identity.Api.Features.Authentication.V1;
using Wonga.Services.Identity.Application;
using Wonga.Services.Identity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var identityDbConnectionString =
    builder.Configuration.GetConnectionString("IdentityDb")
    ?? throw new InvalidOperationException("Connection string 'IdentityDb' is required.");

var accessTokenOptions = AccessTokenOptionsConfiguration.Build(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddHttpLogging(_ => { });
builder.Services.AddHealthChecks();
builder.Services.AddSingleton(new NpgsqlDataSourceBuilder(identityDbConnectionString).Build());
builder.Services.AddSingleton(accessTokenOptions);
builder.Services.AddSingleton<IdentityDatabaseInitializer>();
builder.Services.AddScoped<IIdentityRepository, IdentityRepository>();
builder.Services.AddScoped<IdentityApplicationService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpLogging();

using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<IdentityDatabaseInitializer>();
    await databaseInitializer.InitializeAsync(CancellationToken.None);
}

app.MapGet("/", () => Results.Ok(new
{
    service = "identity-service",
    status = "initialized"
}));

app.MapRegisterEndpoint();
app.MapLoginEndpoint();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
