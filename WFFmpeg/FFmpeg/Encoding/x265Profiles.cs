using System;
using System.Collections.Generic;
using System.Text;

namespace WFFmpeg.FFmpeg.Encoding
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "x265 is a proper name")]
    public enum x265Profiles
    {
        Main,
        Main_Intra,
        MainStillPicture,
        Main444_8,
        Main444_Intra,
        Main444_StillPicture,
        Main10,
        Main10_Intra,
        Main422_10,
        Main422_10_Intra,
        Main444_10,
        Main444_10_Intra,
        Main12,
        Main12_Intra,
        Main422_12,
        Main422_12_Intra,
        Main444_12,
        Main444_12_Intra
    }
}
