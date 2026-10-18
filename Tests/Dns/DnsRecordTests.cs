namespace Tests.Dns;

public class DnsRecordTests {

    [Fact]
    public void Limits() {
        DnsRecord.MinTimeToLive.Should().Be(TimeSpan.FromSeconds(300));
        DnsRecord.MaxTimeToLive.Should().Be(TimeSpan.FromSeconds(2592000));
    }

    [Fact]
    public void Equality() {
        DnsRecord record    = new(RecordType.A, "www", DnsRecord.MinTimeToLive, "204.11.166.180");
        DnsRecord same      = new(RecordType.A, "www", DnsRecord.MinTimeToLive, "204.11.166.180");
        DnsRecord different = new(RecordType.A, "@", DnsRecord.MinTimeToLive, "204.11.166.180");

        record.Should().Be(same);
        (record == same).Should().BeTrue();
        (record != same).Should().BeFalse();
        record.GetHashCode().Should().Be(same.GetHashCode());

        record.Should().NotBe(different);
        (record == different).Should().BeFalse();
        (record != different).Should().BeTrue();
        record.GetHashCode().Should().NotBe(different.GetHashCode());
    }

    [Fact]
    public void ToStringTest() {
        string actual = new DnsRecord(RecordType.A, "www", DnsRecord.MinTimeToLive, "204.11.166.180").ToString();
        actual.Should().Be("Type: A, Name: www, TimeToLive: 00:05:00, Values: 204.11.166.180");
    }

}