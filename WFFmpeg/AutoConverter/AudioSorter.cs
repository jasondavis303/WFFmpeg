using System;

namespace WFFmpeg.AutoConverter
{
    class AudioSorter : IComparable
    {
        public Media.AudioStream Source { get; set; }

        private bool IsValidLanguage => Source.IsEnglish
            || Source.Language.ICStartsWith("unk")
            || Source.Language.ICStartsWith("und");

        public int CompareTo(object obj)
        {
            //Title: commentary loses, all others equal
            //IsEnglish
            //Language: all others equal
            //Default
            //Codec: aac wins, all others equal
            //channels: if both aac: 2 channel wins, all others equal
            //Codec: neither are aac, so now search: DTS, TrueHD, EAC3, AC3, FLAC, AAC, Vorbis, MP3, MP2, all others equal
            //Channels
            //Bitrate
            AudioSorter comp = (AudioSorter)obj;

            int ret = Source.IsCommentary.CompareTo(comp.Source.IsCommentary);
            if (ret == 0) ret = -Source.IsEnglish.CompareTo(comp.Source.IsEnglish);
            if (ret == 0) ret = -IsValidLanguage.CompareTo(comp.IsValidLanguage);
            if (ret == 0) ret = -Source.IsDefault.CompareTo(comp.Source.IsDefault);

            if (ret == 0) ret = -(Source.IsAAC && !Source.Profile.ICEquals("HE"))
                    .CompareTo((comp.Source.IsAAC && !comp.Source.Profile.ICEquals("HE")));


            if (ret == 0) ret = -Source.IsAAC.CompareTo(comp.Source.IsAAC);
            if (ret == 0 && Source.IsAAC)
            {
                if (Source.Channels == 2) ret = -Convert.ToInt32(comp.Source.Channels != 2);
                else ret = Convert.ToInt32(comp.Source.Channels == 2);
            }

            foreach (string codec in new string[] { "dts", "truehd", "eac3", "ac3", "flac", "aac", "vorbis", "mp3", "mp2" })
                if (ret == 0)
                    ret = -Source.CodecName.ICEquals(codec).CompareTo(comp.Source.CodecName.ICEquals(codec));

            if (ret == 0) ret = -Source.Channels.CompareTo(comp.Source.Channels);
            if (ret == 0) ret = -Source.BitRate.CompareTo(comp.Source.BitRate);

            return ret;
        }
    }
}
