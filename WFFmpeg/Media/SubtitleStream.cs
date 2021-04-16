using System;

namespace WFFmpeg.Media
{
    public sealed class SubtitleStream
    {
        public int Index { get; set; }

        public string CodecName { get; set; }

        public TimeSpan Duration { get; set; }

        public bool IsDefault { get; set; }

        public bool IsForced { get; set; }

        public string Language { get; set; }

        public string Title { get; set; }

        public bool IsEnglish { get; set; }

        public bool IsText { get; set; }

        public bool IsCC { get; set; }

        public override string ToString()
        {
            string ret = $"{Index}";
            if (IsDefault)
                ret += " (default)";
            if(IsForced)
                ret += " (forced)";

            ret += $": {CodecName}";

            if (!string.IsNullOrWhiteSpace(Language))
                ret += $" / {Language}";

            if (!string.IsNullOrWhiteSpace(Title))
                ret += $" / {Title}";

            return ret;
        }
    }
}
