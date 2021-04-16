using Newtonsoft.Json;

namespace WFFmpeg.FFprobe.json
{
    class FFChapterTags
    {
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
    }
}
