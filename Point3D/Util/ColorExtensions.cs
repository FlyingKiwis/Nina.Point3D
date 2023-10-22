using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NINA.Point3d.Util {
    public static class ColorExtensions 
    {
        public static Color ToMediaColor(this System.Drawing.Color color) {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
