using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace WFFmpeg.AutoConverter
{
    public class TargetAudio : IEquatable<TargetAudio>, ICloneable
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public FFmpeg.Encoding.AudioCodecs Codec { get; set; }

        public int Channels { get; set; }

        public int MaxKiloBitRate { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TargetAudio);
        }

        public bool Equals(TargetAudio other)
        {
            return other != null &&
                   Codec == other.Codec &&
                   Channels == other.Channels &&
                   MaxKiloBitRate == other.MaxKiloBitRate;
        }

        public override int GetHashCode()
        {
            int hashCode = -1401553241;
            hashCode = hashCode * -1521134295 + Codec.GetHashCode();
            hashCode = hashCode * -1521134295 + Channels.GetHashCode();
            hashCode = hashCode * -1521134295 + MaxKiloBitRate.GetHashCode();
            return hashCode;
        }

        public object Clone() => new TargetAudio
        {
            Channels = Channels,
            Codec = Codec,
            MaxKiloBitRate = MaxKiloBitRate
        };

        public static bool operator ==(TargetAudio left, TargetAudio right)
        {
            return EqualityComparer<TargetAudio>.Default.Equals(left, right);
        }

        public static bool operator !=(TargetAudio left, TargetAudio right)
        {
            return !(left == right);
        }






        public static TargetAudio AAC => new TargetAudio
        {
            Channels = 2,
            Codec = FFmpeg.Encoding.AudioCodecs.AAC,
            MaxKiloBitRate = 160
        };

        public static TargetAudio AC3 => new TargetAudio
        {
            Channels = 6,
            Codec = FFmpeg.Encoding.AudioCodecs.AC3,
            MaxKiloBitRate = 384
        };

        public static TargetAudio EAC3 => new TargetAudio
        {
            Channels = 6,
            Codec = FFmpeg.Encoding.AudioCodecs.EAC3,
            MaxKiloBitRate = 640
        };

        public static TargetAudio MP3 => new TargetAudio
        {
            Channels = 2,
            Codec = FFmpeg.Encoding.AudioCodecs.MP3,
            MaxKiloBitRate = 192
        };

        
    }
}
