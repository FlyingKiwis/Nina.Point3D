using NINA.Point3d.Properties;
using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.Image.ImageData;
using NINA.Plugin;
using NINA.Plugin.Interfaces;
using NINA.Profile;
using NINA.Profile.Interfaces;
using NINA.WPF.Base.Interfaces.Mediator;
using NINA.WPF.Base.Interfaces.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Settings = NINA.Point3d.Properties.Settings;
using System.Drawing;
using NINA.Point3D.Helpers;

namespace NINA.Point3d {
    /// <summary>
    /// This class exports the IPluginManifest interface and will be used for the general plugin information and options
    /// The base class "PluginBase" will populate all the necessary Manifest Meta Data out of the AssemblyInfo attributes. Please fill these accoringly
    /// 
    /// An instance of this class will be created and set as datacontext on the plugin options tab in N.I.N.A. to be able to configure global plugin settings
    /// The user interface for the settings will be defined by a DataTemplate with the key having the naming convention "Point3d_Options" where Point3d corresponds to the AssemblyTitle - In this template example it is found in the Options.xaml
    /// </summary>
    [Export(typeof(IPluginManifest))]
    public class Point3d : PluginBase, INotifyPropertyChanged {

        [ImportingConstructor]
        public Point3d(IProfileService profileService, IOptionsVM options, IImageSaveMediator imageSaveMediator) {
            if (Settings.Default.UpdateSettings) {
                Settings.Default.Upgrade();
                Settings.Default.UpdateSettings = false;
                CoreUtil.SaveSettings(Settings.Default);
            }

        }

        public override Task Teardown() {
            return base.Teardown();
        }

        public double XOffset {
            get {
                return Settings.Default.XOffset;
            }
            set {
                Settings.Default.XOffset = value;
                CoreUtil.SaveSettings(Settings.Default);
                RaisePropertyChanged();
            }
        }

        public double YOffset {
            get {
                return Settings.Default.YOffset;
            }
            set {
                Settings.Default.YOffset = value;
                CoreUtil.SaveSettings(Settings.Default);
                RaisePropertyChanged();
            }
        }

        public double ZOffset {
            get {
                return Settings.Default.ZOffset;
            }
            set {
                Settings.Default.ZOffset = value;
                CoreUtil.SaveSettings(Settings.Default);
                RaisePropertyChanged();
            }
        }

        public Color ModelColor {
            get {
                return Settings.Default.ModelColor;
            }
            set {
                Settings.Default.ModelColor = value;
                CoreUtil.SaveSettings(Settings.Default);
                RaisePropertyChanged();
            }
        }

        public Model3DType OTAStyle {
            get {
                return Settings.Default.OTAStyle;
            }
            set {
                Settings.Default.OTAStyle = value;
                CoreUtil.SaveSettings(Settings.Default);
                RaisePropertyChanged();
            }
        }

        public static Dictionary<String, Color> ModelColors { get; } = new Dictionary<String, Color> {
            { "Red", Color.Red },
            { "Blue", Color.Blue },
            { "Green", Color.Green },
            { "Black", Color.Black },
            { "White", Color.White }
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
