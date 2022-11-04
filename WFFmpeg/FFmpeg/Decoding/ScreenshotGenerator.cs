using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WFFmpeg.Media;

namespace WFFmpeg.FFmpeg.Decoding
{
    public class ScreenshotGenerator : Runner
    {
        /// <summary>
        /// Build args to extract 1 iamge
        /// </summary>
        public static string BuildArgs(string inputFile, double duration, string outputFile)
        {
            if (string.IsNullOrWhiteSpace(inputFile))
                throw new FileNotFoundException("File not found", inputFile);

            if (duration <= 0)
                throw new ArgumentOutOfRangeException(nameof(duration));

            int seconds = 300;
            if (duration < 390)
                seconds = (int)(duration / 2);

            return $"-ss {TimeSpan.FromSeconds(seconds)} -i \"{inputFile}\" -vframes 1 -q:v 1 -y \"{outputFile}\"";
        }        

        /// <summary>
        /// Build args to extrac images every n seconds. Also returns expected number of images
        /// </summary>
        public static (string, int) BuildArgs(MediaFile mf, string argsFile, int secs, int maxW, string outputDirectory)
        {

            if (secs < 1)
                throw new ArgumentOutOfRangeException(nameof(secs));

            int maxH = -2;
            try { maxH = Convert.ToInt32((double)maxW / mf.VideoStreams[0].DisplayAspectRatioNum); }
            catch { }


            int totalSecs = Convert.ToInt32(Math.Floor(mf.VideoStreams[0].Duration.TotalSeconds));
            if(totalSecs < 1)
                totalSecs = Convert.ToInt32(Math.Floor(mf.Duration.TotalSeconds));

            string args = "select='eq(n,1)";
            int cnt = 1;
            for (int i = 0; i < totalSecs - secs; i += secs)
            {
                args += $" +lt(prev_pts*TB,{i})*gte(pts*TB,{i})";
                cnt++;
            }
            args += "'";
            if (maxW > 0)
                args += $",scale={maxW}:{maxH}";

            File.WriteAllText(argsFile, args);

            if (!(outputDirectory.EndsWith("\\") || outputDirectory.EndsWith("/")))
                outputDirectory += Path.DirectorySeparatorChar;

            string ret = $"-i \"{mf.Filename}\" -filter_complex_script \"{argsFile}\" -r {1 / (double)secs:0.000} -start_number 0 -y \"{outputDirectory}%08d.jpg\"";

            return (ret, cnt);
        }


        /// <summary>
        /// Generates a screenshot and returns the path to the screenshot filename
        /// </summary>
        public async Task<string> RunAsync(MediaFile mf, IProgress<Progress> progress = null, CancellationToken cancellationToken = default)
        {
            string tmpFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".jpg");
            string args = BuildArgs(mf.Filename, mf.Duration.TotalSeconds, tmpFile);

            await RunAsync(new string[] { args }, "Extracting", progress, cancellationToken).ConfigureAwait(false);

            return tmpFile;
        }


        /// <summary>
        /// Extract images every n seconds
        /// </summary>
        public async Task<int> RunAsync(MediaFile mf, string outputDirectory, int secs = 10, int maxW = 0, IProgress<Progress> progress = null, CancellationToken cancellationToken = default)
        {
            string tmpFile = Path.GetTempFileName();
            try
            {
                (string args, int cnt) = BuildArgs(mf, tmpFile, secs, maxW, outputDirectory);
                await RunAsync(new string[] { args }, "Extracting", progress, cancellationToken).ConfigureAwait(false);
                return cnt;
            }
            finally
            {
                File.Delete(tmpFile);
            }
        }        
    }
}
