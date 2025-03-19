using System.Linq;
using System.Text.Json.Serialization;
using Unfucked;

namespace Gandi.Dns;

/// <summary>
/// A DNS record stored in Gandi's LiveDNS servers, which would be returned as the result of a forward DNS query.
/// </summary>
public readonly struct DnsRecord {

    /// <summary>
    /// <para>A special record name that represents the root of the domain.</para>
    /// <para>For example, if your domain is <c>aldaviva.com</c>, you could set the IP address for lookups of <c>aldaviva.com</c> itself by creating an <see cref="RecordType.A"/> record with the name
    /// <see cref="Origin"/> (<c>@</c>).</para>
    /// </summary>
    public const string Origin = "@";

    /// <summary>
    /// The shortest period that a DNS record may be cached by Gandi, 5 minutes.
    /// </summary>
    public static readonly TimeSpan MinTimeToLive = TimeSpan.FromMinutes(5);

    /// <summary>
    /// The longest period that a DNS record may be cached by Gandi, 30 days.
    /// </summary>
    public static readonly TimeSpan MaxTimeToLive = TimeSpan.FromDays(30);

    /// <summary>
    /// The type of the DNS record, such as <see cref="RecordType.A"/>, <see cref="RecordType.CNAME"/>, or <see cref="RecordType.TXT"/>.
    /// </summary>
    [JsonPropertyName("rrset_type")]
    public RecordType Type { get; init; }

    /// <summary>
    /// The name of the DNS record, which is either the subdomain (without the domain name and trailing period) such as <c>www</c> or <c>api.stage</c>, or is <c>@</c> (<see cref="Origin"/>) for the
    /// top/root/origin/apex level of your domain.
    /// </summary>
    [JsonPropertyName("rrset_name")]
    public string Name { get; init; }

    // when null, this defaults to 3 hours
    /// <summary>
    /// <para>How long the DNS record may be cached after a lookup by a resolver before resolutions will require a fresh lookup.</para>
    /// <para>The shortest period allowed by Gandi is 5 minutes (<see cref="MinTimeToLive"/>), and the longest allowed is 30 days (<see cref="MaxTimeToLive"/>). Periods outside this range will be
    /// clipped.</para>
    /// <para>If this is <c>null</c>, Gandi will default it to 3 hours when you save the record.</para>
    /// </summary>
    [JsonPropertyName("rrset_ttl")]
    public TimeSpan? TimeToLive { get; init; }

    /// <summary>
    /// <para>Sequence of one or more values for the DNS record.</para>
    /// <para>Common formats are dotted-quad IP addresses for <see cref="RecordType.A"/> records, FQDNs ending in a period for <see cref="RecordType.CNAME"/> records, and strings with surrounding double quotation marks (optional when saving) for <see cref="RecordType.TXT"/> records.</para>
    /// </summary>
    [JsonPropertyName("rrset_values")]
    public IEnumerable<string> Values { get; init; }

    /// <summary>
    /// Construct a DNS record.
    /// </summary>
    /// <param name="type">The type of the record. It is recommended to not set top-level <see cref="RecordType.NS"/> records, or <see cref="RecordType.ALIAS"/> if DNSSEC is enabled.</param>
    /// <param name="name">The name of the record, without the trailing period and second-level domain name, such as <c>www</c> or <c>api.stage</c>. For the top/root/origin/apex level of your domain,
    /// pass <c>@</c> (<see cref="DnsRecord.Origin"/>).</param>
    /// <param name="timeToLive">Optional caching period of the record in resolvers. Will be clipped to stay in the valid range [5 minutes, 30 days], which are available as <see cref="MinTimeToLive"/> and <see cref="MaxTimeToLive"/>. When <c>null</c> is passed, it defaults to 3 hours.</param>
    /// <param name="values">One or more values for the DNS record.</param>
    public DnsRecord(RecordType type, string name, TimeSpan? timeToLive = null, params IEnumerable<string> values) {
        Type       = type;
        Name       = name;
        TimeToLive = timeToLive;
        Values     = values;
    }

    /// <inheritdoc cref="Equals(object?)" />
    public bool Equals(DnsRecord other) => Type == other.Type && Name == other.Name && Nullable.Equals(TimeToLive, other.TimeToLive) && Values.SequenceEqual(other.Values);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is DnsRecord other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() {
        unchecked {
            int hashCode = (int) Type;
            hashCode = (hashCode * 397) ^ Name.GetHashCode();
            hashCode = (hashCode * 397) ^ TimeToLive.GetHashCode();
            return Values.Aggregate(hashCode, (current, value) => (current * 397) ^ value.GetHashCode());
        }
    }

    /// <inheritdoc cref="Equals(object?)" />
    public static bool operator ==(DnsRecord left, DnsRecord right) => left.Equals(right);

    /// <summary>Indicates whether two objects are unequal.</summary>
    /// <param name="left">One of the objects to compare.</param>
    /// <param name="right">The other object to compare.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> and <paramref name="right"/> are the same type and represent the same value; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(DnsRecord left, DnsRecord right) => !left.Equals(right);

    /// <inheritdoc />
    public override string ToString() => $"{nameof(Type)}: {Type}, {nameof(Name)}: {Name}, {nameof(TimeToLive)}: {TimeToLive?.ToString() ?? "null"}, {nameof(Values)}: {Values.Join(' ')}";

}