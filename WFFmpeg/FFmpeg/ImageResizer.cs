using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WFFmpeg.Media;

namespace WFFmpeg.FFmpeg
{
    public class ImageResizer : Runner
    {
        public async Task ResizeImageAsync(string inputFile, string outputFile, int maxW, int maxH = -1, CancellationToken cancellationToken = default)
        {
            var mf = await new FFprobe.Prober().RunAsync(inputFile, cancellationToken).ConfigureAwait(false);
            await ResizeImageAsync(mf, outputFile, maxW, maxH, cancellationToken).ConfigureAwait(false);
        }

        public Task ResizeImageAsync(MediaFile mf, string outputFile, int maxW, int maxH = -1, CancellationToken cancellationToken = default)
        {
            if (mf.Images[0].Width > maxW || mf.Images[0].Height > maxH)
            {
                var scaleW = (double)maxW / (double)mf.Images[0].Width;
                var scaleH = (double)maxH / (double)mf.Images[0].Height;
                var scale = Math.Min(scaleW, scaleH);
                int newW = Convert.ToInt32(Math.Floor(scale * (double)mf.Images[0].Width));
                int newH = maxH < 1 ? -2 : Convert.ToInt32(Math.Floor(scale * (double)mf.Images[0].Height));

                string args = $"-i \"{mf.Filename}\" -vf scale={newW}:{newH} -q:v 1 -y \"{outputFile}\"";
                return RunAsync(new string[] { args }, null, null, cancellationToken);
            }
            else
            {
                File.Copy(mf.Filename, outputFile, true);
                return Task.CompletedTask;
            }
        }
    }
}
