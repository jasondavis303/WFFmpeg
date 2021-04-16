using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace WFFmpeg.FFprobe.json
{
    static class FFConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeLocal }
            },
        };
    }

}
