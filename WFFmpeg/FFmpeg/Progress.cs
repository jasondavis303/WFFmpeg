using System;

namespace WFFmpeg.FFmpeg
{
    public class Progress
    {
        internal Progress(string text, double percent, bool done, DateTime started)
        {
            Text = text;
            Percent = percent;
            Done = done;
            RunTime = DateTime.Now - started;
        }

        public string Text { get; }
        public double Percent { get; }
        public bool Done { get; }
        public TimeSpan RunTime { get; }

        public override string ToString()
        {
            string ret = Text;

            if (!string.IsNullOrWhiteSpace(ret))
                ret += ": ";
            ret += $"{Percent:0.00%}";

            return ret;
        }
    }
}
