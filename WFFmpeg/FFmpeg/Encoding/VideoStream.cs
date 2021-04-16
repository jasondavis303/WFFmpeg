using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Drawing;

namespace WFFmpeg.FFmpeg.Encoding
{
    public class VideoStream
    {
        /// <summary>
        /// Input stream index. REQUIRED TO BE SET
        /// </summary>
        public int Index { get; set; } = -1;

        [JsonConverter(typeof(StringEnumConverter))] 
        public VideoCodecs Codec { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EncodingMethods Method { get; set; }

        /// <summary>
        /// Only used when Method == EncodingMethods.Quality
        /// </summary>
        public double CRF { get; set; } = -1;

        /// <summary>
        /// Only used when Method == EncodingMethods.AvgBitrate
        /// </summary>
        public int KiloBitrate { get; set; }

        public int MaxKiloBitrate { get; set; }

        public int MaxKiloBuffer { get; set; }

        /// <summary>
        /// Ignored if method = copy, If null, IsEmpty, or if Height or Width = 0
        /// </summary>
        public Rectangle Crop { get; set; }

        /// <summary>
        /// Ignored if method = copy, If null, or if Scale.IsEmpty.  Use a size paired with -1 to auto size, or use -2 to autosize for h.264/h.265 
        /// </summary>
        public Size Scale { get; set; }

        /// <summary>
        /// Ignored if method = copy. Default is Medium
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EncodeSpeed Speed { get; set; } = EncodeSpeed.S06_Medium;


        /// <summary>
        /// Ignored if method = copy. Can be set using x264Profiles.(value).ToString() or x265Profiles.(value).ToString(), based on the chosen encoder
        /// </summary>
        public string Profile { get; set; }

        /// <summary>
        /// Ignored if method = copy. 
        /// </summary>
        public double Level { get; set; }

        /// <summary>
        /// Only for x265 encoder. Ignored if method = copy. 
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public x265Tiers Tier { get; set; }

        /// <summary>
        /// Ignored if method = copy. Common values if set: 23.976, 24, 25, 29.97, 30
        /// </summary>
        public double FrameRate { get; set; }

        /// <summary>
        /// Ignored if method = copy
        /// </summary>
        public bool VariableFrameRate { get; set; }

        /// <summary>
        /// Ignored if method = copy. 
        /// </summary>
        public bool DeInterlace { get; set; }

        /// <summary>
        /// Ignored if method = copy. 
        /// </summary>
        public bool DeTelecine { get; set; }

        /// <summary>
        /// Ignored if None
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public VideoTunes Tune { get; set; }

        /// <summary>
        /// Ignored if method = copy.
        /// </summary>
        public string PixelFormat { get; set; }

        /// <summary>
        /// If it's known where the intro ends, force a key frame at this point
        /// </summary>
        public double ForceKeyFrameAt { get; set; } = -1;

    }
}
