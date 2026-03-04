using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeBattleArena.Server.Untils
{
    public class UtcDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(
           ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.SpecifyKind(reader.GetDateTime(), DateTimeKind.Utc);
        }

        public override void Write(
            Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var utcValue = DateTime.SpecifyKind(value, DateTimeKind.Utc); // НЕ преобразуем!
            writer.WriteStringValue(utcValue.ToString("o")); // сериализуем как ISO8601 (добавит Z)
        }
    }

}
