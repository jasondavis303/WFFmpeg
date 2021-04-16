using Newtonsoft.Json;
using System;

namespace WFFmpeg.FFprobe.json
{
    class FFParseBoolNumStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(bool) || t == typeof(bool?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);

            if (int.TryParse(value, out int num))
                return num > 0;
            throw new Exception("Cannot unmarshal type bool");
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

        public static readonly FFParseBoolNumStringConverter Singleton = new FFParseBoolNumStringConverter();
    }
}
