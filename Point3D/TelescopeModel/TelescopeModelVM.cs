using HelixToolkit.Wpf;
using NINA.Core.Utility;
using NINA.Equipment.Equipment.MyTelescope;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Equipment.Interfaces.ViewModel;
using NINA.Point3d.Properties;
using NINA.Point3d.Util;
using NINA.Point3D.Classes;
using NINA.Point3D.Helpers;
using NINA.Profile.Interfaces;
using NINA.WPF.Base.ViewModel;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Model3D = NINA.Point3D.Classes.Model3D;

namespace NINA.Point3d.TelescopeModel {

    [Export(typeof(IDockableVM))]
    public class TelescopeModelVM : DockableVM, ITelescopeConsumer {
        private readonly ITelescopeMediator _telescopeMediator;

        private bool? _isSouthernHem = null;

        private Color _modelColor;
        private Model3DType _otaType;

        [ImportingConstructor]
        public TelescopeModelVM(ITelescopeMediator telescopeMediator, IProfileService profileService) : base(profileService) {
            _telescopeMediator = telescopeMediator;
            _telescopeMediator.RegisterConsumer(this);
            Title = "Telescope Model";

            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            LoadModel(true);

            LookDirection = new Vector3D(43, 2157, -747);
            UpDirection = new Vector3D(0, 0.5, 0.85);
            Position = new System.Windows.Media.Media3D.Point3D(-135, -2324, 956);
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            LoadModel();
        }

        public double XAxisOffset { get; private set; } = 90;
        public double YAxisOffset { get; private set; } = -90;
        public double ZAxisOffset { get; private set; } = 0;
        public string RightAscension { get; private set; } = "None yet";
        public string Declination { get; private set; } = "None yet";
        public bool ModelOn { get; private set; } = false;
        public Material Compass { get; private set; }
        public bool CameraVis { get; private set; } = false;
        public System.Windows.Media.Media3D.Model3D Model { get; private set; }

        private Vector3D _upDirection;
        public Vector3D UpDirection {
            get {
                return _upDirection;
            }
            set {
                _upDirection = value;
                RaisePropertyChanged(nameof(UpDirection));
            }
        }

        private Vector3D _lookDirection;
        public Vector3D LookDirection {
            get {
                return _lookDirection;
            }
            set {
                _lookDirection = value;
                RaisePropertyChanged(nameof(LookDirection));
            }
        }

        private System.Windows.Media.Media3D.Point3D _position;
        public System.Windows.Media.Media3D.Point3D Position
            {
            get { return _position; }
            set {
                _position = value;
                RaisePropertyChanged(nameof(Position));
            }
            }

        public void Dispose() {
            _telescopeMediator.RemoveConsumer(this);
            Settings.Default.PropertyChanged -= Settings_PropertyChanged;
        }

        public void UpdateDeviceInfo(TelescopeInfo deviceInfo) {
            Logger.Debug(string.Empty);

            if (!ModelOn) {
                ModelOn = true;
                RaisePropertyChanged(nameof(ModelOn));
            }

            LoadModel();
            
            var ra = deviceInfo.RightAscension;
            var dec = deviceInfo.Declination;
            var alignMode = deviceInfo.AlignmentMode;
            var sourthernHem = deviceInfo.SiteLatitude < 0;
            var latitude = deviceInfo.SiteLatitude;
            var siderealTime = deviceInfo.SiderealTime;
            var sideOfPier = deviceInfo.SideOfPier;

            if(!_isSouthernHem.HasValue || _isSouthernHem.Value != sourthernHem) {
                _isSouthernHem = sourthernHem;
                Compass = MaterialHelper.CreateImageMaterial(Model3D.GetCompassFile(sourthernHem), 100);

                RaisePropertyChanged(nameof(Compass));
            }

            var raDec = Axes.RaDecToAxesXY(alignMode, ra, dec, siderealTime, sourthernHem, sideOfPier);
            var axes = Model3D.RotateModel(raDec[0], raDec[1], sourthernHem);

            XAxisOffset = axes[0] + Settings.Default.XOffset;
            YAxisOffset = axes[1] + Settings.Default.YOffset;
            ZAxisOffset = Math.Round(Math.Abs(latitude), 2) + Settings.Default.ZOffset;

            RightAscension = deviceInfo.RightAscensionString;
            Declination = deviceInfo.DeclinationString;

            RaisePropertyChanged(nameof(XAxisOffset));
            RaisePropertyChanged(nameof(YAxisOffset));
            RaisePropertyChanged(nameof(ZAxisOffset));
            RaisePropertyChanged(nameof(RightAscension));
            RaisePropertyChanged(nameof(Declination));
        }

        private void LoadModel(bool force = false) {

            Application.Current.Dispatcher.Invoke(() => {

                if (!force) {
                    if (Settings.Default.OTAStyle == _otaType && Settings.Default.ModelColor.Equals(_modelColor))
                        return;
                }

                _modelColor = Settings.Default.ModelColor.ToMediaColor();
                _otaType = Settings.Default.OTAStyle;

                Logger.Debug($"Color={_modelColor} Style={_otaType.ToString()}");

                var import = new ModelImporter();
                var model = import.Load(Model3D.GetModelFile(_otaType));

                var accentbrush = new SolidColorBrush(_modelColor);

                var materialota = MaterialHelper.CreateMaterial(accentbrush);
                if (model.Children[0] is GeometryModel3D ota) ota.Material = materialota;

                //color weights
                var materialweights = MaterialHelper.CreateMaterial(new SolidColorBrush(Color.FromRgb(64, 64, 64)));
                if (model.Children[1] is GeometryModel3D weights) { weights.Material = materialweights; }
                //color bar
                var materialbar = MaterialHelper.CreateMaterial(Brushes.Gainsboro);
                if (model.Children[2] is GeometryModel3D bar) { bar.Material = materialbar; }

                Model = model;
                RaisePropertyChanged(nameof(Model));
            });
        }
    }
}
