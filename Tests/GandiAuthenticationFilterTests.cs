using System.Net.Http.Headers;
using Unfucked.HTTP.Filters;

namespace Tests;

public class GandiAuthenticationFilterTests {

    [Fact]
    public async Task ApiKey() {
        const string              apiKey  = "dZiBqfkRztGO9nzXp1G6vxax";
        GandiAuthenticationFilter filter  = new(() => apiKey);
        HttpRequestMessage        request = new(HttpMethod.Get, new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records"));
        await filter.Filter(request, new FilterContext(), CancellationToken.None);
        AuthenticationHeaderValue? actual = request.Headers.Authorization;
        actual.Should().NotBeNull();
        actual.Scheme.Should().Be("Apikey");
        actual.Parameter.Should().Be(apiKey);
    }

    [Fact]
    public async Task PersonalAccessToken() {
        const string              pat     = "d38d9e31fc0e44db855c3bf1197e2e26dc46b7b1";
        GandiAuthenticationFilter filter  = new(() => pat);
        HttpRequestMessage        request = new(HttpMethod.Get, new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records"));
        await filter.Filter(request, new FilterContext(), CancellationToken.None);
        AuthenticationHeaderValue? actual = request.Headers.Authorization;
        actual.Should().NotBeNull();
        actual.Scheme.Should().Be("Bearer");
        actual.Parameter.Should().Be(pat);
    }

    [Fact]
    public async Task DomainLock() {
        const string              pat     = "d38d9e31fc0e44db855c3bf1197e2e26dc46b7b1";
        GandiAuthenticationFilter filter  = new(() => pat);
        HttpRequestMessage        request = new(HttpMethod.Get, new Uri("https://raw.githubusercontent.com/pradt2/always-online-stun/master/valid_hosts.txt"));
        await filter.Filter(request, new FilterContext(), CancellationToken.None);
        AuthenticationHeaderValue? actual = request.Headers.Authorization;
        actual.Should().BeNull("wrong URL");
    }

    [Fact]
    public async Task PrioritizeExistingAuth() {
        const string              pat    = "8f9b164315f9370708294f310e5c0ce5172c7e61";
        GandiAuthenticationFilter filter = new(() => "d38d9e31fc0e44db855c3bf1197e2e26dc46b7b1");
        HttpRequestMessage request = new(HttpMethod.Get, new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records"))
            { Headers = { Authorization = new AuthenticationHeaderValue("Bearer", pat) } };
        await filter.Filter(request, new FilterContext(), CancellationToken.None);
        AuthenticationHeaderValue? actual = request.Headers.Authorization;
        actual.Should().NotBeNull();
        actual.Scheme.Should().Be("Bearer");
        actual.Parameter.Should().Be(pat);
    }

    [Fact]
    public async Task IgnoreNullTokens() {
        GandiAuthenticationFilter filter  = new(() => null);
        HttpRequestMessage        request = new(HttpMethod.Get, new Uri("https://api.gandi.net/v5/livedns/domains/aldaviva.com/records"));
        await filter.Filter(request, new FilterContext(), CancellationToken.None);
        AuthenticationHeaderValue? actual = request.Headers.Authorization;
        actual.Should().BeNull("no token provided");
    }

}