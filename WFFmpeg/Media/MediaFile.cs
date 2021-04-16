using System;
using System.Collections.Generic;

namespace WFFmpeg.Media
{
    public sealed class MediaFile
    {
        public string Filename { get; set; }

        public TimeSpan Duration { get; set; }

        public long Size { get; set; }

        public int BitRate { get; set; }

        public int KiloBitRate => (int)Math.Ceiling((double)BitRate / 1000);

        public List<VideoStream> VideoStreams { get; set; } = new List<VideoStream>();

        public List<AudioStream> AudioStreams { get; set; } = new List<AudioStream>();

        public List<SubtitleStream> SubtitleStreams { get; set; } = new List<SubtitleStream>();

        public List<Image> Images { get; set; } = new List<Image>();

        public Metadata Meta { get; set; } = new Metadata();

        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
    }
}
