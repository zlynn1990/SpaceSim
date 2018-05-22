using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace SpaceSim
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            string profileDirectory = DetectProfileDirectory();

            //SpaceSim.MainWindow.FullScreen = false;

            SpaceSim.MainWindow.ProfilePaths = new List<string>
            {
                //Path.Combine(profileDirectory, "CRS-9"),
                //Path.Combine(profileDirectory, "CRS-11"),
                //Path.Combine(profileDirectory, "CRS-12"),
                //Path.Combine(profileDirectory, "CRS-14"),
                //Path.Combine(profileDirectory, "Formosat-5"),
                
                Path.Combine(profileDirectory, "Bangabandhu-1"),
                //Path.Combine(profileDirectory, "BulgariaSat-1"),

                //Path.Combine(profileDirectory, "BulgariaSat-1b"),
                //Path.Combine(profileDirectory, "Hwasong-14"),
                //Path.Combine(profileDirectory, "NROL-76"),
                //Path.Combine(profileDirectory, "SES-10"),
                //Path.Combine(profileDirectory, "Iridium NEXT"),
                //Path.Combine(profileDirectory, "Inmarsat-5"),
                //Path.Combine(profileDirectory, "Intelsat-35e"),
                //Path.Combine(profileDirectory, "BFR Crew Launch"),
                //Path.Combine(profileDirectory, "BFS to GEO"),
                //Path.Combine(profileDirectory, "BFS300 to LEO"),
                //Path.Combine(profileDirectory, "BFS250 to LEO"),
                //Path.Combine(profileDirectory, "BFR Direct GTO"),
                //Path.Combine(profileDirectory, "BFS Earth EDL"),
                //Path.Combine(profileDirectory, "ITS Crew Launch"),
                //Path.Combine(profileDirectory, "ITS Tanker SSTO"),
                //Path.Combine(profileDirectory, "ITS Earth Aerocapture"),
                //Path.Combine(profileDirectory, "ITS Earth EDL"),
                //Path.Combine(profileDirectory, "ITS Earth Direct"),
                //Path.Combine(profileDirectory, "ITS Mars Aerocapture"),
                //Path.Combine(profileDirectory, "ITS Mars EDL"),
                //Path.Combine(profileDirectory, "ITS Mars Direct"),
                //Path.Combine(profileDirectory, "AutoLanding Test"),
                //Path.Combine(profileDirectory, "RedDragon Launch"),
                //Path.Combine(profileDirectory, "Dragon Abort"),
                //Path.Combine(profileDirectory, "Dragon Entry"),
                //Path.Combine(profileDirectory, "F9 SSTO"),
                //Path.Combine(profileDirectory, "F9-B5-ASDS"),
                //Path.Combine(profileDirectory, "F9-B5-Expendable"),
                //Path.Combine(profileDirectory, "FH-ASDS"),
                //Path.Combine(profileDirectory, "FH-DEMO"),
                //Path.Combine(profileDirectory, "FH-Europa-Clipper"),
                //Path.Combine(profileDirectory, "FH-Europa-Clipper-TMI"),
                //Path.Combine(profileDirectory, "FH-RTLS"),
                //Path.Combine(profileDirectory, "FH-Expendable"),
                //Path.Combine(profileDirectory, "F9S2 Earth LEO EDL"),
                //Path.Combine(profileDirectory, "F9S2 Earth EDL"),
                //Path.Combine(profileDirectory, "F9S2 Earth EDL2"),
                //Path.Combine(profileDirectory, "Orbcomm-OG2"),
                //Path.Combine(profileDirectory, "OTV-5"),
                //Path.Combine(profileDirectory, "SES9"),
                //Path.Combine(profileDirectory, "Thaicom-8"),
                //Path.Combine(profileDirectory, "Grey Dragon Flyby"),
                //Path.Combine(profileDirectory, "Scaled BFR Launch"),
                //Path.Combine(profileDirectory, "Scaled BFR GTO"),
                //Path.Combine(profileDirectory, "Scaled BFS TLI"),
                //Path.Combine(profileDirectory, "Scaled BFS LL"),
                //Path.Combine(profileDirectory, "Scaled BFS TEI"),
                //Path.Combine(profileDirectory, "Scaled BFS EDL"),
            };

            if (SpaceSim.MainWindow.ProfilePaths == null ||
                SpaceSim.MainWindow.ProfilePaths.Count == 0)
            {
                throw new Exception("Must specify at least one mission profile!");
            }
        }

        private string DetectProfileDirectory()
        {
            if (Directory.Exists("flight profiles"))
            {
                return "flight profiles";
            }

            if (Directory.Exists("../../../../flight profiles"))
            {
                return "../../../../flight profiles";
            }

            throw new Exception("Profile directory was not detected!");
        }
    }
}
