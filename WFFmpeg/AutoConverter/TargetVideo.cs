using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace WFFmpeg.AutoConverter
{
    public class TargetVideo : IEquatable<TargetVideo>, ICloneable
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public FFmpeg.Encoding.VideoCodecs Codec { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public VideoResolutions TargetResolution { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public VideoResolutions MaxResolution { get; set; }

        public bool Animation { get; set; }

        public int MaxKiloBitRate { get; set; }

        public double NoiseFactor { get; set; }

        public double QualityFactor { get; set; }

        public double TargetFrameRate { get; set; }

        public double MaxFrameRate { get; set; }

        public string MaxProfile { get; set; }

        public double MaxLevel { get; set; }

        public bool x265HighTier { get; set; }

        public string PixelFormat { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TargetVideo);
        }

        public bool Equals(TargetVideo other)
        {
            return other != null &&
                   Codec == other.Codec &&
                   TargetResolution == other.TargetResolution &&
                   MaxResolution == other.MaxResolution &&
                   Animation == other.Animation &&
                   MaxKiloBitRate == other.MaxKiloBitRate &&
                   NoiseFactor == other.NoiseFactor &&
                   QualityFactor == other.QualityFactor &&
                   TargetFrameRate == other.TargetFrameRate &&
                   MaxFrameRate == other.MaxFrameRate &&
                   MaxProfile == other.MaxProfile &&
                   MaxLevel == other.MaxLevel &&
                   x265HighTier == other.x265HighTier &&
                   PixelFormat == other.PixelFormat;
        }

        public override int GetHashCode()
        {
            int hashCode = -1080518637;
            hashCode = hashCode * -1521134295 + Codec.GetHashCode();
            hashCode = hashCode * -1521134295 + TargetResolution.GetHashCode();
            hashCode = hashCode * -1521134295 + MaxResolution.GetHashCode();
            hashCode = hashCode * -1521134295 + Animation.GetHashCode();
            hashCode = hashCode * -1521134295 + MaxKiloBitRate.GetHashCode();
            hashCode = hashCode * -1521134295 + NoiseFactor.GetHashCode();
            hashCode = hashCode * -1521134295 + QualityFactor.GetHashCode();
            hashCode = hashCode * -1521134295 + TargetFrameRate.GetHashCode();
            hashCode = hashCode * -1521134295 + MaxFrameRate.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MaxProfile);
            hashCode = hashCode * -1521134295 + MaxLevel.GetHashCode();
            hashCode = hashCode * -1521134295 + x265HighTier.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PixelFormat);

            return hashCode;
        }

        public object Clone() => new TargetVideo
        {
            Animation = Animation,
            Codec = Codec,
            MaxFrameRate = MaxFrameRate,
            MaxKiloBitRate = MaxKiloBitRate,
            MaxLevel = MaxLevel,
            MaxProfile = MaxProfile,
            MaxResolution = MaxResolution,
            NoiseFactor = NoiseFactor,
            QualityFactor = QualityFactor,
            TargetFrameRate = TargetFrameRate,
            TargetResolution = TargetResolution,
            x265HighTier = x265HighTier,
            PixelFormat = PixelFormat
        };

        public static bool operator ==(TargetVideo left, TargetVideo right)
        {
            return EqualityComparer<TargetVideo>.Default.Equals(left, right);
        }

        public static bool operator !=(TargetVideo left, TargetVideo right)
        {
            return !(left == right);
        }





        public static TargetVideo SD_Film => new TargetVideo
        {
            Codec = FFmpeg.Encoding.VideoCodecs.x264,
            MaxFrameRate = 30.0d,
            MaxKiloBitRate = 2500,
            MaxResolution = VideoResolutions.SD,
            NoiseFactor = 1.5d,
            TargetFrameRate = 24.0d,
            TargetResolution = VideoResolutions.SD,
            MaxProfile = "High",
            MaxLevel = 4.1d,
            QualityFactor = 8000,
            PixelFormat = "yuv420p"
        };

        public static TargetVideo SD_Animation => new TargetVideo
        {
            Animation = true,
            Codec = FFmpeg.Encoding.VideoCodecs.x264,
            MaxFrameRate = 30.0d,
            MaxKiloBitRate = 1500,
            MaxResolution = VideoResolutions.SD,
            NoiseFactor = 1.0d,
            TargetFrameRate = 24.0d,
            TargetResolution = VideoResolutions.SD,
            MaxProfile = "High",
            MaxLevel = 4.1d,
            QualityFactor = 8000,
            PixelFormat = "yuv420p"
        };

        public static TargetVideo HD_Film => new TargetVideo
        {
            Codec = FFmpeg.Encoding.VideoCodecs.x264,
            MaxFrameRate = 30.0d,
            MaxKiloBitRate = 5000,
            MaxResolution = VideoResolutions.FHD,
            NoiseFactor = 1.5d,
            TargetFrameRate = 24.0d,
            TargetResolution = VideoResolutions.HD,
            MaxProfile = "High",
            MaxLevel = 4.1d,
            QualityFactor = 8000,
            PixelFormat = "yuv420p"
        };

        public static TargetVideo HD_Animation => new TargetVideo
        {
            Animation = true,
            MaxFrameRate = 30.0d,
            Codec = FFmpeg.Encoding.VideoCodecs.x264,
            MaxKiloBitRate = 2500,
            MaxResolution = VideoResolutions.FHD,
            NoiseFactor = 1.0d,
            TargetFrameRate = 24.0d,
            TargetResolution = VideoResolutions.HD,
            MaxProfile = "High",
            MaxLevel = 4.1d,
            QualityFactor = 8000,
            PixelFormat = "yuv420p"
        };

        public static TargetVideo FHD_Film => new TargetVideo
        {
            Codec = FFmpeg.Encoding.VideoCodecs.x264,
            MaxFrameRate = 30.0d,
            MaxKiloBitRate = 10000,
            MaxResolution = VideoResolutions.FHD,
            NoiseFactor = 1.5d,
            TargetFrameRate = 24.0d,
            TargetResolution = VideoResolutions.FHD,
            MaxProfile = "High",
            MaxLevel = 4.1d,
            QualityFactor = 8000,
            PixelFormat = "yuv420p"
        };

        public static TargetVideo FHD_Animation => new TargetVideo
        {
            Animation = true,
            Codec = FFmpeg.Encoding.VideoCodecs.x264,
            MaxFrameRate = 30.0d,
            MaxKiloBitRate = 3500,
            MaxResolution = VideoResolutions.FHD,
            NoiseFactor = 1.0d,
            TargetFrameRate = 24.0d,
            TargetResolution = VideoResolutions.FHD,
            MaxProfile = "High",
            MaxLevel = 4.1d,
            QualityFactor = 8000,
            PixelFormat = "yuv420p"
        };


        public static TargetVideo UHD_Film => new TargetVideo
        {
            Codec = FFmpeg.Encoding.VideoCodecs.x265,
            MaxFrameRate = 60.0d,
            MaxKiloBitRate = 20000,
            MaxResolution = VideoResolutions.UHD,
            NoiseFactor = 1.5d,
            TargetFrameRate = 30.0d,
            TargetResolution = VideoResolutions.UHD,
            MaxProfile = "Main",
            MaxLevel = 3.1d,
            QualityFactor = 8000,
            PixelFormat = "yuv420p"
        };

        public static TargetVideo UHD_Animation => new TargetVideo
        {
            Animation = true,
            Codec = FFmpeg.Encoding.VideoCodecs.x265,
            MaxFrameRate = 60.0d,
            MaxKiloBitRate = 10000,
            MaxResolution = VideoResolutions.UHD,
            NoiseFactor = 1.0d,
            TargetFrameRate = 30.0d,
            TargetResolution = VideoResolutions.UHD,
            MaxProfile = "Main",
            MaxLevel = 3.1d,
            QualityFactor = 8000,
            PixelFormat = "yuv420p"
        };

        
    }
}
