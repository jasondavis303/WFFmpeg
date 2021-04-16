using Newtonsoft.Json;
using System.Collections.Generic;

namespace WFFmpeg.FFprobe.json
{
    class FFResult
    {
        [JsonProperty("streams", NullValueHandling = NullValueHandling.Ignore)]
        public List<FFStream> Streams { get; set; }

        [JsonProperty("format", NullValueHandling = NullValueHandling.Ignore)]
        public FFFormat Format { get; set; }

        [JsonProperty("chapters", NullValueHandling = NullValueHandling.Ignore)]
        public List<FFChapter> Chapters { get; set; }

        public static FFResult FromJson(string json) => JsonConvert.DeserializeObject<FFResult>(json, FFConverter.Settings);
    }
}
