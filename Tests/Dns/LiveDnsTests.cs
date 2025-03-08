using System.Net.Mime;
using Unfucked.HTTP.Config;

namespace Tests.Dns;

public class LiveDnsTests {

    private readonly IGandiClient       _gandi = A.Fake<IGandiClient>();
    private readonly LiveDns            _liveDns;
    private readonly UnfuckedHttpClient _httpClient = A.Fake<UnfuckedHttpClient>();

    public LiveDnsTests() {
        _httpClient.Property(PropertyKey.JsonSerializerOptions, GandiClient.JsonOptions);
        A.CallTo(() => _gandi.HttpClient).Returns(_httpClient);
        _liveDns = new LiveDns(_gandi, "aldaviva.com");
    }

    [Fact]
    public async Task ListAll() {
        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>._, A<CancellationToken>._)).Returns(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(
                """[{"rrset_name":"@","rrset_type":"A","rrset_ttl":300,"rrset_values":["67.210.32.33"],"rrset_href":"https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/%40/A"},{"rrset_name":"west","rrset_type":"A","rrset_ttl":300,"rrset_values":["172.11.57.29"],"rrset_href":"https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/west/A"}]""",
                Encoding.UTF8, MediaTypeNames.Application.Json)
        });

        IEnumerable<DnsRecord> actual = await _liveDns.List();

        actual.Should().BeEquivalentTo([
            new DnsRecord(RecordType.A, "@", TimeSpan.FromMinutes(5), "67.210.32.33"),
            new DnsRecord(RecordType.A, "west", TimeSpan.FromMinutes(5), "172.11.57.29")
        ]);

        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>.That.Matches(req =>
            req.Equals(new HttpRequest(HttpMethod.Get, new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records"),
                new[] { new KeyValuePair<string, string>("Accept", MediaTypeNames.Application.Json) }, null))
        ), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ListByType() {
        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>._, A<CancellationToken>._)).Returns(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(
                """[{"rrset_name":"@","rrset_type":"A","rrset_ttl":300,"rrset_values":["67.210.32.33"],"rrset_href":"https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/%40/A"},{"rrset_name":"west","rrset_type":"A","rrset_ttl":300,"rrset_values":["172.11.57.29"],"rrset_href":"https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/west/A"}]""",
                Encoding.UTF8, MediaTypeNames.Application.Json)
        });

        IEnumerable<DnsRecord> actual = await _liveDns.List(RecordType.A);

        actual.Should().BeEquivalentTo([
            new DnsRecord(RecordType.A, "@", TimeSpan.FromMinutes(5), "67.210.32.33"),
            new DnsRecord(RecordType.A, "west", TimeSpan.FromMinutes(5), "172.11.57.29")
        ]);

        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>.That.Matches(req =>
            req.Equals(new HttpRequest(HttpMethod.Get, new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records?rrset_type=A"),
                new[] { new KeyValuePair<string, string>("Accept", MediaTypeNames.Application.Json) }, null))
        ), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ListByName() {
        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>._, A<CancellationToken>._)).Returns(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(
                """[{"rrset_name":"west","rrset_type":"A","rrset_ttl":300,"rrset_values":["172.11.57.29"],"rrset_href":"https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/west/A"}]""",
                Encoding.UTF8, MediaTypeNames.Application.Json)
        });

        IEnumerable<DnsRecord> actual = await _liveDns.List(name: "west");

        actual.Should().BeEquivalentTo([
            new DnsRecord(RecordType.A, "west", TimeSpan.FromMinutes(5), "172.11.57.29")
        ]);

        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>.That.Matches(req =>
            req.Equals(new HttpRequest(HttpMethod.Get, new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/west"),
                new[] { new KeyValuePair<string, string>("Accept", "application/json") }, null))
        ), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ListByTypeAndName() {
        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>._, A<CancellationToken>._)).Returns(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(
                """{"rrset_name":"west","rrset_type":"A","rrset_ttl":300,"rrset_values":["172.11.57.29"],"rrset_href":"https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/west/A"}""",
                Encoding.UTF8, MediaTypeNames.Application.Json)
        });

        IEnumerable<DnsRecord> actual = await _liveDns.List(RecordType.A, "west");

        actual.Should().BeEquivalentTo([
            new DnsRecord(RecordType.A, "west", TimeSpan.FromMinutes(5), "172.11.57.29")
        ]);

        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>.That.Matches(req =>
            req.Equals(new HttpRequest(HttpMethod.Get, new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/west/A"),
                new[] { new KeyValuePair<string, string>("Accept", "application/json") }, null))
        ), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Get() {
        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>._, A<CancellationToken>._)).Returns(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(
                """{"rrset_name":"west","rrset_type":"A","rrset_ttl":300,"rrset_values":["172.11.57.29"],"rrset_href":"https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/west/A"}""",
                Encoding.UTF8, MediaTypeNames.Application.Json)
        });

        DnsRecord? actual = await _liveDns.Get(RecordType.A, "west");

        actual.Should().Be(new DnsRecord(RecordType.A, "west", TimeSpan.FromMinutes(5), "172.11.57.29"));

        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>.That.Matches(req =>
            req.Equals(new HttpRequest(HttpMethod.Get, new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/west/A"),
                new[] { new KeyValuePair<string, string>("Accept", "application/json") }, null))
        ), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task GetMissing() {
        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>._, A<CancellationToken>._)).Returns(new HttpResponseMessage(HttpStatusCode.NotFound));

        DnsRecord? actual = await _liveDns.Get(RecordType.A, "missing");

        actual.Should().BeNull();

        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>.That.Matches(req =>
            req.Equals(new HttpRequest(HttpMethod.Get, new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/missing/A"),
                new[] { new KeyValuePair<string, string>("Accept", "application/json") }, null))
        ), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Delete() {
        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>._, A<CancellationToken>._)).Returns(new HttpResponseMessage(HttpStatusCode.NoContent));

        await _liveDns.Delete(new DnsRecord(RecordType.A, "deleteme"));

        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>.That.Matches(req =>
            req.Equals(new HttpRequest(HttpMethod.Delete, new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/deleteme/A"),
                new[] { new KeyValuePair<string, string>("Accept", "application/json") }, null))
        ), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Set() {
        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>._, A<CancellationToken>._)).Returns(new HttpResponseMessage(HttpStatusCode.Created) {
            Content = new StringContent("""{"message":"DNS Record Created"}""", Encoding.UTF8, MediaTypeNames.Application.Json),
            Headers = { Location = new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/_test/TXT") }
        });

        await _liveDns.Set(new DnsRecord(RecordType.TXT, "_test", null, "hi"));

        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>.That.Matches(req =>
            req.Equals(new HttpRequest(HttpMethod.Put, new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records/_test/TXT"),
                new[] { new KeyValuePair<string, string>("Accept", "application/json") },
                new StringContent("""{"rrset_type":"TXT","rrset_name":"_test","rrset_values":["hi"]}""", Encoding.UTF8, MediaTypeNames.Application.Json)))
        ), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task SetRequireValue() {
        Func<Task> thrower = async () => await _liveDns.Set(new DnsRecord(RecordType.TXT, "_test"));
        await thrower.Should().ThrowAsync<ArgumentOutOfRangeException>();

        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task ErrorResponses() {
        A.CallTo(() => _httpClient.SendAsync(A<HttpRequest>._, A<CancellationToken>._)).ReturnsLazily(() => new HttpResponseMessage(HttpStatusCode.Forbidden));

        IEnumerable<Func<Task>> throwers = [
            async () => await _liveDns.List(),
            async () => await _liveDns.Get(RecordType.A, "_test"),
            async () => await _liveDns.Set(new DnsRecord(RecordType.A, "_test", null, "127.0.0.1")),
            async () => await _liveDns.Delete(RecordType.A, "_test"),
        ];

        foreach (Func<Task> thrower in throwers) {
            await thrower.Should().ThrowAsync<GandiException>();
        }
    }

}