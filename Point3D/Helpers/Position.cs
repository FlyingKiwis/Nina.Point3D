using NINA.Core.Enum;
using NINA.Core.Utility;
using NINA.Equipment.Equipment.MyTelescope;
using System;
using System.Windows.Media.Media3D;

namespace NINA.Point3d.Helpers {

    public static class Position {

        public static Vector3D TelescopeInfoToXYZ(TelescopeInfo telescopeInfo, bool useSideOfPier) {
            Logger.Debug($"Use side of pier={useSideOfPier}");

            var xOffset = -90;
            var yOffset = -90;

            var elevation = -Math.Abs(telescopeInfo.SiteLatitude);
            var adjustedDec = 180 - telescopeInfo.Declination;
            var hourAngleDeg = GetHourAngleDegree(telescopeInfo);
            var adjustedHourAngleDeg = hourAngleDeg;
            var sopWest = UseWestCoordinates(telescopeInfo, useSideOfPier);
            if (sopWest) {
                adjustedHourAngleDeg = adjustedHourAngleDeg - 180;
                adjustedDec = telescopeInfo.Declination;
                Logger.Debug("Using west coordinates");
            }
            adjustedDec = telescopeInfo.SiteLatitude > 0 ? adjustedDec : -adjustedDec;
            Logger.Debug($"HourAngle={adjustedHourAngleDeg} RA={telescopeInfo.RightAscension} adjustedDec={adjustedDec} elevation={elevation} Use west?={sopWest} Telescope SOP={telescopeInfo.SideOfPier}");

            return new Vector3D(adjustedDec + xOffset, adjustedHourAngleDeg + yOffset, elevation);
        }

        private static bool UseWestCoordinates(TelescopeInfo telescopeInfo, bool useSideOfPier) {
            if (useSideOfPier) {
                if (telescopeInfo.SideOfPier != PierSide.pierUnknown) {
                    return telescopeInfo.SideOfPier == PierSide.pierWest;
                }
            }

            var hourAngleDeg = GetHourAngleDegree(telescopeInfo);

            if (hourAngleDeg < 180) {
                return false;
            }

            return true;
        }

        private static double GetHourAngleDegree(TelescopeInfo telescopeInfo) {
            var hourAngleDeg = (telescopeInfo.SiderealTime - telescopeInfo.RightAscension) * 15.0;

            while (hourAngleDeg < 0) {
                hourAngleDeg += 360.0;
            }

            return hourAngleDeg;
        }
    }
}