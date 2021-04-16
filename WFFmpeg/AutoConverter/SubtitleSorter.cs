using System;

namespace WFFmpeg.AutoConverter
{
    class SubtitleSorter : IComparable
    {
        public Media.SubtitleStream Source { get; set; }

        private bool IsValidLanguage => Source.IsEnglish 
                || Source.Language.ICStartsWith("unk")
                || Source.Language.ICStartsWith("und");

        public int CompareTo(object obj)
        {
            SubtitleSorter comp = (SubtitleSorter)obj;

            int ret = -Source.IsEnglish.CompareTo(comp.Source.IsEnglish);
            if (ret == 0) ret = -IsValidLanguage.CompareTo(comp.IsValidLanguage);
            if (ret == 0) ret = -Source.IsForced.CompareTo(comp.Source.IsForced);
            if (ret == 0) ret = -Source.IsText.CompareTo(comp.Source.IsText);
            if (ret == 0) ret = -Source.IsCC.CompareTo(comp.Source.IsCC);
            if (ret == 0) ret = -Source.IsDefault.CompareTo(comp.Source.IsDefault);

            return ret;
        }
    }
}
