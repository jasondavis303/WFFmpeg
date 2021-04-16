using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WFFmpeg.FFmpeg.Encoding
{
    public class AudioStream
    {
        /// <summary>
        /// Input stream index. If < 0, ffmpeg chooses the 'best' video stream
        /// </summary>
        public int Index { get; set; } = -1;

        [JsonConverter(typeof(StringEnumConverter))]
        public EncodingMethods Method { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AudioCodecs Codec { get; set; }

        /// <summary>
        /// Only used when Method == EncodingMethods.Quality
        /// </summary>
        public double Quality { get; set; } = -1;

        /// <summary>
        /// Only used when Method == EncodingMethods.AvgBitrate
        /// </summary>
        public int KiloBitrate { get; set; } = -1;

        /// <summary>
        /// Ignored if not set. Most common is s16
        /// </summary>
        public string SampleFormat { get; set; }

        /// <summary>
        /// Ignored if not set. Most common are 44100 and 48000
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// Ignored if not set.  Use 6 for DD 5.1, use 8 for 7.1
        /// </summary>
        public int Channels { get; set; }

        /// <summary>
        /// Set to true when downmixing from 5.1 or 7.1 channels
        /// </summary>
        public bool DolbyProLogic2 { get; set; }

        /// <summary>
        /// eng, und for my uses
        /// </summary>
        public string Language { get; set; }

        public string Title { get; set; }

        public bool Commentary { get; set; }

        public bool Default { get; set; }
    }
}
