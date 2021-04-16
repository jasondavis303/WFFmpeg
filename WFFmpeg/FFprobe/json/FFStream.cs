using Newtonsoft.Json;

namespace WFFmpeg.FFprobe.json
{
    class FFStream
    {
        [JsonProperty("index", NullValueHandling = NullValueHandling.Ignore)]
        public int Index { get; set; }

        [JsonProperty("codec_name", NullValueHandling = NullValueHandling.Ignore)]
        public string CodecName { get; set; }

        [JsonProperty("codec_long_name", NullValueHandling = NullValueHandling.Ignore)]
        public string CodecLongName { get; set; }

        [JsonProperty("profile", NullValueHandling = NullValueHandling.Ignore)]
        public string Profile { get; set; }

        [JsonProperty("codec_type", NullValueHandling = NullValueHandling.Ignore)]
        public string CodecType { get; set; }

        [JsonProperty("codec_time_base", NullValueHandling = NullValueHandling.Ignore)]
        public string CodecTimeBase { get; set; }

        [JsonProperty("codec_tag_string", NullValueHandling = NullValueHandling.Ignore)]
        public string CodecTagString { get; set; }

        [JsonProperty("codec_tag", NullValueHandling = NullValueHandling.Ignore)]
        public string CodecTag { get; set; }

        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public int Width { get; set; }

        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public int Height { get; set; }

        [JsonProperty("coded_width", NullValueHandling = NullValueHandling.Ignore)]
        public int CodedWidth { get; set; }

        [JsonProperty("coded_height", NullValueHandling = NullValueHandling.Ignore)]
        public int CodedHeight { get; set; }

        [JsonProperty("has_b_frames", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool HasBFrames { get; set; }

        [JsonProperty("sample_aspect_ratio", NullValueHandling = NullValueHandling.Ignore)]
        public string SampleAspectRatio { get; set; }

        [JsonProperty("display_aspect_ratio", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayAspectRatio { get; set; }

        [JsonProperty("pix_fmt", NullValueHandling = NullValueHandling.Ignore)]
        public string PixFmt { get; set; }

        [JsonProperty("level", NullValueHandling = NullValueHandling.Ignore)]
        public double Level { get; set; }

        [JsonProperty("color_range", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorRange { get; set; }

        [JsonProperty("color_space", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorSpace { get; set; }

        [JsonProperty("color_transfer", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorTransfer { get; set; }

        [JsonProperty("color_primaries", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorPrimaries { get; set; }

        [JsonProperty("chroma_location", NullValueHandling = NullValueHandling.Ignore)]
        public string ChromaLocation { get; set; }

        [JsonProperty("field_order", NullValueHandling = NullValueHandling.Ignore)]
        public string FieldOrder { get; set; }

        [JsonProperty("refs", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public bool Refs { get; set; }

        [JsonProperty("is_avc", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FFParseBoolStringConverter))]
        public bool IsAvc { get; set; }

        [JsonProperty("nal_length_size", NullValueHandling = NullValueHandling.Ignore)]
        public int NalLengthSize { get; set; }

        [JsonProperty("r_frame_rate", NullValueHandling = NullValueHandling.Ignore)]
        public string RFrameRate { get; set; }

        [JsonProperty("avg_frame_rate", NullValueHandling = NullValueHandling.Ignore)]
        public string AvgFrameRate { get; set; }

        [JsonProperty("time_base", NullValueHandling = NullValueHandling.Ignore)]
        public string TimeBase { get; set; }

        [JsonProperty("start_pts", NullValueHandling = NullValueHandling.Ignore)]
        public int StartPts { get; set; }

        [JsonProperty("start_time", NullValueHandling = NullValueHandling.Ignore)]
        public double StartTime { get; set; }

        [JsonProperty("bits_per_raw_sample", NullValueHandling = NullValueHandling.Ignore)]
        public int BitsPerRawSample { get; set; }

        [JsonProperty("nb_frames", NullValueHandling = NullValueHandling.Ignore)]
        public int NBFrames { get; set; }

        [JsonProperty("disposition", NullValueHandling = NullValueHandling.Ignore)]
        public FFDisposition Disposition { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public FFStreamTags Tags { get; set; }

        [JsonProperty("sample_fmt", NullValueHandling = NullValueHandling.Ignore)]
        public string SampleFmt { get; set; }

        [JsonProperty("sample_rate", NullValueHandling = NullValueHandling.Ignore)]
        public int SampleRate { get; set; }

        [JsonProperty("channels", NullValueHandling = NullValueHandling.Ignore)]
        public int Channels { get; set; }

        [JsonProperty("channel_layout", NullValueHandling = NullValueHandling.Ignore)]
        public string ChannelLayout { get; set; }

        [JsonProperty("bits_per_sample", NullValueHandling = NullValueHandling.Ignore)]
        public int BitsPerSample { get; set; }

        [JsonProperty("duration_ts", NullValueHandling = NullValueHandling.Ignore)]
        public long DurationTS { get; set; }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public double? Duration { get; set; }

        [JsonProperty("bit_rate", NullValueHandling = NullValueHandling.Ignore)]
        public int BitRate { get; set; }

        [JsonProperty("max_bit_rate", NullValueHandling = NullValueHandling.Ignore)]
        public int MaxBitRate { get; set; }
    }

}
