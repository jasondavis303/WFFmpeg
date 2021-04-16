namespace WFFmpeg.FFmpeg.Encoding
{
    public enum VideoTunes
    {
        None,

        /// <summary>
        /// Not valid for x265
        /// </summary>
        Film,
        Animation,
        Grain,

        /// <summary>
        /// Not valid for x265
        /// </summary>
        StillImage,
        FastDecode,
        ZeroLatency,
        PSNR,
        SSIM
    }
}
