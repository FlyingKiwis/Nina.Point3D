using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// [MANDATORY] The following GUID is used as a unique identifier of the plugin. Generate a fresh one for your plugin!
[assembly: Guid("200ce2d2-6992-44fe-bf83-f8c2e01c7244")]

// [MANDATORY] The assembly versioning
//Should be incremented for each new release build of a plugin
[assembly: AssemblyVersion("0.2.3.0")]
[assembly: AssemblyFileVersion("0.2.3.0")]

// [MANDATORY] The name of your plugin
[assembly: AssemblyTitle("Point3D")]
// [MANDATORY] A short description of your plugin
[assembly: AssemblyDescription("Displays a model of how the telescope is pointing")]

// The following attributes are not required for the plugin per se, but are required by the official manifest meta data

// Your name
[assembly: AssemblyCompany("Drew McDermott")]
// The product name that this plugin is part of
[assembly: AssemblyProduct("Point3D")]
[assembly: AssemblyCopyright("Copyright © 2023 Drew McDermott")]

// The minimum Version of N.I.N.A. that this plugin is compatible with
[assembly: AssemblyMetadata("MinimumApplicationVersion", "3.0.0.1085")]

// The license your plugin code is using
[assembly: AssemblyMetadata("License", "GPL-3.0")]
// The url to the license
[assembly: AssemblyMetadata("LicenseURL", "https://www.gnu.org/licenses/gpl-3.0.en.html")]
// The repository where your pluggin is hosted
[assembly: AssemblyMetadata("Repository", "https://github.com/FlyingKiwis/Nina.Point3D")]

// The following attributes are optional for the official manifest meta data

//[Optional] Your plugin homepage URL - omit if not applicaple
[assembly: AssemblyMetadata("Homepage", "https://github.com/FlyingKiwis/Nina.Point3D")]

//[Optional] Common tags that quickly describe your plugin
[assembly: AssemblyMetadata("Tags", "Point3D,Telescope,Model,Pointing,Simulation,Simulated")]

//[Optional] A link that will show a log of all changes in between your plugin's versions
[assembly: AssemblyMetadata("ChangelogURL", "https://github.com/FlyingKiwis/Nina.Point3D/blob/master/CHANGELOG.md")]

//[Optional] The url to a featured logo that will be displayed in the plugin list next to the name
[assembly: AssemblyMetadata("FeaturedImageURL", "")]
//[Optional] A url to an example screenshot of your plugin in action
[assembly: AssemblyMetadata("ScreenshotURL", "")]
//[Optional] An additional url to an example example screenshot of your plugin in action
[assembly: AssemblyMetadata("AltScreenshotURL", "")]
//[Optional] An in-depth description of your plugin
[assembly: AssemblyMetadata("LongDescription", @"!!! Early beta version.  Using during imaging that you care about is not reccomended. !!!

# About

Based on [Green Swamp's Pont3D](https://greenswamp.org/)

This plugin adds a dockable window in the imaging view that shows a simulated model of your telescope.  Please see [the readme](https://github.com/FlyingKiwis/Nina.Point3D/blob/master/README.md) for more info.

# Legal

- This plugin is distributed under the GPL v3 license.
    - [License](https://github.com/FlyingKiwis/Nina.Point3D/blob/master/LICENSE)
- It is primarily a port of Green Swamp Server's Point3D © 2021 Rob Morgan released under GPL v3
    - [Website](https://greenswamp.org/)
    - [License](https://github.com/rmorgan001/GS.Point3d/blob/master/LICENSE)
- Models and images redistributed with permission from Rob Morgan

# Contact

I'm in the [NINA discord](https://discord.gg/rWRbVbw) server as Kiwi🥝

")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
// [Unused]
[assembly: AssemblyConfiguration("")]
// [Unused]
[assembly: AssemblyTrademark("")]
// [Unused]
[assembly: AssemblyCulture("")]