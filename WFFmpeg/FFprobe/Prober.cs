using EasyProc;
using iTunes.Metadata;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WFFmpeg.FFprobe.json;
using WFFmpeg.Media;

namespace WFFmpeg.FFprobe
{
    public class Prober
    {
        /// <summary>
        /// Returns the args to run ffprobe
        /// </summary>
        public static string BuildArgs(string inputFile) =>  $"-v error -print_format json -show_chapters -show_format -show_streams \"{inputFile}\"";

        public async Task<MediaFile> RunAsync(string filename, CancellationToken cancellationToken = default)
        {        
            var sbJson = new StringBuilder();
            var sbError = new StringBuilder();

            var cmd = new Command();
            cmd.OnStdOut += (sender, e) => sbJson.AppendLine(e.Text);
            cmd.OnStdErr += (sender, e) => sbError.AppendLine(e.Text);

            int exitCode = await cmd.RunAsync(Configuration.FFprobe, BuildArgs(filename), cancellationToken).ConfigureAwait(false);
            if (exitCode != 0)
            {
                string errTxt = (sbError.ToString() + string.Empty).Trim();
                if (errTxt.ICStartsWith(filename + ": "))
                    throw new Exception(errTxt.Substring(filename.Length + 2));

                throw new Exception("Failed to run ffprobe");
            }


            var ff = FFResult.FromJson(sbJson.ToString());

            MediaFile ret = new MediaFile { Filename = filename };

            //File info
            ret.BitRate = ff.Format?.BitRate ?? 0;
            ret.Size = ff.Format?.Size ?? 0;
            ret.Duration = TimeSpan.FromSeconds(ff.Format?.Duration ?? 0d);

            //Chapters
            if (ff.Chapters != null)
            {
                foreach (var chapter in ff.Chapters)
                    ret.Chapters.Add(new Chapter
                    {
                        Title = chapter.Tags?.Title,
                        Start = chapter.StartTime ?? 0,
                        End = chapter.EndTime ?? 0
                    });
            }


            //Metadata
            if (ff.Format != null && ff.Format.Tags != null)
            {
                ret.Meta.AccountType = ff.Format.Tags.AccountType;

                if (DateTime.TryParse(ff.Format.Tags.Date, out DateTime dt))
                    ret.Meta.Date = dt;

                if (DateTime.TryParse(ff.Format.Tags.PurchaseDate, out dt))
                    ret.Meta.Purchased = dt;


                ret.Meta.Description = ff.Format.Tags.Description;
                ret.Meta.Encoder = ff.Format.Tags.Encoder;
                ret.Meta.Episode = ff.Format.Tags.Episode;
                ret.Meta.Genre = ff.Format.Tags.Genre;
                ret.Meta.HDVideo = ff.Format.Tags.HDVideo;
                ret.Meta.Network = ff.Format.Tags.Network;
                ret.Meta.Season = ff.Format.Tags.Season;
                ret.Meta.Show = ff.Format.Tags.Show;
                ret.Meta.Synopsis = ff.Format.Tags.Synopsis;
                ret.Meta.Title = ff.Format.Tags.Title;

                var extc = iTunEXTC.Read(ff.Format.Tags.iTunEXTC);
                ret.Meta.Rating = extc.Rating;

                var movi = iTunMOVI.Read(ff.Format.Tags.iTunMOVI);
                ret.Meta.Cast.AddRange(movi.Cast);
                ret.Meta.Directors.AddRange(movi.Directors);
                ret.Meta.Producers.AddRange(movi.Producers);
                ret.Meta.ScreenWriters.AddRange(movi.ScreenWriters);
                ret.Meta.Studio = movi.Studio;
            }


            //Streams
            foreach (var istrm in ff.Streams?.OrderBy(item => item.Index))
            {
                if (istrm.CodecType == "video")
                {
                    bool realVid = true;
                    if (istrm.Disposition != null)
                        if (istrm.Disposition.AttachedPic)
                            realVid = false;

                    if (new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" }.ICContains(System.IO.Path.GetExtension(filename)))
                        realVid = false;

                    if (realVid)
                    {
                        var strm = new VideoStream
                        {
                            Index = istrm.Index,

                            CodecName = istrm.CodecName,
                            Profile = istrm.Profile,
                            Level = istrm.Level,

                            IsAVC = istrm.IsAvc || istrm.CodecName.ICEquals("h264") || istrm.CodecName.ICEquals("avc") || istrm.CodecName.ICEquals("avc1"),
                            IsHEVC = istrm.CodecName.ICEquals("hevc") || istrm.CodecName.Equals("h265"),

                            Height = istrm.Height == 0 ? istrm.CodedHeight : istrm.Height,
                            Width = istrm.Width == 0 ? istrm.CodedWidth : istrm.Width,

                            BitRate = istrm.BitRate,

                            PixelFormat = istrm.PixFmt,

                            DisplayAspectRatio = istrm.DisplayAspectRatio,

                            Duration = TimeSpan.FromSeconds(istrm.Duration ?? 0d)
                        };

                        //Fix Bitrate
                        if (strm.BitRate == 0)
                            strm.BitRate = istrm.Tags?.BPSEng ?? 0;

                        //Fix Level
                        if (strm.IsAVC || strm.IsHEVC)
                            strm.Level /= 10;

                        //Framerate
                        try
                        {
                            strm.FrameRateString = istrm.AvgFrameRate;
                            strm.VariableFramerate = istrm.RFrameRate != istrm.AvgFrameRate;
                            double[] parts = istrm.AvgFrameRate.Split('/').Select(item => double.Parse(item)).ToArray();
                            strm.FrameRate = Math.Round(parts[0] / parts[1], 3);
                        }
                        catch { }

                        //Misc
                        if (!istrm.FieldOrder.ICEquals("progressive"))
                        {
                            strm.IsInterlaced = new string[] { "tt", "bb", "tb", "bt" }.ICContains(istrm.FieldOrder);

                            if (!strm.IsInterlaced)
                                strm.IsTelecined = istrm.CodecName.ICEquals("mpeg2video") &&
                                    Math.Round(strm.FrameRate, 2) == 29.97d;
                        }

                        ret.VideoStreams.Add(strm);
                    }
                    else
                    {
                        ret.Images.Add(new Image
                        {
                            Index = istrm.Index,
                            CodecName = istrm.CodecName,
                            Height = istrm.Height == 0 ? istrm.CodedHeight : istrm.Height,
                            Width = istrm.Width == 0 ? istrm.CodedWidth : istrm.Width,
                            PixelFormat = istrm.PixFmt,
                            DisplayAspectRatio = istrm.DisplayAspectRatio
                        });
                    }
                }



                if (istrm.CodecType == "audio")
                {
                    var strm = new AudioStream
                    {
                        Index = istrm.Index,

                        CodecName = istrm.CodecName,
                        Profile = istrm.Profile,

                        BitRate = istrm.BitRate == 0 ? istrm.Tags?.BPSEng ?? 0 : istrm.BitRate,
                        Channels = istrm.Channels,
                        SampleRate = istrm.SampleRate,
                        Duration = TimeSpan.FromSeconds(istrm.Duration ?? 0d),

                        Title = istrm.Tags?.Title,
                        Language = string.IsNullOrWhiteSpace(istrm.Tags?.Language) ? "und" : istrm.Tags?.Language,

                        IsDefault = istrm.Disposition?.Default ?? false,

                        IsAAC = istrm.CodecName.ICEquals("aac"),
                        IsMP3 = istrm.CodecName.ICEquals("mp3"),
                        IsAC3 = istrm.CodecName.ICContains("ac3"),

                        IsCommentary = (istrm.Disposition?.Comment ?? false) || (istrm.Tags?.Title.ICContains("commenta") ?? false),

                        IsEnglish = (istrm.Tags?.Language.ICEquals("eng") ?? false) || 
                                    (istrm.Tags?.Language.ICEquals("cpe") ?? false) || 
                                    (istrm.Tags?.Language.ICStartsWith("en") ?? false)
                    };

                    //Fix Bitrate
                    if (strm.BitRate == 0)
                        strm.BitRate = istrm.Tags?.BPSEng ?? 0;

                    ret.AudioStreams.Add(strm);
                }



                if (istrm.CodecType == "subtitle")
                {
                    var strm = new SubtitleStream
                    {
                        Index = istrm.Index,

                        CodecName = istrm.CodecName,

                        Duration = TimeSpan.FromSeconds(istrm.Duration ?? 0d),

                        IsDefault = istrm.Disposition?.Default ?? false,
                        IsForced = istrm.Disposition?.Forced ?? false,

                        Title = istrm.Tags?.Title,
                        Language = string.IsNullOrWhiteSpace(istrm.Tags?.Language) ? "und" : istrm.Tags?.Language,

                        IsEnglish = (istrm.Tags?.Language.ICEquals("eng") ?? false) ||
                                    (istrm.Tags?.Language.ICStartsWith("en") ?? false) ||
                                    (istrm.Tags?.Language.ICEquals("cpe") ?? false),

                        IsText = new string[] { "ass", "eia_608", "eia_708", "cea_608", "mov_text", "subrip" }.ICContains(istrm.CodecName),
                        IsCC = istrm.CodecName.ICEquals("eia_608") || istrm.CodecName.ICEquals("eia_708")
                    };

                    ret.SubtitleStreams.Add(strm);
                }

            }

            return ret;
        }
    
    }
}
