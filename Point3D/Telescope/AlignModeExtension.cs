using NINA.Core.Enum;
using GS.Point3D.Helpers;

namespace NINA.Point3d.Telescope {
    public static class AlignModeExtension 
    {
        public static AlignMode ToGsAlignMode(this AlignmentMode alignmentMode) 
        {
            switch (alignmentMode) {
                case AlignmentMode.AltAz:
                    return AlignMode.algAltAz;
                case AlignmentMode.Polar:
                    return AlignMode.algPolar;
                case AlignmentMode.GermanPolar:
                    return AlignMode.algGermanPolar;
                default:
                    return AlignMode.algUnknown;
            }
        }
    }
}
