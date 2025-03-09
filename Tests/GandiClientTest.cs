using Gandi.Marshal;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unfucked.HTTP.Config;

namespace Tests;

public class GandiClientTest {

    [Fact]
    public void Construction() {
        using GandiClient gandiClient = new();

        gandiClient.HttpClient.Should().BeOfType<UnfuckedHttpClient>();
        gandiClient.HttpClient.Timeout.Should().Be(TimeSpan.FromSeconds(10));
        UnfuckedHttpClient httpClient = (UnfuckedHttpClient) gandiClient.HttpClient;
        httpClient.Handler.RequestFilters.Should().ContainItemsAssignableTo<GandiAuthenticationFilter>();
        httpClient.Handler.Property(PropertyKey.JsonSerializerOptions, out JsonSerializerOptions? jsonOptions).Should().BeTrue();
        jsonOptions!.PropertyNameCaseInsensitive.Should().BeTrue();
        jsonOptions.PropertyNamingPolicy.Should().Be(JsonNamingPolicy.SnakeCaseLower);
        jsonOptions.DefaultIgnoreCondition.Should().Be(JsonIgnoreCondition.WhenWritingNull);
        jsonOptions.Converters.Should().ContainItemsAssignableTo<JsonStringEnumConverter>();
        jsonOptions.Converters.Should().ContainItemsAssignableTo<SecondsToTimeSpanConverter>();

        gandiClient.AuthToken.Should().BeNull("not set yet");
    }

    [Fact]
    public async Task LiveDnsRequest() {
        UnfuckedHttpHandler handler     = A.Fake<UnfuckedHttpHandler>(options => options.CallsBaseMethods());
        using GandiClient   gandiClient = new("ed46842ea7f2a78ec7191373200b24b3ad1b376d", new UnfuckedHttpClient((HttpMessageHandler) handler));

        A.CallTo(() => handler.TestableSendAsync(A<HttpRequestMessage>._, A<CancellationToken>._)).Returns(new HttpResponseMessage {
            Content = new StringContent(
                """{"rrset_name":"@","rrset_type":"A","rrset_ttl":604800,"rrset_values":["67.210.32.33"],"rrset_href":"https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/@/A/api/v5/domains/aldaviva.com/records/@/A"}""",
                Encoding.UTF8, MediaTypeNames.Application.Json)
        });

        ILiveDns   liveDns = gandiClient.LiveDns("aldaviva.com");
        DnsRecord? actual  = await liveDns.Get(RecordType.A, DnsRecord.Origin);

        actual.Should().Be(new DnsRecord(RecordType.A, "@", TimeSpan.FromDays(7), "67.210.32.33"));

        A.CallTo(() => handler.TestableSendAsync(A<HttpRequestMessage>.That.Matches(req =>
            req.RequestUri == new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/@/A") &&
            req.Method == HttpMethod.Get &&
            req.Content == null &&
            req.Headers.Authorization!.Scheme == "Bearer" &&
            req.Headers.Authorization.Parameter == "ed46842ea7f2a78ec7191373200b24b3ad1b376d" &&
            req.Headers.Accept.SequenceEqual(new[] { new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json) })
        ), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public void AuthToken() {
        using GandiClient client = new("4b456787b41fa06d679d43822abcc9f11a6684e7");
        client.AuthToken.Should().Be("4b456787b41fa06d679d43822abcc9f11a6684e7");

        client.AuthToken = "ecaf24354b1ed098d3c30eae2477b32665b383fe";
        client.AuthToken.Should().Be("ecaf24354b1ed098d3c30eae2477b32665b383fe");
    }

    [Fact]
    public void AutoDisposeHttpClientIfImplicitlyCreated() {
        GandiClient client     = new("7abf9633e407ca1452c59bf0a607af8d452a2607");
        HttpClient  httpClient = client.HttpClient;
        client.Dispose();
        Action throwIfDisposed = () => httpClient.CancelPendingRequests();
        throwIfDisposed.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void DontAutoDisposeHttpClientIfExplicitlySupplied() {
        using HttpClient httpClient = new(new UnfuckedHttpHandler());
        GandiClient      client     = new("4943467edda90478d1e129f37f41230d6dc55883", httpClient);
        client.Dispose();
        httpClient.CancelPendingRequests();
    }

    [Fact]
    public void SpecifyHttpClientDisposalLogic() {
        GandiClient client     = new("7abf9633e407ca1452c59bf0a607af8d452a2607", disposeHttpClient: false);
        HttpClient  httpClient = client.HttpClient;
        client.Dispose();
        httpClient.CancelPendingRequests();

        client = new GandiClient("4943467edda90478d1e129f37f41230d6dc55883", httpClient, true);
        client.Dispose();
        Action throwIfDisposed = () => httpClient.CancelPendingRequests();
        throwIfDisposed.Should().Throw<ObjectDisposedException>();
    }

}