using HelixToolkit.Wpf;
using NINA.Astrometry;
using NINA.Core.Utility;
using NINA.Equipment.Equipment.MyTelescope;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Equipment.Interfaces.ViewModel;
using NINA.Point3d.Util;
using NINA.Point3D.Helpers;
using NINA.Profile;
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
        private readonly IProfileService _profileService;
        private readonly IPluginOptionsAccessor _pluginOptions;

        private bool? _isSouthernHem = null;

        private Color _modelColor;
        private Model3DType _otaType;
        private IProfile _activeProfile;
        private static DateTime _lastInfoLog = DateTime.Now;

        [ImportingConstructor]
        public TelescopeModelVM(ITelescopeMediator telescopeMediator, IProfileService profileService) : base(profileService) {
            _telescopeMediator = telescopeMediator;
            _profileService = profileService;
            Title = "Telescope Model";

            _pluginOptions = new PluginOptionsAccessor(profileService, Point3d.PluginGuid);
            _profileService.ProfileChanged += ProfileService_ProfileChanged;
            _activeProfile = _profileService.ActiveProfile;

            if (_activeProfile != null) {
                _activeProfile.PluginSettings.PropertyChanged += ActiveProfile_PropertyChanged;
            }

            LoadModel(true);
            LoadView();
            _telescopeMediator.RegisterConsumer(this);
        }

        private void ActiveProfile_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            LoadModel();
        }

        private void ProfileService_ProfileChanged(object sender, EventArgs e) 
        {
            _activeProfile.PluginSettings.PropertyChanged -= ActiveProfile_PropertyChanged;
            _activeProfile = _profileService.ActiveProfile;
            if (_activeProfile != null) {
                _activeProfile.PluginSettings.PropertyChanged += ActiveProfile_PropertyChanged;
            }
            LoadView();
            LoadModel();
        }

        private double _xAxisOffset = 0;
        public double XAxisOffset { 
            get { 
                return _xAxisOffset;
            } 
            set {
                if (_xAxisOffset != value) {
                    _xAxisOffset = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _yAxisOffset = 0;
        public double YAxisOffset {
            get {
                return _yAxisOffset;
            }
            set {
                if (_yAxisOffset != value) {
                    _yAxisOffset = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _zAxisOffset = 0;
        public double ZAxisOffset {
            get {
                return _zAxisOffset;
            }
            set {
                if (_zAxisOffset != value) {
                    _zAxisOffset = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _xAxis = 0;
        public double XAxis {
            get {
                return _xAxis;
            }
            set {
                if (_xAxis != value) {
                    _xAxis = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _yAxis = 0;
        public double YAxis {
            get {
                return _yAxis;
            }
            set {
                if (_yAxis != value) {
                    _yAxis = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _zAxis = 0;
        public double ZAxis {
            get {
                return _zAxis;
            }
            set {
                if (_zAxis != value) {
                    _zAxis = value;
                    RaisePropertyChanged();
                }
            }
        }

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
                _pluginOptions.SetValueString(PluginOptions.UpDirection, value.ToString());
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
                _pluginOptions.SetValueString(PluginOptions.LookDirection, value.ToString());
                RaisePropertyChanged(nameof(LookDirection));
            }
        }

        private System.Windows.Media.Media3D.Point3D _position;
        public System.Windows.Media.Media3D.Point3D Position
            {
            get { return _position; }
            set {
                _position = value;
                _pluginOptions.SetValueString(PluginOptions.Position, value.ToString());
                RaisePropertyChanged(nameof(Position));
            }
            }

        public void Dispose() {
            _telescopeMediator.RemoveConsumer(this);
            _profileService.ProfileChanged -= ProfileService_ProfileChanged;
            _activeProfile.PluginSettings.PropertyChanged -= ActiveProfile_PropertyChanged;
        }

        public void UpdateDeviceInfo(TelescopeInfo deviceInfo) {
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

            Logger.Trace($"RA={ra} DEC={dec} Lat={latitude}");

            if (!_isSouthernHem.HasValue || _isSouthernHem.Value != sourthernHem) {
                _isSouthernHem = sourthernHem;
                Compass = MaterialHelper.CreateImageMaterial(Model3D.GetCompassFile(sourthernHem), 100);

                RaisePropertyChanged(nameof(Compass));
            }

            XAxisOffset = _pluginOptions.GetValueDouble(PluginOptions.XOffset, 0);
            YAxisOffset = _pluginOptions.GetValueDouble(PluginOptions.YOffset, 0);
            ZAxisOffset = _pluginOptions.GetValueDouble(PluginOptions.ZOffset, 0);

            var pos = Helpers.Position.TelescopeInfoToXYZ(deviceInfo, _profileService.ActiveProfile.MeridianFlipSettings.UseSideOfPier);

            var xPos = pos.X;
            var yPos = pos.Y;
            var zPos = pos.Z;

            XAxis = xPos + XAxisOffset;
            YAxis = yPos + YAxisOffset;
            ZAxis = zPos + ZAxisOffset;

            var msg = "Device position: ";
            msg += $"Latitude={AstroUtil.DegreesToDMS(latitude)}, ";
            msg += $"Longitude={AstroUtil.DegreesToDMS(deviceInfo.SiteLongitude)}, ";
            msg += $"Altitude={AstroUtil.DegreesToDMS(deviceInfo.Altitude)}, ";
            msg += $"Azimuth = {AstroUtil.DegreesToDMS(deviceInfo.Azimuth)}, ";
            msg += $"Declination = {AstroUtil.DegreesToDMS(dec)}, ";
            msg += $"RightAscension = {AstroUtil.HoursToHMS(ra)}, ";
            msg += $"DegX = {Math.Round(XAxis, 2)}°, ";
            msg += $"DegY = {Math.Round(YAxis, 2)}°, ";
            msg += $"DegZ = {Math.Round(ZAxis, 2)}°, ";
            msg += $"SiderealTime = {AstroUtil.HoursToHMS(siderealTime)}, ";
            msg += $"SideOfPier = {sideOfPier}, ";
            msg += $"UseSideOfPier = {_profileService.ActiveProfile.MeridianFlipSettings.UseSideOfPier}, ";
            msg += $"Southern Hem = {sourthernHem}";

            if (DateTime.Now - _lastInfoLog >= TimeSpan.FromMinutes(5)) {
                _lastInfoLog = DateTime.Now;
                Logger.Info(msg);
            } else {
                Logger.Trace(msg);
            }
        }

        private void LoadView() {

            var lookDirection = _pluginOptions.GetValueString(PluginOptions.LookDirection, null);
            try {
                LookDirection = Vector3D.Parse(lookDirection);
            }
            catch 
            {
                Logger.Info($"{nameof(PluginOptions.LookDirection)}={lookDirection} : not parseable, loading default");
                LookDirection = new Vector3D(43, 2157, -747);
            }

            var upDirection = _pluginOptions.GetValueString(PluginOptions.UpDirection, null);
            try { 
                UpDirection = Vector3D.Parse(upDirection);
            }
            catch {
                Logger.Info($"{nameof(PluginOptions.UpDirection)}={upDirection} : not parseable, loading default");
                UpDirection = new Vector3D(0, 0.5, 0.85);
            }

            var postion = _pluginOptions.GetValueString(PluginOptions.Position, null);
            try {
                Position = System.Windows.Media.Media3D.Point3D.Parse(postion);
            }
            catch {
                Logger.Info($"{nameof(PluginOptions.Position)}={postion} : not parseable, loading default");
                Position = new System.Windows.Media.Media3D.Point3D(-135, -2324, 956);
            }
        }

        private void LoadModel(bool force = false) {

            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                try {
                    var otaStyle = _pluginOptions.GetValueEnum(PluginOptions.OTAStyle, Model3DType.Default);
                    var modelColor = _pluginOptions.GetValueColor(PluginOptions.ModelColor, System.Drawing.Color.Red.ToMediaColor());

                    Logger.Debug($"Requested change: OTA Style={otaStyle} Model Color={modelColor} Current: OTA Style={_otaType} Model color:{_modelColor}");

                    if (!force) {
                        if (otaStyle == _otaType && modelColor.Equals(_modelColor))
                            return;
                    }

                    _modelColor = modelColor;
                    _otaType = otaStyle;

                    Logger.Debug($"Color={_modelColor} Style={_otaType}");

                    var import = new ModelImporter();
                    var model = import.Load(Model3D.GetModelFile(_otaType));

                    var accentbrush = new SolidColorBrush(_modelColor);

                    var materialota = MaterialHelper.CreateMaterial(accentbrush);
                    if (model.Children[0] is GeometryModel3D ota) ota.Material = materialota;

                    if (model.Children.Count >= 3) {
                        //color weights
                        var materialweights = MaterialHelper.CreateMaterial(new SolidColorBrush(Color.FromRgb(64, 64, 64)));
                        if (model.Children[1] is GeometryModel3D weights) { weights.Material = materialweights; }
                        //color bar
                        var materialbar = MaterialHelper.CreateMaterial(Brushes.Gainsboro);
                        if (model.Children[2] is GeometryModel3D bar) { bar.Material = materialbar; }
                    }

                    Model = model;
                    RaisePropertyChanged(nameof(Model));
                }
                catch (Exception ex) {
                    Logger.Error(ex);
                }
            }));
        }
    }
}
