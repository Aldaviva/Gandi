using Gandi.Marshal;
using System.Text.Json;

namespace Tests.Marshal;

public class SecondsToTimeSpanConverterTest {

    private readonly SecondsToTimeSpanConverter _converter = new();

    [Fact]
    public void Deserialize() {
        Utf8JsonReader reader = new("10800"u8);
        reader.Read();
        TimeSpan actual = _converter.Read(ref reader, typeof(TimeSpan), JsonSerializerOptions.Default);
        actual.Should().Be(TimeSpan.FromHours(3));
    }

    [Fact]
    public async Task Serialize() {
        using MemoryStream         jsonStream = new();
        await using Utf8JsonWriter writer     = new(jsonStream);
        _converter.Write(writer, TimeSpan.FromMinutes(5), JsonSerializerOptions.Default);
        await writer.FlushAsync();
        jsonStream.ToArray().Should().Equal("300"u8.ToArray());
    }

}