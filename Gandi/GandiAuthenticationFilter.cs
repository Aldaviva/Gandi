using System.Net.Http.Headers;
using Unfucked.HTTP.Filters;
using HttpHeaders = Unfucked.HTTP.HttpHeaders;

namespace Gandi;

internal class GandiAuthenticationFilter(Func<string?> authTokenProvider): ClientRequestFilter {

    private const string BearerScheme = "Bearer";
    private const string ApiKeyScheme = "Apikey";

    public ValueTask<HttpRequestMessage> Filter(HttpRequestMessage request, FilterContext context, CancellationToken cancellationToken) {
        if (!request.Headers.Contains(HttpHeaders.AUTHORIZATION) && (request.RequestUri?.BelongsToDomain(GandiClient.ApiBase.Host) ?? false) && authTokenProvider() is {} authToken) {
            request.Headers.Authorization = authToken switch {
                { Length: 24 }     => new AuthenticationHeaderValue(ApiKeyScheme, authToken), // API Key
                _ /* Length: 40 */ => new AuthenticationHeaderValue(BearerScheme, authToken)  // Personal Access Token
            };
        }
        return new ValueTask<HttpRequestMessage>(request);
    }

}