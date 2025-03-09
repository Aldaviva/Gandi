using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gandi.Marshal;

internal class SecondsToTimeSpanConverter: JsonConverter<TimeSpan> {

    // ExceptionAdjustment: M:System.Text.Json.Utf8JsonReader.GetUInt64 -T:System.FormatException
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => TimeSpan.FromSeconds(reader.GetUInt64());

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options) => writer.WriteNumberValue((ulong) value.TotalSeconds);

}