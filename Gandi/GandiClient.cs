using Gandi.Dns;
using Gandi.Marshal;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unfucked.HTTP;
using Unfucked.HTTP.Config;

namespace Gandi;

public class GandiClient: IGandiClient {

    internal static readonly Uri                  ApiBase             = new("https://api.gandi.net/v5/");
    internal static readonly MediaTypeHeaderValue ApplicationJsonType = new("application/json");

    internal static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) {
        PropertyNamingPolicy   = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = {
            new JsonStringEnumConverter(),
            new SecondsToTimeSpanConverter()
        }
    };

    /// <inheritdoc />
    public HttpClient HttpClient { get; }

    /// <inheritdoc />
    public string? AuthToken { get; set; }

    public GandiClient(HttpClient? httpClient = null) {
        HttpClient = (httpClient ?? new UnfuckedHttpClient { Timeout = TimeSpan.FromSeconds(10) })
            .Register(new GandiAuthenticationFilter(() => AuthToken))
            .Property(PropertyKey.JsonSerializerOptions, JsonOptions);
    }

    /// <inheritdoc />
    public ILiveDns LiveDns(string domain) => new LiveDns(this, domain);

    /// <inheritdoc cref="Dispose()" />
    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            HttpClient.Dispose();
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

}