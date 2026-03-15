using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Wonga.IntegrationTests;

public sealed class GatewaySmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public GatewaySmokeTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task RootEndpoint_ReturnsGatewayMetadata()
    {
        using var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var contentStream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(contentStream);
        var root = document.RootElement;

        Assert.Equal("gateway", root.GetProperty("service").GetString());
        Assert.Equal("initialized", root.GetProperty("status").GetString());

        var routes = root.GetProperty("routes")
            .EnumerateArray()
            .Select(route => route.GetString())
            .ToArray();

        Assert.Contains("/identity/register", routes);
        Assert.Contains("/identity/login", routes);
        Assert.Contains("/users/me", routes);
    }
}
