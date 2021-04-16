using EasyProc;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WFFmpeg.FFmpeg
{
    public class Runner
    {
        public async Task RunAsync(string[] args, string progressPrefix = null, IProgress<Progress> progress = null, CancellationToken cancellationToken = default)
        {
            _progress = progress;

            for (int i = 0; i < args.Length; i++)
            {
                _duration = 0;
                _percent = 0;
                _text = progressPrefix;
                if (args.Length > 1)
                    _text += $" (Pass {i + 1} of {args.Length})";

                progress?.Report(new Progress { Text = _text });

                var cmd = new Command();
                cmd.OnStdErr += (sender, e) => ParseText(e.Text);
                int exitCode = await cmd.RunAsync(Configuration.FFmpeg, args[i], cancellationToken).ConfigureAwait(false);
                if (exitCode != 0)
                    throw new Exception($"Failed to run ffmpeg. Exit code {exitCode}");

                progress?.Report(new Progress { Text = progressPrefix, Percent = 1d });
            }
        }

        private IProgress<Progress> _progress = null;
        private double _duration = 0;
        private double _percent = 0;
        private string _text = null;

        private void ParseText(string text)
        {
            if (_progress == null)
                return;

            Match match;

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

            _progress.Report(new Progress
            {
                Text = _text,
                Percent = _percent
            });
        }
    }
}
