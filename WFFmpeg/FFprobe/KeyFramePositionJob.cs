using EasyProc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WFFmpeg.FFprobe
{
    public class KeyFramePositionJob
    {
        public static string BuildArgs(string file, double start = -1, double end = -1)
        {
            string ret = "-v quiet -print_format default=noprint_wrappers=1:nokey=1 -select_streams v:0 -show_frames -skip_frame nokey -show_entries frame=pkt_pts_time";
            
            if (start >= 0 && end >= 0)
                ret += $" -read_intervals {start:0.000}%+{end - start:0.000}";
            else if (start >= 0)
                ret += $" -read_intervals {start:0.000}";
            else if (end >= 0)
                ret += $" -read_intervals +{end:0.000}";
            
            ret += $" \"{file}\"";

            return ret;
        }


        public async Task<List<double>> RunAsync(string file, double start = -1, double end = -1, CancellationToken cancellationToken = default)
        {
            var ret = new List<double>();

            string args = BuildArgs(file, start, end);
            var cmd = new Command();
            cmd.OnStdOut += (sender, e) =>
            {
                if (double.TryParse(e.Text, out double d))
                    ret.Add(d);
            };
            int exitCode = await cmd.RunAsync(Configuration.FFprobe, args, cancellationToken).ConfigureAwait(false);
            if (exitCode != 0)
                throw new Exception("Failed to run ffprobe");

            ret.Sort();

            return ret;
        }

    }
}
