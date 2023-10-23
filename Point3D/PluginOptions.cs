using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NINA.Point3d {
    public static class PluginOptions 
    {
        public static string XOffset => nameof(XOffset);
        public static string YOffset => nameof(YOffset);
        public static string ZOffset => nameof(ZOffset);
        public static string OTAStyle => nameof(OTAStyle);
        public static string ModelColor => nameof(ModelColor);
        public static string UpDirection => nameof(UpDirection);
        public static string LookDirection => nameof(LookDirection);
        public static string Position => nameof(Position);
    }
}
