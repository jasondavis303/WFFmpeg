using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WFFmpeg.FFmpeg.Decoding
{
    public class StreamExtractor
    {        
        public static string BuildArgs(string inputFile, IEnumerable<StreamInfo> streams)
        {
            if (string.IsNullOrWhiteSpace(inputFile))
                throw new System.IO.FileNotFoundException("File not found", inputFile);

            if (streams == null)
                throw new ArgumentNullException(nameof(streams));


            string ret = null;

            string cv = null;
            string ca = null;
            string cs = null;
            string ci = null;

            int cnt = 0;
            foreach(var strm in streams)
            {
                cnt++;

                if (strm.Index < 0)
                    throw new ArgumentOutOfRangeException(nameof(strm.Index));

                if (string.IsNullOrWhiteSpace(strm.OutputFile))
                    throw new ArgumentNullException(nameof(strm.OutputFile));

                ret += $" -map 0:{strm.Index} -y \"{strm.OutputFile}\"";
                
                switch(strm.StreamType)
                {
                    case StreamType.Audio:
                        ca = " -c:a copy";
                        break;
                    case StreamType.Image:
                        ci = " -q:v 1";
                        break;
                    case StreamType.Text:
                        cs = " -c:s srt";
                        break;
                    case StreamType.Video:
                        cv = " -c:v copy";
                        break;
                }
            }

            if(cnt == 0)
                throw new ArgumentOutOfRangeException(nameof(streams));

            ret = $"-i \"{inputFile}\"{cv}{ca}{cs}{ci}{ret}";
                       
            return ret;
        }

        public Task RunAsync(string inputFile, int streamIndex, StreamType streamType, string outputFile, IProgress<Progress> progress = null, CancellationToken cancellationToken = default)
        {
            var streams = new StreamInfo[]
            {
                new StreamInfo
                {
                     Index = streamIndex,
                     OutputFile = outputFile,
                     StreamType = streamType
                }
            };
            return RunAsync(BuildArgs(inputFile, streams), progress, cancellationToken);
        }

        public Task RunAsync(Media.MediaFile mf, int streamIndex, string outputFile, IProgress<Progress> progress = null, CancellationToken cancellationToken = default)
        {
            int st = -1;
            try
            {
                mf.VideoStreams.First(item => item.Index == streamIndex);
                st = (int)StreamType.Video;
            }
            catch { }

            if (st == -1)
                try
                {
                    mf.AudioStreams.First(item => item.Index == streamIndex);
                    st = (int)StreamType.Audio;
                }
                catch { }

            if (st == -1)
                try
                {
                    mf.Images.First(item => item.Index == streamIndex);
                    st = (int)StreamType.Image;
                }
                catch { }

            if (st == -1)
                try
                {
                    mf.SubtitleStreams.First(item => item.Index == streamIndex);
                    st = (int)StreamType.Text;
                }
                catch { }

            if (st == -1)
                throw new Exception("Cannot find stream type");

            var streams = new StreamInfo[]
            {
                new StreamInfo
                {
                     Index = streamIndex,
                     OutputFile = outputFile,
                     StreamType = (StreamType)st
                }
            };

            return RunAsync(mf.Filename, streams, progress, cancellationToken);
        }

        public Task RunAsync(string inputFile, IEnumerable<StreamInfo> streams, IProgress<Progress> progress = null, CancellationToken cancellationToken = default)
        {
            return RunAsync(BuildArgs(inputFile, streams), progress, cancellationToken);
        }

        public Task RunAsync(string args, IProgress<Progress> progress = null, CancellationToken cancellationToken = default)
        {
            return new Runner().RunAsync(new string[] { args }, "Extracting", progress, cancellationToken);
        }
    }
}
