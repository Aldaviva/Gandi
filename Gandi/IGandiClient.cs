using Gandi.Dns;

namespace Gandi;

public interface IGandiClient: IDisposable {

    HttpClient HttpClient { get; }
    string? AuthToken { get; set; }

    ILiveDns LiveDns(string domain);

}