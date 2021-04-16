using Newtonsoft.Json;

namespace WFFmpeg.FFprobe.json
{
    class FFFormat
    {
        [JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
        public string Filename { get; set; }

        [JsonProperty("nb_streams", NullValueHandling = NullValueHandling.Ignore)]
        public int NbStreams { get; set; }

        [JsonProperty("NB_programs", NullValueHandling = NullValueHandling.Ignore)]
        public int NBPrograms { get; set; }

        [JsonProperty("format_name", NullValueHandling = NullValueHandling.Ignore)]
        public string FormatName { get; set; }

        [JsonProperty("format_long_name", NullValueHandling = NullValueHandling.Ignore)]
        public string FormatLongName { get; set; }

        [JsonProperty("start_time", NullValueHandling = NullValueHandling.Ignore)]
        public double StartTime { get; set; }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public double Duration { get; set; }

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long Size { get; set; }

        [JsonProperty("bit_rate", NullValueHandling = NullValueHandling.Ignore)]
        public int BitRate { get; set; }

        [JsonProperty("probe_score", NullValueHandling = NullValueHandling.Ignore)]
        public int ProbeScore { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public FFFormatTags Tags { get; set; }
    }

}
