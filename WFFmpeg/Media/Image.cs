using System.Linq;

namespace WFFmpeg.Media
{
    public class Image
    {
        public int Index { get; set; }

        public string CodecName { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string PixelFormat { get; set; }

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


        public override string ToString() => $"{Index}: {CodecName} / {Width}x{Height}";
    }
}
