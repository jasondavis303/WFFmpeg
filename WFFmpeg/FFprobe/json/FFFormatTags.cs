using Newtonsoft.Json;
using System;

namespace WFFmpeg.FFprobe.json
{
    class FFFormatTags
    {
        [JsonProperty("ENCODER", NullValueHandling = NullValueHandling.Ignore)]
        public string Encoder { get; set; }

        [JsonProperty("major_brand", NullValueHandling = NullValueHandling.Ignore)]
        public string MajorBrand { get; set; }

        [JsonProperty("minor_brand", NullValueHandling = NullValueHandling.Ignore)]
        public string MinorBrand { get; set; }

        [JsonProperty("compatible_brands", NullValueHandling = NullValueHandling.Ignore)]
        public string CompatibleBrands { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("genre", NullValueHandling = NullValueHandling.Ignore)]
        public string Genre { get; set; }

        [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
        public string Date { get; set; }

        [JsonProperty("show", NullValueHandling = NullValueHandling.Ignore)]
        public string Show { get; set; }

        [JsonProperty("network", NullValueHandling = NullValueHandling.Ignore)]
        public string Network { get; set; }

        [JsonProperty("season_number", NullValueHandling = NullValueHandling.Ignore)]
        public int? Season { get; set; }

        [JsonProperty("episode_sort", NullValueHandling = NullValueHandling.Ignore)]
        public int? Episode { get; set; }
        
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("synopsis", NullValueHandling = NullValueHandling.Ignore)]
        public string Synopsis { get; set; }

        [JsonProperty("hd_video", NullValueHandling = NullValueHandling.Ignore)]
        //[JsonConverter(typeof(FFParseBoolNumStringConverter))]
        public byte? HDVideo { get; set; }

        [JsonProperty("media_type", NullValueHandling = NullValueHandling.Ignore)]
        public string MediaType { get; set; }

        [JsonProperty("purchase_date", NullValueHandling = NullValueHandling.Ignore)]
        public string PurchaseDate { get; set; }

        [JsonProperty("account_type", NullValueHandling = NullValueHandling.Ignore)]
        public byte? AccountType { get; set; }

        [JsonProperty("iTunEXTC", NullValueHandling = NullValueHandling.Ignore)]
        public string iTunEXTC { get; set; }

        [JsonProperty("iTunMOVI", NullValueHandling = NullValueHandling.Ignore)]
        public string iTunMOVI { get; set; }
    }

}
