/* Copyright(C) 2021  Rob Morgan (robert.morgan.e@gmail.com)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using NINA.Core.Enum;
using NINA.Core.Utility;
using Range = NINA.Point3D.Helpers.Range;

namespace NINA.Point3D.Classes {
    public static class Axes
    {

        static Axes()
        {

        }

        public static double[] RaDecToAxesXY(AlignmentMode mode, double rightAscension, double declination, double siderealTime, bool southernHemisphere, PierSide sideOfPier)
        {
            Logger.Trace($"{nameof(mode)}={mode} {nameof(rightAscension)}={rightAscension} {nameof(declination)}={declination} {nameof(siderealTime)}={siderealTime} {nameof(southernHemisphere)}={southernHemisphere} {nameof(sideOfPier)}={sideOfPier}");

            var axes = new[] { rightAscension, declination};
            switch (mode)
            {
                case AlignmentMode.AltAz:
                    //axes = Range.RangeAzAlt(axes);
                    //axes = Coordinate.RaDec2AltAz(axes[0], axes[1], SkyServer.SiderealTime, SkySettings.Latitude);
                    return axes;
                case AlignmentMode.GermanPolar:
                    axes[0] = (siderealTime - axes[0]) * 15.0;
                    if (southernHemisphere) { axes[1] = -axes[1]; }

                    Logger.Trace($"1: axes[0]={axes[0]} axes[1]={axes[1]}");

                    var axes3 = GetAltAxisPosition(axes);

                    Logger.Trace($"2: axes[0]={axes[0]} axes[1]={axes[1]} axes3[0]={axes3[0]} axes3[1]={axes3[1]}");

                    switch (sideOfPier)
                    {
                        case PierSide.pierUnknown:
                            break;
                        case PierSide.pierEast:
                            if (southernHemisphere)
                            {
                                // southern
                                axes[0] = axes[0];
                                axes[1] = axes[1];
                            }
                            else
                            {
                                axes[0] = axes[0];
                                axes[1] = axes[1];
                            }

                            break;
                        case PierSide.pierWest:
                            if (southernHemisphere)
                            {
                                // southern
                                axes[0] = axes3[0];
                                axes[1] = axes3[1];
                            }
                            else
                            {
                                axes[0] = axes3[0];
                                axes[1] = axes3[1];
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    Logger.Trace($"3: axes[0]={axes[0]} axes[1]={axes[1]} axes3[0]={axes3[0]} axes3[1]={axes3[1]}");
                    return axes;
                case AlignmentMode.Polar:
                    //axes[0] = (SkyServer.SiderealTime - axes[0]) * 15.0;
                    //axes[1] = (SkyServer.SouthernHemisphere) ? -axes[1] : axes[1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            axes = Range.RangeAxesXY(axes);
            return axes;
        }

        /// <summary>
        /// GEMs have two possible axes positions, given an axis position this returns the other 
        /// </summary>
        /// <param name="alt">position</param>
        /// <returns>other axis position</returns>
        private static double[] GetAltAxisPosition(double[] alt)
        {
            var d = new[] {0.0, 0.0};
            if (alt[0] > 90)
            {
                d[0] = alt[0] - 180;
                d[1] = 180 - alt[1];
            }
            else
            {
                d[0] = alt[0] + 180;
                d[1] = 180 - alt[1];
            }

            return d;
        }
    }
}
