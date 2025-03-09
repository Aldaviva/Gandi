using Gandi.Dns;

namespace Gandi;

/// <summary>
/// <para>HTTP REST client for the Gandi v5 API.</para>
/// <para>To get started, construct a new instance of <see cref="GandiClient"/>, passing your personal access token, and start calling methods on <see cref="LiveDns"/>.</para>
/// <para>Reference: <see href="https://api.gandi.net/docs/reference/"/></para>
/// </summary>
public interface IGandiClient: IDisposable {

    /// <summary>
    /// <para>The HTTP client used by this library to make REST requests. It is exposed here so you can customize its behavior, such as changing timeouts and adding default headers.</para>
    /// <para>By default, this is constructed automatically, but you can specify a custom one (with restrictions) by passing it to the <see cref="GandiClient(string?,System.Net.Http.HttpClient?,bool?)"/>
    /// constructor.</para>
    /// </summary>
    HttpClient HttpClient { get; }

    /// <summary>
    /// <para>Personal access Token for your user or organization which has permissions to manage your domain name technical configurations, created using
    /// <see href="https://admin.gandi.net/organizations/account/pat"/> or <see href="https://admin.gandi.net/organizations/"/>.</para>
    /// <para>Can also be an API Key, although those can't be created anymore if you don't already have one.</para>
    /// </summary>
    string? AuthToken { get; set; }

    /// <summary>
    /// Get a client for the LiveDNS API with the specified domain name.
    /// </summary>
    /// <param name="domain">Fully-qualified second-level domain name that is registered with and has its DNS managed by Gandi.</param>
    /// <returns>Client object that can call REST methods on the LiveDNS API for the specified domain.</returns>
    ILiveDns LiveDns(string domain);

}