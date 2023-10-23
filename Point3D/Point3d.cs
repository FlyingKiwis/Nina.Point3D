using NINA.Core.Utility;
using NINA.Plugin;
using NINA.Plugin.Interfaces;
using NINA.Profile.Interfaces;
using NINA.WPF.Base.Interfaces.Mediator;
using NINA.WPF.Base.Interfaces.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NINA.Point3D.Helpers;
using Settings = NINA.Point3d.Properties.Settings;
using NINA.Profile;
using System.Windows.Media;
using NINA.Point3d.Util;

namespace NINA.Point3d {

    [Export(typeof(IPluginManifest))]
    public class Point3d : PluginBase, INotifyPropertyChanged 
        {

        public static Guid PluginGuid { get; private set; }
        private readonly IProfileService _profileService;
        private readonly PluginOptionsAccessor _pluginOptions;

        [ImportingConstructor]
        public Point3d(IProfileService profileService, IOptionsVM options, IImageSaveMediator imageSaveMediator) {
            if (Settings.Default.UpdateSettings) {
                Settings.Default.Upgrade();
                Settings.Default.UpdateSettings = false;
                CoreUtil.SaveSettings(Settings.Default);
            }

            PluginGuid = Guid.Parse(Identifier);
            _pluginOptions = new PluginOptionsAccessor(profileService, PluginGuid);

            _profileService = profileService;
            _profileService.ProfileChanged += ProfileService_ProfileChanged;
        }

        private void ProfileService_ProfileChanged(object sender, EventArgs e) {
            RaisePropertyChanged(nameof(ModelColor));
            RaisePropertyChanged(nameof(OTAStyle));
            RaisePropertyChanged(nameof(XOffset));
            RaisePropertyChanged(nameof(YOffset));
            RaisePropertyChanged(nameof(ZOffset));
        }

        public override Task Teardown() {
            return base.Teardown();
        }

        public double XOffset {
            get {
                return _pluginOptions.GetValueDouble(PluginOptions.XOffset, 0);
            }
            set {
                _pluginOptions.SetValueDouble(PluginOptions.XOffset, value);
                RaisePropertyChanged();
            }
        }

        public double YOffset {
            get {
                return _pluginOptions.GetValueDouble(PluginOptions.YOffset, 0);
            }
            set {
                _pluginOptions.SetValueDouble(PluginOptions.YOffset, value);
                RaisePropertyChanged();
            }
        }

        public double ZOffset {
            get {
                return _pluginOptions.GetValueDouble(PluginOptions.ZOffset, 0);
            }
            set {
                _pluginOptions.SetValueDouble(PluginOptions.ZOffset, value);
                RaisePropertyChanged();
            }
        }

        public Color ModelColor {
            get {
                return _pluginOptions.GetValueColor(PluginOptions.ModelColor, System.Drawing.Color.Red.ToMediaColor());
            }
            set {
                _pluginOptions.SetValueColor(PluginOptions.ModelColor, value);
                RaisePropertyChanged();
            }
        }

        public Model3DType OTAStyle {
            get {
                return _pluginOptions.GetValueEnum(PluginOptions.OTAStyle, Model3DType.Default);
            }
            set {
                _pluginOptions.SetValueEnum(PluginOptions.OTAStyle, value);
                RaisePropertyChanged();
            }
        }

        public static Dictionary<String, Color> ModelColors { get; } = new Dictionary<String, Color> {
            { "Red", Color.FromArgb(255, 255, 0, 0) },
            { "Blue", Color.FromArgb(255, 0, 0, 255) },
            { "Green", Color.FromArgb(255, 0, 255, 0) },
            { "Black", Color.FromArgb(255, 0, 0, 0) },
            { "White", Color.FromArgb(255, 255, 255, 255) }
        };

        public static Dictionary<String, Model3DType> OTAStyles { get; } = new Dictionary<String, Model3DType> {
            { "Default", Model3DType.Default },
            { "SCT", Model3DType.SchmidtCassegrain },
            { "RC", Model3DType.RitcheyChretien },
            { "RC Truss", Model3DType.RitcheyChretienTruss },
            { "Reflector", Model3DType.Reflector },
            { "Refractor", Model3DType.Refractor },
        };

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
