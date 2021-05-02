using EasyProc;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WFFmpeg.FFmpeg.Decoding
{
    public class StreamScanner
    {
        public static string BuildScanArgs(string inputFile, int index) => $"-i \"{inputFile}\" -map 0:{index} -c copy -f null -";

        public static string BuildVFRDetectArgs(string inputFile, int index, TimeSpan vidDuration) => $"-ss {vidDuration.TotalSeconds / 2:0.000} -t 100 -i \"{inputFile}\" -map 0:{index} -vf vfrdet -f null -";


        /// <summary>
        /// Try to calculate missing duration, framerate and bitrate from a video stream. NOT the most accurate
        /// </summary>
        public async Task<VideoStreamInfo> ScanVideoStreamAsync(string inputFile, int index, IProgress<Progress> progress = null, CancellationToken cancellationToken = default)
        {
            await ScanStreamAsync(BuildScanArgs(inputFile, index), progress, cancellationToken).ConfigureAwait(false);
                        
            return new VideoStreamInfo
            {
                Duration = TimeSpan.FromSeconds(_duration),
                Frames = _frames,
                KiloBytes = _vkb
            };
        }


        /// <summary>
        /// Try to calculate missing duration and bitrate from an audio stream. NOT the most accurate
        /// </summary>
        public async Task<AudioStreamInfo> ScanAudioStreamAsync(string inputFile, int index, IProgress<Progress> progress = null, CancellationToken cancellationToken = default)
        {
            await ScanStreamAsync(BuildScanArgs(inputFile, index), progress, cancellationToken).ConfigureAwait(false);

            return new AudioStreamInfo
            {
                Duration = TimeSpan.FromSeconds(_duration),
                KiloBytes = _akb
            };
        }


        public async Task<bool> ScanVideoForVariableFramerate(string inputFile, int index, TimeSpan vidDuration, IProgress<Progress> progress = null, CancellationToken cancellationToken = default)
        {
            await ScanStreamAsync(BuildVFRDetectArgs(inputFile, index, vidDuration), progress, cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(_lastVFR))
                throw new Exception("Something went wrong - no crop info in output!");

            if (double.TryParse(_lastVFR.Split(' ').First(item => item.ICStartsWith("VFR:")).Split(':')[1], out double d))
                if (d != 0d)
                    return true;

            return false;
        }



        private async Task ScanStreamAsync(string args, IProgress<Progress> progress, CancellationToken cancellationToken)
        {
            _progress = progress;
            _duration = 0;
            _frames = 0;
            _vkb = 0;
            _akb = 0;
            _lastLine = null;
            _lastVFR = null;

            progress?.Report(new Progress("Scanning", 0, false));

            var cmd = new Command();
            cmd.OnStdErr += (sender, e) => ParseText(e.Text);
            await cmd.RunAsync(Configuration.FFmpeg, args, cancellationToken).ConfigureAwait(false);
            

            //Get the sizes
            string[] sections = (_lastLine + string.Empty).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string section in sections)
            {
                string[] pair = section.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (pair[0] == "video")
                    try { _vkb = int.Parse(pair[1].Substring(0, pair[1].Length - 2)); }
                    catch { }
                
                if (pair[0].ICEquals("audio"))
                    try { _akb = int.Parse(pair[1].Substring(0, pair[1].Length - 2)); }
                    catch { }
            }

            progress?.Report(new Progress("Scanning", 1d, true));

        }


        private IProgress<Progress> _progress = null;
        private double _duration = 0;
        private int _frames = 0;
        private int _vkb = 0;
        private int _akb = 0;
        private string _lastLine = null;
        private string _lastVFR = null;


        private void ParseText(string text)
        {
            Match match;

            if (_duration == 0)
            {
                match = new Regex("[D|d]uration:.((\\d|:|\\.)*)").Match(text);
                if (match.Success)
                    try { _duration = TimeSpan.Parse(match.Groups[1].Value).TotalSeconds; }
                    catch { }
            }

            if(_frames == 0)
            {
                match = new Regex("frame=\\s*(\\d+)").Match(text);
                if (match.Success)
                    if (int.TryParse(match.Groups[1].Value, out int frames))
                        _frames = frames;
            }           

            if (_duration > 0)
            {
                match = new Regex("time=((\\d|:|\\.)*)").Match(text);
                if (match.Success)
                    try
                    {
                        _progress?.Report(new Progress("Scanning", TimeSpan.Parse(match.Groups[1].Value).TotalSeconds / _duration, false));
                    }
                    catch { }
            }

            _lastLine = text;
            
            if (text.ICStartsWith("[Parsed_vfrdet_"))
                _lastVFR = text;
        }
    }
}
