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
using System.Reflection;
using NINA.Point3D.Helpers;
using System.IO;

namespace NINA.Point3D.Classes
{
    public static class Model3D
    {
        private static readonly string _directoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), "Resources");
        public static string GetModelFile(Model3DType modelType)
        {
            string gpModel;
            switch (modelType)
            {
                case Model3DType.Default:
                    gpModel = @"Default.obj";
                    break;
                case Model3DType.Reflector:
                    gpModel = @"Reflector.obj";
                    break;
                case Model3DType.Refractor:
                    gpModel = @"Refractor.obj";
                    break;
                case Model3DType.SchmidtCassegrain:
                    gpModel = @"SchmidtCassegrain.obj";
                    break;
                case Model3DType.RitcheyChretien:
                    gpModel = @"RitcheyChretien.obj";
                    break;
                case Model3DType.RitcheyChretienTruss:
                    gpModel = @"RitcheyChretienTruss.obj";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(modelType), modelType, null);
            }
            var filePath = Path.Combine(_directoryPath ?? throw new InvalidOperationException(), gpModel);
            var file = new Uri(filePath).LocalPath;
            return file;
        }
        public static string GetCompassFile(bool southernHemisphere)
        {
            const string compassN = @"CompassN.png";
            const string compassS = @"CompassS.png";
            var compassFile = southernHemisphere ? compassS : compassN;
            var filePath = Path.Combine(_directoryPath ?? throw new InvalidOperationException(), compassFile);
            var file = new Uri(filePath).LocalPath;
            return file;
        }
    }
}
