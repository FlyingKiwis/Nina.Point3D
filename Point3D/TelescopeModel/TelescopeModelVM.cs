﻿using HelixToolkit.Wpf;
using NINA.Core.Utility;
using NINA.Equipment.Equipment.MyTelescope;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Equipment.Interfaces.ViewModel;
using NINA.Point3d.Properties;
using NINA.Point3d.Util;
using NINA.Point3D.Classes;
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
using Settings = NINA.Point3d.Properties.Settings;

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

        [ImportingConstructor]
        public TelescopeModelVM(ITelescopeMediator telescopeMediator, IProfileService profileService) : base(profileService) {
            _telescopeMediator = telescopeMediator;
            _profileService = profileService;
            Title = "Telescope Model";

            _pluginOptions = new PluginOptionsAccessor(profileService, Point3d.PluginGuid);
            _profileService.ProfileChanged += ProfileService_ProfileChanged;
            _activeProfile = _profileService.ActiveProfile;

            if (_activeProfile != null) {
                _activeProfile.PropertyChanged += ActiveProfile_PropertyChanged;
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
            _activeProfile.PropertyChanged -= ActiveProfile_PropertyChanged;
            _activeProfile = _profileService.ActiveProfile;
            if (_activeProfile != null) {
                _activeProfile.PropertyChanged += ActiveProfile_PropertyChanged;
            }
            LoadView();
            LoadModel();
        }


        public double XAxisOffset { get; private set; } = 0;
        public double YAxisOffset { get; private set; } = 0;
        public double ZAxisOffset { get; private set; } = 0;
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
            _activeProfile.PropertyChanged -= ActiveProfile_PropertyChanged;
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

            XAxisOffset = axes[0] + _pluginOptions.GetValueDouble(PluginOptions.XOffset, 0);
            YAxisOffset = axes[1] + _pluginOptions.GetValueDouble(PluginOptions.YOffset, 0);
            ZAxisOffset = Math.Round(Math.Abs(latitude), 2) + _pluginOptions.GetValueDouble(PluginOptions.ZOffset, 0);

            RaisePropertyChanged(nameof(XAxisOffset));
            RaisePropertyChanged(nameof(YAxisOffset));
            RaisePropertyChanged(nameof(ZAxisOffset));
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

            Application.Current.Dispatcher.Invoke(() => {
                try {
                    var otaStyle = _pluginOptions.GetValueEnum(PluginOptions.OTAStyle, Model3DType.Default);
                    var modelColor = _pluginOptions.GetValueColor(PluginOptions.ModelColor, System.Drawing.Color.Red.ToMediaColor());

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

                    //color weights
                    var materialweights = MaterialHelper.CreateMaterial(new SolidColorBrush(Color.FromRgb(64, 64, 64)));
                    if (model.Children[1] is GeometryModel3D weights) { weights.Material = materialweights; }
                    //color bar
                    var materialbar = MaterialHelper.CreateMaterial(Brushes.Gainsboro);
                    if (model.Children[2] is GeometryModel3D bar) { bar.Material = materialbar; }

                    Model = model;
                    RaisePropertyChanged(nameof(Model));
                }
                catch (Exception ex) {
                    Logger.Error(ex);
                }
            });
        }
    }
}