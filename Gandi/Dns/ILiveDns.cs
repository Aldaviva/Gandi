namespace Gandi.Dns;

/// <summary>
/// Client for Gandi LiveDNS Management API. Used to create, read, update, and delete DNS records for a given domain whose DNS is hosted by Gandi.
/// </summary>
public interface ILiveDns {

    /// <summary>
    /// Find all DNS records in the domain, with optional filtering on the name and type of the records.
    /// </summary>
    /// <param name="type">Only return records of this type. If omitted, returns records of all types. Never returns any <c>SOA</c>, <c>DS</c>, or <c>DNSKEY</c> records, or the top-level
    /// <see cref="RecordType.NS"/> record.</param>
    /// <param name="name">Only return records with this name. To specify the top/root/origin/apex level of your domain, pass <c>@</c> (<see cref="DnsRecord.Origin"/>).</param>
    /// <param name="cancellationToken">To cancel the async operation before it finishes.</param>
    /// <returns>A sequence of all matching DNS records in the domain, sorted ascending by name. If no records matched, returns the empty sequence.</returns>
    /// <exception cref="GandiException">The auth token doesn't exist, has expired, or doesn't have permissions to access this domain; or an I/O exception occurred during the request, such as a DNS or TCP error.</exception>
    Task<IEnumerable<DnsRecord>> List(RecordType? type = null, string? name = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetch a single DNS record with the given type and name in the domain.
    /// </summary>
    /// <param name="type">The type of DNS record to return.</param>
    /// <param name="name">The name of the DNS record to return. To specify the top/root/origin/apex level of your domain, pass <c>@</c> (<see cref="DnsRecord.Origin"/>).</param>
    /// <param name="cancellationToken">To cancel the async operation before it finishes.</param>
    /// <returns>The DNS record from the domain that matches the given <paramref name="type"/> and <paramref name="name"/>; or <c>null</c> if no matching record was found, or the top-level
    /// <see cref="RecordType.NS"/> record was requested.</returns>
    /// <exception cref="GandiException">The auth token doesn't exist, has expired, or doesn't have permissions to access this domain; or an I/O exception occurred during the request, such as a DNS or TCP error.</exception>
    Task<DnsRecord?> Get(RecordType type, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetch a single DNS record with the given type and name in the domain.
    /// </summary>
    /// <param name="query">An input DNS record containing the <see cref="DnsRecord.Type"/> and <see cref="DnsRecord.Name"/> to find. Other properties (such as <see cref="DnsRecord.TimeToLive"/> and
    /// <see cref="DnsRecord.Values"/>) are ignored.</param>
    /// <param name="cancellationToken">To cancel the async operation before it finishes.</param>
    /// <returns>The DNS record from the domain that matches the type and name of the given <paramref name="query"/>; or <c>null</c> if no matching record was found, or the top-level
    /// <see cref="RecordType.NS"/> record was requested.</returns>
    /// <exception cref="GandiException">The auth token doesn't exist, has expired, or doesn't have permissions to access this domain; or an I/O exception occurred during the request, such as a DNS or TCP error.</exception>
    Task<DnsRecord?> Get(DnsRecord query, CancellationToken cancellationToken = default);

    /// <summary>
    /// <para>Create or update a DNS record in the domain.</para>
    /// <para>If another record with the same type and name already exists, it will be updated with the data from <paramref name="record"/>, otherwise, a new record will be created.</para>
    /// </summary>
    /// <param name="record">The new DNS record to add or modify. It must have one or more <see cref="DnsRecord.Values"/>. When set to <c>null</c>, <see cref="DnsRecord.TimeToLive"/> will default to 3
    /// hours.</param>
    /// <param name="cancellationToken">To cancel the async operation before it finishes.</param>
    /// <exception cref="ArgumentOutOfRangeException">No <see cref="DnsRecord.Values"/> were supplied in <paramref name="record"/>. At least one value is required.</exception>
    /// <exception cref="GandiException">The auth token doesn't exist, has expired, or doesn't have permissions to access this domain; or an I/O exception occurred during the request, such as a DNS or TCP error.</exception>
    Task Set(DnsRecord record, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete DNS records with the given name and optional type from the domain.
    /// </summary>
    /// <param name="type">Optional type requirement for the record to delete. If not <c>null</c>, a record will be deleted only if it matches this <paramref name="type"/> and <paramref name="name"/>, otherwise, all records of all types with the given <paramref name="name"/> will be deleted.</param>
    /// <param name="name">Name of the records to delete. To specify the top/root/origin/apex level of your domain, pass <c>@</c> (<see cref="DnsRecord.Origin"/>).</param>
    /// <param name="cancellationToken">To cancel the async operation before it finishes.</param>
    /// <returns>A task that will resolve successfully whether the records existed and were deleted, or if they didn't exist and there was nothing to delete, because in either case, the records do not
    /// exist after the response was received.</returns>
    /// <exception cref="GandiException">The auth token doesn't exist, has expired, or doesn't have permissions to access this domain; or an I/O exception occurred during the request, such as a DNS or TCP error.</exception>
    Task Delete(RecordType? type, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a DNS record with the given name and optional type from the domain.
    /// </summary>
    /// <param name="record">An input DNS record containing the <see cref="DnsRecord.Type"/> and <see cref="DnsRecord.Name"/> to delete. Other properties (such as <see cref="DnsRecord.TimeToLive"/> and
    /// <see cref="DnsRecord.Values"/>) are ignored.</param>
    /// <param name="cancellationToken">To cancel the async operation before it finishes.</param>
    /// <returns>A task that will resolve successfully whether the record existed and was deleted, or if it didn't exist and there was nothing to delete, because in either case, the record does not
    /// exist after the response was received.</returns>
    /// <exception cref="GandiException">The auth token doesn't exist, has expired, or doesn't have permissions to access this domain; or an I/O exception occurred during the request, such as a DNS or TCP error.</exception>
    Task Delete(DnsRecord record, CancellationToken cancellationToken = default);

}