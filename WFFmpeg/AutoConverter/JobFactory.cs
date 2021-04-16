using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using WFFmpeg.FFmpeg.Encoding;

namespace WFFmpeg.AutoConverter
{
    public class JobFactory
    {
        public static Job Build(Media.MediaFile mediaFile)
        {
            return Build(mediaFile, TargetVideo.HD_Film, TargetAudio.AAC);
        }

        public static Job Build(Media.MediaFile mediaFile, TargetVideo targetVideo, TargetAudio targetAudio)
        {
            var ret = new Job
            {
                VideoStream = BuildVideo(mediaFile, targetVideo),
                BurnSubtitleStream = BuildBurnSubtitle(mediaFile),
                InputFile = mediaFile.Filename,
                OutputFile = Path.Combine(Path.GetDirectoryName(mediaFile.Filename), Path.GetFileNameWithoutExtension(mediaFile.Filename) + "_encoded.mp4")
            };

            var astrm = BuildAudio(mediaFile, targetAudio);
            if (astrm != null)
                ret.AudioStreams.Add(astrm);

            var sstrm = BuildCopySubtitle(mediaFile);
            if (sstrm != null)
                ret.CopySubtitleStreams.Add(sstrm);
            
            return ret;
        }

        private static Size GetResolutionAsSize(VideoResolutions vr)
        {
            switch(vr)
            {
                case VideoResolutions.FHD:
                    return new Size(1920, 1080);

                case VideoResolutions.HD:
                    return new Size(1280, 720);

                case VideoResolutions.SD:
                    return new Size(720, 576);

                case VideoResolutions.UHD:
                    return new Size(3840, 2160);

                default:
                    //FHD
                    return GetResolutionAsSize(VideoResolutions.FHD);
            }
        }

