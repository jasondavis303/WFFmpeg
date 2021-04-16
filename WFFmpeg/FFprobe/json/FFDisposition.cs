using Newtonsoft.Json;

namespace WFFmpeg.FFprobe.json
{
    class FFDisposition
    {
        [JsonProperty("default", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool Default { get; set; }

        [JsonProperty("dub", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool Dub { get; set; }

        [JsonProperty("original", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool Original { get; set; }

        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool Comment { get; set; }

        [JsonProperty("lyrics", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool Lyrics { get; set; }

        [JsonProperty("karaoke", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool Karaoke { get; set; }

        [JsonProperty("forced", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool Forced { get; set; }

        [JsonProperty("hearing_impaired", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool HearingImpaired { get; set; }

        [JsonProperty("visual_impaired", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool VisualImpaired { get; set; }

        [JsonProperty("clean_effects", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool CleanEffects { get; set; }

        [JsonProperty("attached_pic", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool AttachedPic { get; set; }

        [JsonProperty("timed_thumbnails", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool TimedThumbnails { get; set; }
    }

}
