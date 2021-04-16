using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WFFmpeg.FFprobe.json
{
    class FFChapter
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("time_base", NullValueHandling = NullValueHandling.Ignore)]
        public string TimeBase { get; set; }

        [JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
        public long? Start { get; set; }

        [JsonProperty("start_time", NullValueHandling = NullValueHandling.Ignore)]
        public double? StartTime { get; set; }

        [JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
        public long? End { get; set; }

        [JsonProperty("end_time", NullValueHandling = NullValueHandling.Ignore)]
        public double? EndTime { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public FFChapterTags Tags { get; set; }
    }
}
