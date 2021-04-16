using System;

namespace WFFmpeg.Media
{
    public sealed class AudioStream
    {
        public int Index { get; set; }

        public string CodecName { get; set; }

        public string Profile { get; set; }

        public TimeSpan Duration { get; set; }

        public bool IsAAC { get; set; }

        public bool IsMP3 { get; set; }

        public bool IsAC3 { get; set; }

        public int SampleRate { get; set; }

        public int Channels { get; set; }        

        public bool IsDefault { get; set; }

        public string Language { get; set; }

        public string Title { get; set; }

        public int BitRate { get; set; }

        public int KiloBitRate => (int)Math.Ceiling((double)BitRate / 1000);

        public bool IsEnglish { get; set; }

        public bool IsCommentary { get; set; }

        public override string ToString()
        {
            string ret = $"{Index}";
            if (IsDefault)
                ret += " (default)";
            
            ret += $": {CodecName}"; 
            if (!string.IsNullOrWhiteSpace(Profile))
                ret += $" - {Profile}";

            ret += $" / {Channels} ch / {KiloBitRate} Kb/s / {SampleRate} KHz/s";

            if (!string.IsNullOrWhiteSpace(Language))
                ret += $" / {Language}";

            if (!string.IsNullOrWhiteSpace(Title))
                ret += $" / {Title}";

            return ret;
        }
    }
}