        private static VideoStream BuildVideo(Media.MediaFile mediaFile, TargetVideo targetVideo)
        {
            if (mediaFile.VideoStreams.Count == 0)
                return null;

            if (targetVideo == null)
                return null;

            var inVS = mediaFile.VideoStreams[0];

            bool encode = mediaFile.SubtitleStreams
                .Where(item => item.IsEnglish || new string[] { "und", "unk" }.ICContains(item.Language))
                .Any(item => item.IsForced);

            var maxSize = GetResolutionAsSize(targetVideo.MaxResolution);
            if (!encode)
                encode = inVS.Height > maxSize.Height || inVS.Width > maxSize.Width;

            if (!encode)
                encode = inVS.KiloBitRate > targetVideo.MaxKiloBitRate;            

            if (!encode)
                encode = inVS.FrameRate > targetVideo.MaxFrameRate;

            if (!encode)
                encode = !(inVS.IsAVC || inVS.IsHEVC);

            if (!encode)
                encode = inVS.IsAVC && targetVideo.Codec != VideoCodecs.x264;

            if (!encode)
                encode = inVS.IsHEVC && targetVideo.Codec != VideoCodecs.x265;

            if (!encode)
                if (inVS.IsAVC || inVS.IsHEVC)
                    encode = inVS.Level > targetVideo.MaxLevel;

            if (!encode)
                encode = inVS.IsInterlaced || inVS.IsTelecined;

            if(!encode)
            {                       
                if(inVS.IsAVC && targetVideo.Codec == VideoCodecs.x264)
                {
                    var profs = new List<string>() { "constrained baseline", "baseline", "main", "high"  };
                    string comp = (inVS.Profile + string.Empty).ToLower().Trim();
                    if (profs.ICContains(comp))
                    {
                        int inIdx = profs.FindIndex(item => item.ICEquals(comp));

                        comp = (targetVideo.MaxProfile + string.Empty).ToLower().Trim();
                        if (string.IsNullOrWhiteSpace(comp))
                            comp = "high";

                        if (!profs.ICContains(comp))
                            comp = "high";

                        int outIdx = profs.FindIndex(item => item.ICEquals(comp));

                        encode = inIdx > outIdx;
                    }
                    else
                    {
                        encode = true;
                    }
                }
                else if (inVS.IsHEVC && targetVideo.Codec == VideoCodecs.x265)
                {
                    var profs = new List<string>() { "Main", "Main_Intra", "MainStillPicture", "Main444_8", "Main444_Intra", "Main444_StillPicture", "Main10", "Main10_Intra", "Main422_10", "Main422_10_Intra", "Main444_10", "Main444_10_Intra", "Main12", "Main12_Intra", "Main422_12", "Main422_12_Intra", "Main444_12", "Main444_12_Intra" };
                    for (int i = 0; i < profs.Count; i++)
                        profs[i] = profs[i].ToLower();

                    string comp = (inVS.Profile + string.Empty).ToLower().Trim().Replace("-", "_");
                    if (profs.ICContains(comp))
                    {
                        int inIdx = profs.FindIndex(item => item.ICEquals(comp));

                        comp = (targetVideo.MaxProfile + string.Empty).ToLower().Trim();
                        if (string.IsNullOrWhiteSpace(comp))
                            comp = "main";

                        if (!profs.ICContains(comp))
                            comp = "main";

                        int outIdx = profs.FindIndex(item => item.ICEquals(comp));

                        encode = inIdx > outIdx;
                    }
                    else
                    {
                        encode = true;
                    }
                }
                else
                {
                    encode = true;
                }
            }

            if (!encode)
                return new VideoStream
                {
                    Index = inVS.Index,
                    Method = EncodingMethods.Copy
                };

            var ret = new VideoStream
            {
                Codec = targetVideo.Codec,
                DeInterlace = inVS.IsInterlaced,
                DeTelecine = inVS.IsTelecined,
                Level = targetVideo.MaxLevel,
                Index = inVS.Index,
                Profile = targetVideo.MaxProfile,
                Speed = EncodeSpeed.S09_VerySlow,
                Tune = targetVideo.Animation ? VideoTunes.Animation : targetVideo.Codec == VideoCodecs.x265 ? VideoTunes.None : VideoTunes.Film
            };

            if (!string.IsNullOrWhiteSpace(targetVideo.PixelFormat))
                if (!string.IsNullOrWhiteSpace(inVS.PixelFormat))
                    if (inVS.PixelFormat != targetVideo.PixelFormat)
                        ret.PixelFormat = targetVideo.PixelFormat;


            //Size and framerate must come before bitrate to get the bitrate correct
            var resolutionForCalcs = new Size(inVS.Width, inVS.Height);
            var targetSize = GetResolutionAsSize(targetVideo.TargetResolution);
            if (inVS.Width > targetSize.Width || inVS.Height > targetSize.Height)
            {
                double sizeRatio = Math.Min((double)targetSize.Width / (double)inVS.Width, (double)targetSize.Height / (double)inVS.Height);
                resolutionForCalcs = new Size
                {
                    Width = (int)Math.Floor(inVS.Width * sizeRatio), 
                    Height = (int)Math.Floor(inVS.Height * sizeRatio)
                };

                if (resolutionForCalcs.Width % 2 != 0)
                    resolutionForCalcs.Width--;
                if (resolutionForCalcs.Height % 2 != 0)
                    resolutionForCalcs.Height--;

                ret.Scale = new Size(resolutionForCalcs.Width, -2);
            }

            double frameRateForCalcs = inVS.FrameRate;
            if (inVS.FrameRate > targetVideo.MaxFrameRate)
            {
                ret.FrameRate = Math.Min(inVS.FrameRate, targetVideo.TargetFrameRate);
                ret.VariableFrameRate = true;

                frameRateForCalcs = ret.FrameRate;
            }





            if (inVS.KiloBitRate >= targetVideo.MaxKiloBitRate)
            {
                ret.Method = EncodingMethods.AvgBitrate;
                int targetBitrate = (int)Math.Ceiling(((resolutionForCalcs.Width * resolutionForCalcs.Height * frameRateForCalcs) / targetVideo.QualityFactor) * targetVideo.NoiseFactor);
                ret.KiloBitrate = Math.Min(targetBitrate, targetVideo.MaxKiloBitRate);
            }
            else
            {
                ret.Method = EncodingMethods.Quality;
                ret.CRF = 18;                
            }
            ret.MaxKiloBitrate = targetVideo.MaxKiloBitRate;
            ret.MaxKiloBuffer = ret.MaxKiloBitrate * 2;

            return ret;
        }

