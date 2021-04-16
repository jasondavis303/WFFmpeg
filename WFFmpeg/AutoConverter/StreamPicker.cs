using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFFmpeg.AutoConverter
{
    public static class StreamPicker
    {
        public static Media.AudioStream PickBestAudio(IEnumerable<Media.AudioStream> streams)
        {
            if (streams == null)
                return null;

            var lst = streams
                .Select(item => new AudioSorter { Source = item })
                .ToList();

            if (lst.Count == 0)
                return null;

            lst.Sort();
            return lst[0].Source;
        }

        public static Media.SubtitleStream PickBestSubtitle(IEnumerable<Media.SubtitleStream> streams)
        {
            if (streams == null)
                return null;

            var lst = streams
                .Select(item => new SubtitleSorter { Source = item })
                .ToList();

            if (lst.Count == 0)
                return null;

            lst.Sort();
            return lst[0].Source;
        }
    }
}
