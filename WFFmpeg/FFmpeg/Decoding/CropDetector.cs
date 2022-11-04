using EasyProc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WFFmpeg.FFmpeg.Decoding
{
    public class CropDetector
    {
        public EventHandler<Progress> OnProgressChanged;

        public static string BuildArgs(string inputFile) => $"-i \"{inputFile}\" -vf fps=1/30,cropdetect=24:2:0 -f null -";

        public async Task<Rectangle> RunAsync(string inputFile, IProgress<Progress> progress = null, CancellationToken cancellationToken = default)
        {
            _progress = progress;
            _started = DateTime.Now;

            _crops = new List<string>();
            var cmd = new Command();
            cmd.OnStdErr += (sender, e) => ParseText(e.Text);
            int exitCode = await cmd.RunAsync(Configuration.FFmpeg, BuildArgs(inputFile), cancellationToken).ConfigureAwait(false);
            if (exitCode != 0)
                throw new Exception($"Failed to run ffmpeg. Exit code {exitCode}");

            if (_crops.Count == 0)
                throw new Exception("Could not detect cropping");

            //Count how many of each crop appeared
            var cnts = new Dictionary<string, int>();
            foreach(string s in _crops)
            {
                if (!cnts.ContainsKey(s))
                    cnts.Add(s, 0);
                cnts[s] += 1;
            }

            //Get the crop(s) that appeared the most often
            int max = cnts.Values.Max(item => item);
            var maxCrops = cnts
                .Where(item => item.Value == max)
                .Select(item => item.Key)
                .ToList();

            //Use foreach, incase there are 2+ crops that happen the same number of times
            int maxW = -1, maxH = -1, minX = int.MaxValue, minY = int.MaxValue;
            foreach(string crop in maxCrops)
            {
                int[] parts = crop
                    .Split(':')
                    .Select(item => int.Parse(item))
                    .ToArray();

                maxW = Math.Max(maxW, parts[0]);
                maxH = Math.Max(maxH, parts[1]);
                minX = Math.Min(minX, parts[2]);
                minY = Math.Min(minY, parts[3]);
            }

            progress?.Report(new Progress(TEXT, 1d, true, _started));
            OnProgressChanged?.Invoke(this, new Progress(TEXT, 1d, true, _started));

            return new Rectangle
            {
                Height = maxH,
                Width = maxW,
                X = minX,
                Y = minY
            };
        }

        private IProgress<Progress> _progress = null;
        private double _duration = 0;
        private double _percent = 0;
        private DateTime _started = DateTime.Now;

        private const string TEXT = "Detecting cropping";
        private List<string> _crops = new List<string>();

        private void ParseText(string text)
        {
            Match match = new Regex("crop=((\\d+\\:\\d+\\:\\d+\\:\\d+))").Match(text);
            if (match.Success)
                try { _crops.Add(match.Groups[1].Value); }
                catch { }

            if (_progress == null && OnProgressChanged == null)
                return;

            if (_duration == 0)
            {
                match = new Regex("[D|d]uration:.((\\d|:|\\.)*)").Match(text);
                if (match.Success)
                    try { _duration = TimeSpan.Parse(match.Groups[1].Value).TotalSeconds; }
                    catch { }
            }

            if (_duration > 0)
            {
                match = new Regex("time=((\\d|:|\\.)*)").Match(text);
                if (match.Success)
                    try { _percent = Math.Max(0d, Math.Min(1d, TimeSpan.Parse(match.Groups[1].Value).TotalSeconds / _duration)); }
                    catch { }
            }

            try { _progress?.Report(new Progress(TEXT, _percent, false, _started)); }
            catch { }
            try { OnProgressChanged?.Invoke(this, new Progress(TEXT, _percent, false, _started)); }
            catch { }
        }
    }
}
