using System.Text.Json;
using System.Text.Json.Serialization;

namespace FitappAdminWeb.Net7.Classes.Converters
{
    public class TimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            TimeSpan parseTS = TimeSpan.MinValue;
            TimeSpan.TryParse(value, out parseTS);

            return parseTS;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            //writer.WriteStartObject();           
            writer.WriteStringValue(value.ToString());
        }
    }

    public class NullableTimeSpanJsonConverter : JsonConverter<TimeSpan?>
    {
        public override bool HandleNull => true;

        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            TimeSpan parseTS = TimeSpan.MinValue;
            if (TimeSpan.TryParse(value, out parseTS))
            {
                return parseTS;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {           
            if (value.HasValue)
            {
                writer.WriteStartObject();
                writer.WriteStringValue(value.Value.ToString());
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteNullValue();
            }
        }
    }
}
