using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WFFmpeg.FFprobe.json
{
    class FFParseTimeSpanStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(bool) || t == typeof(bool?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            string value = serializer.Deserialize<string>(reader);

            string[] parts = value.Split(':');
            if (parts[parts.Length - 1].Contains("."))
            {
                string[] parts2 = parts[parts.Length - 1].Split('.');
                if (parts2[1].Length > 3)
                    parts2[1] = parts2[1].Substring(0, 3);
                parts[parts.Length - 1] = string.Join(".", parts2);
            }
            value = string.Join(":", parts);

            if (TimeSpan.TryParse(value, out TimeSpan ts))
                return ts;
            throw new Exception("Cannot unmarshal type timespan");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            serializer.Serialize(writer, (int)untypedValue);
            return;
        }

        public static readonly FFParseTimeSpanStringConverter Singleton = new FFParseTimeSpanStringConverter();
    }
}
