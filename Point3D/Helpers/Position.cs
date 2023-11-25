using NINA.Core.Enum;
using NINA.Core.Utility;
using NINA.Equipment.Equipment.MyTelescope;
using System;
using System.Windows.Media.Media3D;

namespace NINA.Point3d.Helpers {
    public static class Position {
        public static Vector3D TelescopeInfoToXYZ(TelescopeInfo telescopeInfo) {
            var xOffset = -90;
            var yOffset = -90;

            var elevation = -Math.Abs(telescopeInfo.SiteLatitude);
            var adjustedRa = telescopeInfo.RightAscension;
            var adjustedDec = 180 - telescopeInfo.Declination;
            if (UseWestCoordinates(telescopeInfo)) {
                adjustedRa = telescopeInfo.RightAscension - 12;
                adjustedDec = telescopeInfo.Declination;
                Logger.Debug("Using west coordinates");
            }
            adjustedDec = telescopeInfo.SiteLatitude > 0 ? adjustedDec : -adjustedDec;
            var hourAngleDeg = (telescopeInfo.SiderealTime - adjustedRa) * 15.0;
            Logger.Debug($"HourAngle={hourAngleDeg} adjustedRa={adjustedRa} adjustedDec={adjustedDec} elevation={elevation}");

            return new Vector3D(adjustedDec + xOffset, hourAngleDeg + yOffset, elevation);
        }

        private static bool UseWestCoordinates(TelescopeInfo telescopeInfo) { 
            if (telescopeInfo.SideOfPier == PierSide.pierUnknown)
                return telescopeInfo.SiteLatitude > 0 ? true : false;

            return telescopeInfo.SideOfPier == PierSide.pierWest;
        }
    }
}
