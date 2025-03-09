using Gandi.Dns;
using Gandi.Marshal;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unfucked.HTTP;
using Unfucked.HTTP.Config;

namespace Gandi;

/// <summary>
/// <para>HTTP REST client for the Gandi v5 API.</para>
/// <para>To get started, construct a new instance of <see cref="GandiClient"/>, passing your personal access token, and start calling methods on <see cref="LiveDns"/>.</para>
/// <para>Reference: <see href="https://api.gandi.net/docs/reference/"/></para>
/// </summary>
public class GandiClient: IGandiClient {

    // ExceptionAdjustment: M:System.Uri.#ctor(System.String) -T:System.UriFormatException
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

    private readonly bool _disposeHttpClient;

    /// <inheritdoc />
    public HttpClient HttpClient { get; }

    /// <inheritdoc />
    public string? AuthToken { get; set; }

    /// <summary>
    /// <para>Create a new REST client for the Gandi v5 API.</para>
    /// <para>An instance has at most one auth token, therefore if you have multiple Personal Access Tokens or API Keys, then you will need to construct multiple instances (thread-safe) or change <see cref="AuthToken"/> (not thread-safe).</para>
    /// </summary>
    /// <param name="authToken"><para>Personal access Token for your user or organization which has permissions to manage your domain name technical configurations, created using
    /// <see href="https://admin.gandi.net/organizations/account/pat"/> or <see href="https://admin.gandi.net/organizations/"/>.</para>
    /// <para>Can also be an API Key, although those can't be created anymore if you don't already have one.</para></param>
    /// <param name="httpClient"><para>By default, when this is <c>null</c>, an <see cref="HttpClient"/> is automatically constructed and used by this class. You can modify its properties (like
    /// timeouts and default headers) by accessing the <see cref="HttpClient"/> property.</para>
    /// <para>If you need to supply a completely custom instance, you can pass it to this parameter, although the <see cref="HttpMessageHandler"/> must be or delegate to an
    /// <see cref="UnfuckedHttpHandler"/> so that authentication request filtering and JSON serialization work properly.</para></param>
    /// <param name="disposeHttpClient"><para>Whether the <see cref="HttpClient"/> should be disposed when <see cref="IDisposable.Dispose"/> is called on this <see cref="GandiClient"/>
    /// instance.</para>
    /// <para>When <c>null</c>, defaults to <c>true</c> when the default <see cref="System.Net.Http.HttpClient"/> is automatically created because <paramref name="httpClient"/> was <c>null</c>, and
    /// defaults to <c>false</c> when a custom instance was passed in to <paramref name="httpClient"/>.</para></param>
    public GandiClient(string? authToken = null, HttpClient? httpClient = null, bool? disposeHttpClient = null) {
        AuthToken          = authToken;
        _disposeHttpClient = disposeHttpClient ?? httpClient is null;
        HttpClient = (httpClient ?? new UnfuckedHttpClient { Timeout = TimeSpan.FromSeconds(10) })
            .Register(new GandiAuthenticationFilter(() => AuthToken))
            .Property(PropertyKey.JsonSerializerOptions, JsonOptions);
    }

    /// <inheritdoc />
    public ILiveDns LiveDns(string domain) => new LiveDns(this, domain);

    /// <inheritdoc cref="Dispose()" />
    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            if (_disposeHttpClient) {
                HttpClient.Dispose();
            }
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

}