using System;
using System.Linq;

namespace WFFmpeg.Media
{
    public sealed class VideoStream
    {
        public int Index { get; set; }

        public string CodecName { get; set; }

        public string Profile { get; set; }

        public double Level { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string DisplayAspectRatio { get; set; }

        public double DisplayAspectRatioNum
        {
            get
            {
                try
                {
                    double[] parts = DisplayAspectRatio.Split(':').Select(item => double.Parse(item)).ToArray();
                    return parts[0] / parts[1];
                }
                catch { }
                return -1;
            }
        }

        public TimeSpan Duration { get; set; }

        public string FrameRateString { get; set; }

        public double FrameRate { get; set; }

        public bool VariableFramerate { get; set; }

        public bool IsAVC { get; set; }

        public bool IsHEVC { get; set; }

        public bool IsTelecined { get; set; }

        public bool IsInterlaced { get; set; }

        public string PixelFormat { get; set; }

        public int BitRate { get; set; }

        public int KiloBitRate => (int)Math.Ceiling((double)BitRate / 1000);

        public override string ToString()
        {
            string ret = $"{Index}: {CodecName}";
            if(!string.IsNullOrWhiteSpace(Profile))
            {
                ret += $" - {Profile}";
                if (Level != 0)
                    ret += $" @ {Level}";
            }

            ret += $" / {Width}x{Height} / {FrameRate} fps / {KiloBitRate} Kb/s";

            if (IsTelecined)
                ret += " / Telecined";

            if (IsInterlaced)
                ret += " / Interlaced";

            return ret;
        }
    }
}
