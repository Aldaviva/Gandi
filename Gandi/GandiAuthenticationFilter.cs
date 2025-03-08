using System.Net.Http.Headers;
using Unfucked.HTTP.Filters;
using HttpHeaders = Unfucked.HTTP.HttpHeaders;

namespace Gandi;

internal class GandiAuthenticationFilter(Func<string?> authTokenProvider): ClientRequestFilter {

    private const string BearerScheme = "Bearer";
    private const string ApiKeyScheme = "Apikey";

    public ValueTask Filter(ref HttpRequestMessage request, CancellationToken cancellationToken) {
        HttpRequestHeaders headers = request.Headers;
        if (!headers.Contains(HttpHeaders.Authorization) && request.RequestUri is { } requestUri && GandiClient.ApiBase.IsBaseOf(requestUri) && authTokenProvider() is { } authToken) {
            headers.Authorization = authToken switch {
                { Length: 24 }     => new AuthenticationHeaderValue(ApiKeyScheme, authToken), // API Key
                _ /* Length: 40 */ => new AuthenticationHeaderValue(BearerScheme, authToken)  // Personal Access Token
            };
        }
        return default;
    }

}