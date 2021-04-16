namespace WFFmpeg.FFmpeg.Encoding
{
    public class SubtitleStream
    {
        /// <summary>
        /// Input stream index, if stream is internal
        /// </summary>
        public int Index { get; set; }

        public bool ExternalFile { get; set; }

        public string Filename { get; set; }

        public bool IsText { get; set; }

        /// <summary>
        /// eng, und for my purposes
        /// </summary>
        public string Language { get; set; }

        public bool Default { get; set; }

        public bool Forced { get; set; }
    }
}
