using GS.Point3D.Classes;
using NINA.Equipment.Equipment.MyTelescope;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Equipment.Interfaces.ViewModel;
using NINA.Profile.Interfaces;
using NINA.WPF.Base.ViewModel;
using System.ComponentModel.Composition;

namespace NINA.Point3d.Telescope {

    [Export(typeof(IDockableVM))]
    public class TelescopeModelVM : DockableVM, ITelescopeConsumer
    {

        [ImportingConstructor]
        public TelescopeModelVM(ITelescopeMediator telescopeMediator, IProfileService profileService) : base(profileService)
        {
        }

        public void Dispose() {
        }

        public void UpdateDeviceInfo(TelescopeInfo deviceInfo) {
            var ra = deviceInfo.RightAscension;
            var dec = deviceInfo.Declination;
            var alignMode = deviceInfo.AlignmentMode;
            var sourthernHem = deviceInfo.SiteLatitude < 0;

            var raDec = Axes.RaDecToAxesXY(alignMode.ToGsAlignMode(), new[] { ra, dec });
            var axes = Model3D.RotateModel(raDec[0], raDec[1], sourthernHem);
        }


    }
}
