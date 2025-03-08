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
        using GandiClient   gandiClient = new(new UnfuckedHttpClient((HttpMessageHandler) handler)) { AuthToken = "ed46842ea7f2a78ec7191373200b24b3ad1b376d" };

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

}