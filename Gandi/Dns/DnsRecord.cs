using System.Linq;
using System.Text.Json.Serialization;
using Unfucked;

namespace Gandi.Dns;

public readonly struct DnsRecord {

    public const string Origin = "@";

    public static readonly TimeSpan MinTimeToLive = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan MaxTimeToLive = TimeSpan.FromDays(30);

    [JsonPropertyName("rrset_type")]
    public RecordType Type { get; init; }

    [JsonPropertyName("rrset_name")]
    public string Name { get; init; }

    // when null, this defaults to 3 hours
    [JsonPropertyName("rrset_ttl")]
    public TimeSpan? TimeToLive { get; init; }

    [JsonPropertyName("rrset_values")]
    public IEnumerable<string> Values { get; init; }

    public DnsRecord(RecordType type, string name, TimeSpan? timeToLive = null, params IEnumerable<string> values) {
        Type       = type;
        Name       = name;
        TimeToLive = timeToLive;
        Values     = values;
    }

    public bool Equals(DnsRecord other) => Type == other.Type && Name == other.Name && Nullable.Equals(TimeToLive, other.TimeToLive) && Values.SequenceEqual(other.Values);

    public override bool Equals(object? obj) => obj is DnsRecord other && Equals(other);

    public override int GetHashCode() {
        unchecked {
            int hashCode = (int) Type;
            hashCode = (hashCode * 397) ^ Name.GetHashCode();
            hashCode = (hashCode * 397) ^ TimeToLive.GetHashCode();
            return Values.Aggregate(hashCode, (current, value) => (current * 397) ^ value.GetHashCode());
        }
    }

    public override string ToString() => $"{nameof(Type)}: {Type}, {nameof(Name)}: {Name}, {nameof(TimeToLive)}: {TimeToLive?.ToString() ?? "null"}, {nameof(Values)}: {Values.Join(' ')}";

}