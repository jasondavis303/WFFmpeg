using System;
using System.Collections.Generic;

namespace WFFmpeg.Media
{
    public sealed class Metadata
    {
        public byte? AccountType { get; set; }

        public DateTime? Date { get; set; }
        
        /// <summary>
        /// Short Description
        /// </summary>
        public string Description { get; set; }

        public string Encoder { get; set; }

        public int? Episode { get; set; }

        public string Genre { get; set; }

        public byte? HDVideo { get; set; }

        public string Network { get; set; }

        public DateTime? Purchased { get; set; }

        public int? Season { get; set; }

        public string Show { get; set; }

        public string Title { get; set; }

        public string Rating { get; set; }

        public string Artist { get; set; }

        public string AlbumArtist { get; set; }

        public string Album { get; set; }

        public int? TrackNumber { get; set; }

        public int? TotalTracks { get; set; }

        public int? DiscNumber { get; set; }

        public int? TotalDiscs { get; set; }


        /// <summary>
        /// Long Description
        /// </summary>
        public string Synopsis { get; set; }

        public List<string> Cast { get; set; } = new List<string>();

        public List<string> Directors { get; set; } = new List<string>();

        public List<string> Producers { get; set; } = new List<string>();

        public List<string> ScreenWriters { get; set; } = new List<string>();

        public string Studio { get; set; }
    }
}
