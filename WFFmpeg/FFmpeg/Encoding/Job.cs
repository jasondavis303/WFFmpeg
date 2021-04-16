using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace WFFmpeg.FFmpeg.Encoding
{
    public class Job
    {
        public string InputFile { get; set; }
        
        public double StartTime { get; set; }
        
        public double Duration { get; set; }
        
        public VideoStream VideoStream { get; set; }

        public List<AudioStream> AudioStreams { get; } = new List<AudioStream>();
        
        public List<SubtitleStream> CopySubtitleStreams { get; } = new List<SubtitleStream>();

        public SubtitleStream BurnSubtitleStream { get; set; }
        
        public string OutputFile { get; set; }

        public void Save(string filename)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            File.WriteAllText(filename, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Job Load(string filename) => JsonConvert.DeserializeObject<Job>(File.ReadAllText(filename));
    }
}
