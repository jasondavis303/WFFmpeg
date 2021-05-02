namespace WFFmpeg.FFmpeg
{
    public class Progress
    {
        internal Progress(string text, double percent, bool done)
        {
            Text = text;
            Percent = percent;
            Done = done;
        }

        public string Text { get; }
        public double Percent { get; }
        public bool Done { get; }

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
