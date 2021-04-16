using Newtonsoft.Json;
using System;

namespace WFFmpeg.FFprobe.json
{
    class FFStreamTags
    {
        [JsonProperty("DURATION", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseTimeSpanStringConverter))] 
        public TimeSpan Duration { get; set; }

        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }

        [JsonProperty("handler_name", NullValueHandling = NullValueHandling.Ignore)]
        public string HandlerName { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        //[JsonProperty("creation_time", NullValueHandling = NullValueHandling.Ignore)]
        //public DateTime CreationTime { get; set; }


        [JsonProperty("BPS-eng", NullValueHandling = NullValueHandling.Ignore)]
        public int BPSEng { get; set; }


        [JsonProperty("DURATION-eng", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseTimeSpanStringConverter))]
        public TimeSpan DurationEng { get; set; }


        [JsonProperty("NUMBER_OF_FRAMES-eng", NullValueHandling = NullValueHandling.Ignore)]
        public int NumberOfFramesEng { get; set; }


        [JsonProperty("NUMBER_OF_BYTES-eng", NullValueHandling = NullValueHandling.Ignore)]
        public long NumberOfBytesEng { get; set; }


    }

}
