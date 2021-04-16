namespace WFFmpeg.FFmpeg
{
    public class Progress
    {
        public string Text { get; set; }
        public double Percent { get; set; }

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
