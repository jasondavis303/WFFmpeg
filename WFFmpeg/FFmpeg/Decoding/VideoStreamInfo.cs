using System;

namespace WFFmpeg.FFmpeg.Decoding
{
    public class VideoStreamInfo
    {
        public TimeSpan Duration { get; set; }
        public int Frames { get; set; }
        public long KiloBytes { get; set; }

        public double FrameRate => Duration == TimeSpan.Zero ? 0d : (double)Frames / Duration.TotalSeconds;

        public int BitRate => Duration == TimeSpan.Zero ? 0 : (int)Math.Ceiling(((double)KiloBytes * 8192) / Duration.TotalSeconds);

        public int KiloBitRate => (int)Math.Ceiling((double)BitRate / 1000);
    }
}
