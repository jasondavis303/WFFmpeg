using System;

namespace WFFmpeg.FFmpeg.Decoding
{
    public class AudioStreamInfo
    {
        public TimeSpan Duration { get; set; }
        public long KiloBytes { get; set; }

        public int BitRate => Duration == TimeSpan.Zero ? 0 : (int)Math.Ceiling(((double)KiloBytes * 8192) / Duration.TotalSeconds);

        public int KiloBitRate => (int)Math.Ceiling((double)BitRate / 1000);
    }
}
