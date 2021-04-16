using System;
using System.IO;

namespace WFFmpeg
{
    public static class Configuration
    {
        /// <summary>
        /// Set the directory containing ffmpeg executables. If ffmpeg is installed globally or in the PATH variable, this does not need to be set
        /// </summary>
        public static string FFmpegDirectory { get; set; }

        private static string Ext => Environment.OSVersion.Platform == PlatformID.Win32NT ? ".exe" : string.Empty;
        
        private static string BuildPath(string app)
        {
            string ret = app + Ext;
            if (!string.IsNullOrWhiteSpace(FFmpegDirectory))
                ret = Path.Combine(FFmpegDirectory, ret);
            return ret;
        }

        /// <summary>
        /// Get the path to the ffmpeg exe
        /// </summary>
        public static string FFmpeg => BuildPath("ffmpeg");

        /// <summary>
        /// Get the path to the ffprobe exe
        /// </summary>
        public static string FFprobe => BuildPath("ffprobe");

        /// <summary>
        /// Get the path to the ffplay exe
        /// </summary>
        public static string FFplay => BuildPath("ffplay");
    }
}
