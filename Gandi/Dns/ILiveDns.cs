namespace Gandi.Dns;

public interface ILiveDns {

    Task<IEnumerable<DnsRecord>> List(RecordType? type = null, string? name = null, CancellationToken cancellationToken = default);

    Task<DnsRecord?> Get(RecordType type, string name, CancellationToken cancellationToken = default);

    Task Set(DnsRecord record, CancellationToken cancellationToken = default);

    Task Delete(RecordType? type, string name, CancellationToken cancellationToken = default);
    Task Delete(DnsRecord record, CancellationToken cancellationToken = default);

}