using System;

namespace WFFmpeg.Media
{
    public class Chapter
    {
        public string Title { get; set; }

        public double Start { get; set; }

        public double End { get; set; }

        public TimeSpan StartTime => TimeSpan.FromSeconds(Start);

        public TimeSpan EndTime => TimeSpan.FromSeconds(End);

        public double Duration => End - Start;

        public TimeSpan DurationTime => TimeSpan.FromSeconds(Duration);
    }
}
