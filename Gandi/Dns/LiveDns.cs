using System.Linq;
using System.Net.Http.Json;
using Unfucked;
using Unfucked.HTTP;
using Unfucked.HTTP.Exceptions;

namespace Gandi.Dns;

internal class LiveDns(IGandiClient gandi, string domain): ILiveDns {

    private readonly WebTarget _apiBase = gandi.HttpClient
        .Target(GandiClient.ApiBase)
        .Path("livedns/domains/{domain}/records")
        .ResolveTemplate("domain", domain)
        .Accept(GandiClient.ApplicationJsonType);

    /// <inheritdoc />
    public async Task<IEnumerable<DnsRecord>> List(RecordType? type = null, string? name = null, CancellationToken cancellationToken = default) {
        if (type != null && name != null) {
            return await Get(type.Value, name, cancellationToken).ConfigureAwait(false) is { } singleResult ? [singleResult] : [];
        } else {
            try {
                WebTarget target = _apiBase;
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
            } catch (HttpRequestException e) {
                throw new GandiException($"Failed to find records in domain {domain}", e);
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
        } catch (NotFoundException) {
            return null;
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
            response.EnsureSuccessStatusCode();
        } catch (HttpRequestException e) {
            throw new GandiException($"Failed to create or modify {record.Type} record {record.Name}", e);
        }
    }

    /// <inheritdoc />
    public Task Delete(DnsRecord record, CancellationToken cancellationToken = default) => Delete(record.Type, record.Name, cancellationToken);

    /// <inheritdoc />
    public async Task Delete(RecordType? type, string name, CancellationToken cancellationToken = default) {
        try {
            WebTarget target = _apiBase.Path("{name}");
            target = type != null ? target.Path("{type}") : target;
            using HttpResponseMessage response = await target
                .ResolveTemplate("name", name)
                .ResolveTemplate("type", type?.ToUriString())
                .Delete(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        } catch (HttpRequestException e) {
            throw new GandiException($"Failed to delete record {name}", e);
        }
    }

    private static DnsRecord Sanitize(DnsRecord input) => input with {
        Name = input.Name.EmptyToNull() ?? DnsRecord.Origin,
        TimeToLive = input.TimeToLive == TimeSpan.Zero ? null : input.TimeToLive?.Clip(DnsRecord.MinTimeToLive, DnsRecord.MaxTimeToLive)
    };

}