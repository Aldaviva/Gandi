using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Unfucked.HTTP;
using Unfucked.HTTP.Exceptions;

namespace Gandi.Dns;

internal class LiveDns(IGandiClient gandi, string domain): ILiveDns {

    private readonly IWebTarget _apiBase = gandi.HttpClient
        .Target(GandiClient.ApiBase)
        .Path("livedns/domains/{domain}/records")
        .ResolveTemplate("domain", domain)
        .Accept(GandiClient.ApplicationJsonType);

    /// <inheritdoc />
    public string Domain { get; } = domain;

    /// <inheritdoc />
    public async Task<IEnumerable<DnsRecord>> List(RecordType? type = null, string? name = null, CancellationToken cancellationToken = default) {
        if (type != null && name != null) {
            return await Get(type.Value, name, cancellationToken).ConfigureAwait(false) is {} singleResult ? [singleResult] : [];
        } else {
            try {
                IWebTarget target = _apiBase;
                if (name != null) {
                    target = target.Path("{name}");
                } else if (type != null) {
                    target = target.QueryParam("rrset_type", "{type}");
                }
                return await target
                    .ResolveTemplate("name", name)
                    .ResolveTemplate("type", type?.ToUriString())
                    .Get<IEnumerable<DnsRecord>>(cancellationToken)
                    .ConfigureAwait(false);
            } catch (ClientErrorException e) when (e is ForbiddenException or NotAuthorizedException) {
                throw new GandiException.AuthException("Gandi auth failure", e);
            } catch (HttpRequestException e) {
                throw new GandiException($"Failed to find records in domain {Domain}", e);
            }
        }
    }

    /// <inheritdoc />
    public Task<DnsRecord?> Get(DnsRecord query, CancellationToken cancellationToken = default) => Get(query.Type, query.Name, cancellationToken);

    /// <inheritdoc />
    public async Task<DnsRecord?> Get(RecordType type, string name, CancellationToken cancellationToken = default) {
        try {
            return await _apiBase
                .Path("{name}/{type}")
                .ResolveTemplate("name", name)
                .ResolveTemplate("type", type.ToUriString())
                .Get<DnsRecord>(cancellationToken)
                .ConfigureAwait(false);
        } catch (NotFoundException e) {
            try {
                if (e.ResponseBody is {} responseBody && JsonSerializer.Deserialize<JsonNode>(responseBody.Span)?["object"]?.GetValue<string>() == "HTTPNotFound") {
                    // wrong record type or name gives a "dns-record" value instead
                    throw new GandiException.AuthException($"Gandi auth token is for the wrong domain, not {Domain}", e);
                }
            } catch (JsonException) {}
            return null;
        } catch (ClientErrorException e) when (e is ForbiddenException or NotAuthorizedException) {
            throw new GandiException.AuthException("Gandi auth failure", e);
        } catch (HttpRequestException e) {
            throw new GandiException($"Failed to get {type} record {name}", e);
        }
    }

    /// <inheritdoc />
    public async Task Set(DnsRecord record, CancellationToken cancellationToken = default) {
        record = record.Values is ICollection<string> ? record : record with { Values = record.Values.ToList() }; // prevent multiple enumerations

        if (!record.Values.Any()) {
            throw new ArgumentOutOfRangeException(nameof(record), record,
                $"When creating or modifying a DNS record, it must have one or more values. To delete an existing record, call {nameof(Delete)} instead of {nameof(Set)}.");
        }

        try {
            using HttpResponseMessage response = await _apiBase
                .Path("{name}/{type}")
                .ResolveTemplate("name", record.Name)
                .ResolveTemplate("type", record.Type.ToUriString())
                .Put(JsonContent.Create(Sanitize(record), options: GandiClient.JsonOptions), cancellationToken)
                .ConfigureAwait(false);
            await response.ThrowIfUnsuccessful(cancellationToken).ConfigureAwait(false);
        } catch (NotFoundException e) {
            throw new GandiException.AuthException($"Not authorized to edit domain {Domain}, check that the Personal Access Token or API Key is for the right domain", e);
        } catch (ClientErrorException e) when (e is ForbiddenException or NotAuthorizedException) {
            throw new GandiException.AuthException("Gandi auth failure", e);
        } catch (HttpRequestException e) {
            throw new GandiException($"Failed to create or modify {record.Type} record {record.Name}", e);
        }
    }

    /// <inheritdoc />
    public Task Delete(DnsRecord record, CancellationToken cancellationToken = default) => Delete(record.Type, record.Name, cancellationToken);

    /// <inheritdoc />
    public async Task Delete(RecordType? type, string name, CancellationToken cancellationToken = default) {
        try {
            IWebTarget target = _apiBase.Path("{name}");
            target = type != null ? target.Path("{type}") : target;
            using HttpResponseMessage response = await target
                .ResolveTemplate("name", name)
                .ResolveTemplate("type", type?.ToUriString())
                .Delete(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            await response.ThrowIfUnsuccessful(cancellationToken).ConfigureAwait(false);
        } catch (NotFoundException e) {
            throw new GandiException.AuthException($"Not authorized to edit domain {Domain}, check that the Personal Access Token or API Key is for the right domain", e);
        } catch (ClientErrorException e) when (e is ForbiddenException or NotAuthorizedException) {
            throw new GandiException.AuthException("Gandi auth failure", e);
        } catch (HttpRequestException e) {
            throw new GandiException($"Failed to delete record {name}", e);
        }
    }

    private static DnsRecord Sanitize(DnsRecord input) => input with {
        Name = input.Name.EmptyToNull() ?? DnsRecord.Origin,
        TimeToLive = input.TimeToLive == TimeSpan.Zero ? null : input.TimeToLive?.Clip(DnsRecord.MinTimeToLive, DnsRecord.MaxTimeToLive)
    };

}