        private static AudioStream BuildAudio(Media.MediaFile mediaFile, TargetAudio targetAudio)
        {
            if (mediaFile.AudioStreams == null)
                return null;

            if (targetAudio == null)
                return null;

            var best = StreamPicker.PickBestAudio(mediaFile.AudioStreams);
            if (best == null)
                return null;

            bool encode = best.SampleRate > 48000;

            //To do: Detect audio input bit depth?

            if (!encode)
                encode = best.KiloBitRate > targetAudio.MaxKiloBitRate;

            if (!encode)
                encode = targetAudio.Channels > 0 && best.Channels != targetAudio.Channels;

            if (!encode)
                encode = !best.CodecName.ICEquals(targetAudio.Codec.ToString());

            if (!encode)
                if (best.IsAAC)
                    if (targetAudio.Codec == AudioCodecs.AAC)
                        encode = !best.Profile.ICEquals("LC");

            if (!encode)
                return new AudioStream
                {
                    Index = best.Index,
                    Method = EncodingMethods.Copy,
                    Language = string.IsNullOrWhiteSpace(best.Language) || new string[] { "und", "unk" }.ICContains(best.Language) ? "eng" : best.Language,
                    Title = best.Title,
                    Commentary = best.IsCommentary
                };

            var ret = new AudioStream
            {
                Codec = targetAudio.Codec,
                Commentary = best.IsCommentary,
                Index = best.Index,
                Default = best.IsDefault,
                Language = string.IsNullOrWhiteSpace(best.Language) || new string[] { "und", "unk" }.ICContains(best.Language) ? "eng" : best.Language,
                SampleRate = best.SampleRate > 48000 ? 44100 : 0,
                Title = best.IsCommentary 
                    ? best.Title 
                    : best.Channels == 6 
                        ? targetAudio.Channels == best.Channels
                            ? "Dolby Surround 5.1"
                            : null
                    : best.Channels == 8
                        ? targetAudio.Channels == best.Channels
                            ? "Dolby Surround 7.1"
                            : null
                    : best.Channels >= 6
                        ? targetAudio.Channels == best.Channels
                            ? "Dolby Surround Sound"
                            : null
                    : null
            };

            //Channel Title
            if (best.IsCommentary)
            {
                ret.Title = best.Title;
            }
            else
            {
                if(best.Channels >= 6 && best.Channels == targetAudio.Channels)
                {
                    ret.Title = "Dolby Surround ";
                    if (best.Channels == 6)
                        ret.Title += "5.1";
                    else if (best.Channels == 8)
                        ret.Title += "7.1";
                    else
                        ret.Title += "Sound";
                }
            }


            if (targetAudio.Channels != 0 && best.Channels != targetAudio.Channels)
            {
                ret.Channels = targetAudio.Channels;
                if (best.Channels >= 6 && targetAudio.Channels == 2)
                {
                    ret.DolbyProLogic2 = true;
                    if (!best.IsCommentary)
                        ret.Title = "Dolby Pro Logic II";
                }
            }

            if (targetAudio.Channels == 0 && best.Channels == 1)
                ret.Channels = 2;
            
            if(best.KiloBitRate < targetAudio.MaxKiloBitRate)
            {
                ret.Method = EncodingMethods.Quality;
                if (ret.Codec == AudioCodecs.AAC)
                    ret.Quality = 4;
                if (ret.Codec == AudioCodecs.MP3)
                    ret.Quality = 3;
            }
            else
            {
                ret.Method = EncodingMethods.AvgBitrate;
                ret.KiloBitrate = targetAudio.MaxKiloBitRate;
            }
            
            return ret;
        }

        private static SubtitleStream BuildBurnSubtitle(Media.MediaFile mediaFile)
        {
            var strm = StreamPicker.PickBestSubtitle(mediaFile.SubtitleStreams.Where(item => item.IsForced));
            if (strm == null)
                return null;

            return new SubtitleStream
            {
                Forced = true,
                Index = strm.Index,
                IsText = strm.IsText,
                Language = string.IsNullOrWhiteSpace(strm.Language) || new string[] { "und", "unk" }.ICContains(strm.Language) ? "eng" : strm.Language
            };
        }
    
        private static SubtitleStream BuildCopySubtitle(Media.MediaFile mediaFile)
        {
            var strm = StreamPicker.PickBestSubtitle(mediaFile.SubtitleStreams.Where(item => item.IsText && !item.IsForced));
            if (strm == null)
                return null;

            return new SubtitleStream
            {
                Default = strm.IsDefault,
                Index = strm.Index,
                IsText = strm.IsText,
                Language = string.IsNullOrWhiteSpace(strm.Language) || new string[] { "und", "unk" }.ICContains(strm.Language) ? "eng" : strm.Language
            };
        }    
    }
}
