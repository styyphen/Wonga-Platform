using Npgsql;
using Wonga.Shared.Security;
using Wonga.Services.UserProfile.Api.Features.Users.V1;
using Wonga.Services.UserProfile.Application;
using Wonga.Services.UserProfile.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var userProfileDbConnectionString =
    builder.Configuration.GetConnectionString("UserProfileDb")
    ?? throw new InvalidOperationException("Connection string 'UserProfileDb' is required.");

var accessTokenOptions = AccessTokenOptionsConfiguration.Build(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddHttpLogging(_ => { });
builder.Services.AddHealthChecks();
builder.Services.AddSingleton(new NpgsqlDataSourceBuilder(userProfileDbConnectionString).Build());
builder.Services.AddSingleton(accessTokenOptions);
builder.Services.AddSingleton<UserProfileDatabaseInitializer>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IIdentityTokenValidator, IdentityTokenValidator>();
builder.Services.AddScoped<UserProfileApplicationService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpLogging();

using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<UserProfileDatabaseInitializer>();
    await databaseInitializer.InitializeAsync(CancellationToken.None);
}

app.MapGet("/", () => Results.Ok(new
{
    service = "user-profile-service",
    status = "initialized"
}));

app.MapGetCurrentUserEndpoint();
app.MapUpsertProfileInternalEndpoint();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
