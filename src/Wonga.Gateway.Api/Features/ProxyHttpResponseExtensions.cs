using System.Net.Http.Json;

namespace Wonga.Gateway.Api.Features;

internal static class ProxyHttpResponseExtensions
{
    internal static async Task<IResult> ProxyPostAsJsonAsync<TRequest>(
        this IHttpClientFactory httpClientFactory,
        string clientName,
        string path,
        TRequest request,
        string serviceName,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClientFactory
                .CreateClient(clientName)
                .PostAsJsonAsync(path, request, cancellationToken);

            return await response.ToResultAsync(cancellationToken);
        }
        catch (HttpRequestException)
        {
            return ToServiceUnavailableResult(serviceName);
        }
    }

    internal static async Task<IResult> ToResultAsync(this HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";

        return Results.Content(content, contentType, statusCode: (int)response.StatusCode);
    }

    internal static IResult ToServiceUnavailableResult(string serviceName) =>
        Results.Problem(
            title: "Downstream service unavailable.",
            detail: $"The {serviceName} could not be reached.",
            statusCode: StatusCodes.Status502BadGateway);
}
