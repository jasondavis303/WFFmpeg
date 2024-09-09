using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WFFmpeg.FFmpeg.Encoding
{
    public class Encoder : Runner
    {
        public static string[] BuildArgs(Job job)
        {           
            var inputPass1 = BuildInputArgs(job, 1);
            var inputPass2 = BuildInputArgs(job, 2);

            (var filterPass1, var mapsPass1) = BuildFiltersAndMaps(job, 1);
            (var filterPass2, var mapsPass2) = BuildFiltersAndMaps(job, 2);

            var venc = BuildVideoEncoder(job);
            var aenc = BuildAudioEncoder(job);
            var senc = BuildSubtitleEncoder(job);

            var devNull = Environment.OSVersion.Platform == PlatformID.Win32NT ? "NUL" : "/dev/null";

            //ffmpeg creates copies of chapters. This flag disables the copy, leaving any original chapter intact
            string movFlags = job.VideoStream == null ? null : " -map_metadata -1 -movflags +faststart+disable_chpl";
            

            bool multiPass = job.VideoStream?.Method == EncodingMethods.AvgBitrate;
            string p1 = multiPass ? job.VideoStream.Codec == VideoCodecs.x264 ? " -pass 1" : "-x265-params pass=1" : null;
            string p2 = multiPass ? job.VideoStream.Codec == VideoCodecs.x264 ? " -pass 2" : "-x265-params pass=2" : null;

            var maxMuxingQueueSize = job.MaxMuxingQueueSize > 0 ? $" -max_muxing_queue_size {job.MaxMuxingQueueSize}" : null;

            var cmd1 = $"-fflags +genpts{inputPass1}{filterPass1}{mapsPass1}{venc}{p1} -f mp4 -y {devNull}";
            var cmd2 = $"-fflags +genpts{inputPass2}{filterPass2}{mapsPass2}{venc}{p2}{aenc}{senc}{movFlags}{maxMuxingQueueSize} -y \"{job.OutputFile}\"";

            var cmds = new List<string>();
            if (multiPass)
                cmds.Add(cmd1);
            cmds.Add(cmd2);

            return cmds.ToArray();
        }

        private static string BuildInputArgs(Job job, int pass)
        {
            var ret = new StringBuilder();

            if (job.StartTime > 0)
                ret.AppendFormat(" -ss {0}", TimeSpan.FromSeconds(job.StartTime).ToString("hh\\:mm\\:ss\\.fff"));

            ret.AppendFormat(" -i \"{0}\"", job.InputFile);

            if (pass == 2)
                if (job.CopySubtitleStreams != null)
                    foreach (var cstrm in job.CopySubtitleStreams)
                        if (cstrm.ExternalFile)
                        {
                            if(job.StartTime > 0)
                                ret.AppendFormat(" -ss {0}", TimeSpan.FromSeconds(job.StartTime).ToString("hh\\:mm\\:ss\\.fff"));
                            ret.AppendFormat(" -i \"{0}\"", cstrm.Filename);
                        }

            
            if (job.Duration > 0)
                ret.AppendFormat(" -to {0}", TimeSpan.FromSeconds(job.Duration).ToString("hh\\:mm\\:ss\\.fff"));
           

            return ret.ToString();
        }

        private static (string filter, string maps) BuildFiltersAndMaps(Job job, int pass)
        {
            var maps = new List<string>();

            var filters = new List<string>();

            if(job.VideoStream != null)
            {
                if (job.VideoStream.Method == EncodingMethods.Copy)
                {
                    maps.Add($"0:{job.VideoStream.Index}");
                }
                else
                {
                    var vf = new List<string>();

                    if (job.VideoStream.DeInterlace)
                    {
                        if (System.IO.File.Exists(Configuration.Nnedi3_Weights))
                            vf.Add($"nnedi=weights={FormatFilenameForFilter(Configuration.Nnedi3_Weights)}");
                        else
                            vf.Add("yadif=mode=1");
                    }
                    else if (job.VideoStream.DeTelecine)
                    {
                        vf.Add("pullup");
                    }

                    if (job.VideoStream.Crop.Width > 0 && job.VideoStream.Crop.Height > 0)
                    {
                        string s = $"crop={job.VideoStream.Crop.Width}:{job.VideoStream.Crop.Height}";
                        if (job.VideoStream.Crop.X > 0 || job.VideoStream.Crop.Y > 0)
                            s += $":{job.VideoStream.Crop.X}:{job.VideoStream.Crop.Y}";
                        vf.Add(s);
                    }

                    if (job.VideoStream.Scale.Width > 0 || job.VideoStream.Scale.Height > 0)
                        vf.Add($"scale={job.VideoStream.Scale.Width}:{job.VideoStream.Scale.Height}:flags=bicubic");

                    bool overlay = false;
                    if (job.BurnSubtitleStream != null)
                    {
                        if (job.BurnSubtitleStream.ExternalFile && job.BurnSubtitleStream.IsText)
                            vf.Add($"subtitles={FormatFilenameForFilter(job.BurnSubtitleStream.Filename)}");
                        else
                            overlay = true;
                    }

                    if (!string.IsNullOrWhiteSpace(job.VideoStream.PixelFormat))
                        vf.Add($"format={job.VideoStream.PixelFormat}");

                    if (vf.Count > 0 || overlay)
                    {
                        string joined = string.Join(",", vf);

                        if (vf.Count > 0 && overlay)
                            filters.Add($"[0:{job.VideoStream.Index}]{joined}[vx];[vx][1:{job.BurnSubtitleStream.Index}]overlay[outv]");

                        else if (vf.Count > 0)
                            filters.Add($"[0:{job.VideoStream.Index}]{joined}[outv]");

                        else //overlay
                            filters.Add($"[0:{job.VideoStream.Index}][1:{job.BurnSubtitleStream.Index}]overlay[outv]");

                        maps.Add("\"[outv]\"");
                    }
                    else
                    {
                        maps.Add($"0:{job.VideoStream.Index}");
                    }
                }
            }



            if (pass == 2)
            {
                int cnt = 0;
                foreach (var astrm in job.AudioStreams)
                {
                    if (astrm.Method == EncodingMethods.Copy)
                    {
                        maps.Add($"0:{astrm.Index}");
                    }
                    else
                    {
                        //Filter
                        if (astrm.SampleRate > 0 || (astrm.Channels == 2 && astrm.DolbyProLogic2))
                        {
                            maps.Add($"\"[outa]\"");

                            var af = new List<string>();

                            if (astrm.SampleRate > 0)
                                af.Add($"{astrm.SampleRate}:resampler=soxr");

                            if (astrm.DolbyProLogic2 && astrm.Channels == 2)
                                af.Add("matrix_encoding=dplii");

                            string joined = string.Join(":", af);
                            filters.Add($"[0:{astrm.Index}]aresample={joined}[outa]");
                        }
                        else
                        {
                            maps.Add($"0:{astrm.Index}");
                        }
                        
                        cnt++;
                    }
                }


                cnt = 0;
                foreach (var sstrm in job.CopySubtitleStreams)
                    if (sstrm.ExternalFile)
                        maps.Add($"{++cnt}:0");
                    else
                        maps.Add($"0:{sstrm.Index}");
            }


            string filterRet = string.Join(";", filters);
            if (!string.IsNullOrWhiteSpace(filterRet))
                filterRet = $" -filter_complex \"{filterRet}\"";

            string mapsRet = string.Join(" -map ", maps);
            if (!string.IsNullOrWhiteSpace(mapsRet))
                mapsRet = " -map " + mapsRet;

            return (filterRet, mapsRet);
        }
    
        private static string BuildVideoEncoder(Job job)
        {
            if (job.VideoStream == null)
                return null;

            if (job.VideoStream.Method == EncodingMethods.Copy)
                return " -c:v copy";

            var ret = new StringBuilder();

            ret.AppendFormat(" -c:v lib{0}", job.VideoStream.Codec.ToString());

            if (!string.IsNullOrWhiteSpace(job.VideoStream.Profile))
                ret.AppendFormat(" -profile:v {0}", job.VideoStream.Profile.ToLower().Replace('_', '-'));

            if (job.VideoStream.Level > 0)
                ret.AppendFormat(" -level:v {0}", job.VideoStream.Level);

            if (job.VideoStream.Codec == VideoCodecs.x265)
                if (job.VideoStream.Tier == x265Tiers.High)
                    ret.Append(" -tier:v high");

            if (job.VideoStream.Speed != EncodeSpeed.S06_Medium)
                ret.AppendFormat(" -preset {0}", job.VideoStream.Speed.ToString().ToLower().Substring(4));


            bool addTune = false;
            if (job.VideoStream.Tune != VideoTunes.None)
                if (job.VideoStream.Codec == VideoCodecs.x265)
                {
                    if (job.VideoStream.Tune != VideoTunes.Film && job.VideoStream.Tune != VideoTunes.StillImage)
                        addTune = true;
                }
                else
                {
                    addTune = true;
                }
            if (addTune)
                ret.AppendFormat(" -tune {0}", job.VideoStream.Tune.ToString().ToLower());



            if (job.VideoStream.Method == EncodingMethods.AvgBitrate && job.VideoStream.KiloBitrate > 0)
                ret.AppendFormat(" -b:v {0}k", job.VideoStream.KiloBitrate);

            else if (job.VideoStream.CRF >= 0)
                ret.AppendFormat(" -crf {0}", Math.Round(job.VideoStream.CRF, 2));


            if (job.VideoStream.MaxKiloBitrate > 0)
                ret.AppendFormat(" -maxrate {0}k", job.VideoStream.MaxKiloBitrate);

            if (job.VideoStream.MaxKiloBuffer > 0)
                ret.AppendFormat(" -bufsize {0}k", job.VideoStream.MaxKiloBuffer);

            
            if (job.VideoStream.FrameRate > 0)
            {
                if (job.VideoStream.VariableFrameRate)
                    ret.Append(" -vsync vfr");
                ret.AppendFormat(" -r {0}", Math.Round(job.VideoStream.FrameRate, 3));
            }

            //If encoding a tv episode, setup key frames to be 2 secs apart
            //This is to help out the Skip Intro funcitonality
            //var match = Regex.Match(job.InputFile, "\\b[Ss]\\d+\\s?[Ee]\\d+");
            //if(match.Success)
            //    ret.Append(" -force_key_frames \"expr:gte(t,n_forced*2)\"");
            if (job.VideoStream.ForceKeyFrameAt > 0)
                ret.Append($" -force_key_frames {job.VideoStream.ForceKeyFrameAt:0.000}");

            return ret.ToString();
        }

        private static string BuildAudioEncoder(Job job)
        {
            if (job.AudioStreams == null)
                return null;

            var ret = new StringBuilder();

            int cnt = 0;
            foreach (var astrm in job.AudioStreams)
            {
                ret.Append($" -c:a:{cnt}");

                if (astrm.Method == EncodingMethods.Copy)
                {
                    ret.Append($" copy");
                }
                else
                {
                    switch (astrm.Codec)
                    {
                        case AudioCodecs.AC3:
                            ret.Append(" ac3");
                            break;
                        case AudioCodecs.EAC3:
                            ret.Append(" eac3");
                            break;
                        case AudioCodecs.MP3:
                            ret.Append(" libmp3lame");
                            break;
                        default:
                            ret.Append(" libfdk_aac");
                            break;
                    }

                    if (astrm.Channels > 0)
                        ret.Append($" -ac:a:{cnt} {astrm.Channels}");

                    if (astrm.Method == EncodingMethods.Quality)
                    {
                        if (astrm.Codec == AudioCodecs.AAC)
                            ret.Append($" -vbr:a:{cnt} {astrm.Quality}");

                        if (astrm.Codec == AudioCodecs.MP3)
                            ret.Append($" -q:a:{cnt} {astrm.Quality}");

                    }
                    else //AvgBitrate
                    {
                        if (astrm.KiloBitrate > 0)
                            ret.Append($" -b:a:{cnt} {astrm.KiloBitrate}k");
                    }

                    if (!string.IsNullOrWhiteSpace(astrm.SampleFormat))
                        ret.Append($" -sample_fmt:a:{cnt} {astrm.SampleFormat}");

                }


                if (!string.IsNullOrWhiteSpace(astrm.Language))
                    ret.Append($" -metadata:s:a:{cnt} language={astrm.Language}");

                if (astrm.Default)
                    ret.Append($" -disposition:s:a:{cnt} default");

                cnt++;
            }

            return ret.ToString();
        }

        private static string BuildSubtitleEncoder(Job job)
        {
            var ret = new StringBuilder();

            int cnt = 0;
            foreach (var strm in job.CopySubtitleStreams)
            {
                ret.Append($" -c:s:{cnt}");
                if (strm.IsText)
                {
                    if (System.IO.Path.GetExtension(job.OutputFile).Trim('.').ICEquals("mp4"))
                        ret.Append(" mov_text");
                    else
                        ret.Append(" srt");
                }
                else
                {
                    ret.Append(" copy");
                }

                if (!string.IsNullOrWhiteSpace(strm.Language))
                    if (!strm.Language.ICEquals("und"))
                        if (!strm.Language.ICEquals("unk"))
                            ret.Append($" -metadata:s:s:{cnt} language={strm.Language}");


                //Note: forced doesnt seem to work on mp4
                if(strm.Forced)
                    ret.Append($" -disposition:s:s:{cnt} forced");

                else if (strm.Default)
                    ret.Append($" -disposition:s:s:{cnt} default");

                
                cnt++;
            }

            return ret.ToString();
        }

        private static string FormatFilenameForFilter(string filename)
        {
            /*                             
                C:\path\to\file.ext

                level 1 - escape (:) (\) (')
                C\:\\path\\to\\file.ext

                level 2 - put in single quotes after escaping (\) and  (')
                subtitles='C\:\\\path\\\to\\\file.ext'

                So in 1 step:
                string.Replace(\, \\\).Replace(', \\').Replace(:, \:)
            */
            return $"'{filename.Replace("\\", "\\\\\\").Replace("'", "\\\\").Replace(":", "\\:")}'";
        }

        public Task RunAsync(Job job, IProgress<Progress> progress = null, CancellationToken cancellationToken = default)
        {
            string[] args = BuildArgs(job);

            string task = "Transcoding";
            if (job.VideoStream != null && job.VideoStream.Method != EncodingMethods.Copy)
                task = "Encoding";
            if (args.Length > 1)
                task = "Encoding";

            return RunAsync(args, task, progress, cancellationToken);
        }
    }
}
