using System;
using System.Collections.Generic;
using System.Windows;
using SpaceSim.Common;
using SpaceSim.Properties;

namespace SpaceSim
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SpaceSim.MainWindow.ProfilePaths = new List<string>();

            var flightProfileManager = new FlightProfileManager();

            if (e.Args.Length > 0)
            {
                bool loadProfiles = false;
                SpaceSim.MainWindow.FullScreen = true;

                for (var i = 0; i < e.Args.Length; i++)
                {
                    string argument = e.Args[i];

                    if (argument.Equals("-profiles", StringComparison.InvariantCultureIgnoreCase))
                    {
                        loadProfiles = true;
                    }
                    else if (argument.Equals("-w", StringComparison.InvariantCultureIgnoreCase))
                    {
                        SpaceSim.MainWindow.FullScreen = false;
                        loadProfiles = false;
                    }
                    else if (loadProfiles)
                    {
                        SpaceSim.MainWindow.ProfilePaths.Add(flightProfileManager.BuildFullPath(argument));
                    }
                }
            }
            else
            {
                SpaceSim.MainWindow.FullScreen = Settings.Default.FullScreen;
            }

            if (SpaceSim.MainWindow.ProfilePaths.Count == 0)
            {
                SpaceSim.MainWindow.ProfilePaths = new List<string>
                {
                    //flightProfileManager.BuildFullPath("CRS-9"),
                    //flightProfileManager.BuildFullPath("CRS-11"),
                    //flightProfileManager.BuildFullPath("CRS-12"),
                    //flightProfileManager.BuildFullPath("CRS-14"),
                    //flightProfileManager.BuildFullPath("Formosat-5"),
                    //flightProfileManager.BuildFullPath("Bangabandhu-1"),
                    //flightProfileManager.BuildFullPath("BulgariaSat-1"),
                    //flightProfileManager.BuildFullPath("BulgariaSat-1b"),
                    //flightProfileManager.BuildFullPath("Hwasong-14"),
                    //flightProfileManager.BuildFullPath("NROL-76"),
                    //flightProfileManager.BuildFullPath("SES-10"),
                    //flightProfileManager.BuildFullPath("Iridium NEXT"),
                    //flightProfileManager.BuildFullPath("Iridium-7"),
                    //flightProfileManager.BuildFullPath("Iridium-GRACE-FO"),
                    //flightProfileManager.BuildFullPath("Inmarsat-5"),
                    //flightProfileManager.BuildFullPath("Intelsat-35e"),
                    //flightProfileManager.BuildFullPath("BFR Crew Launch"),
                    //flightProfileManager.BuildFullPath("BFR P2P Launch"),
                    //flightProfileManager.BuildFullPath("BFS to GEO"),
                    //flightProfileManager.BuildFullPath("BFS300 to LEO"),
                    //flightProfileManager.BuildFullPath("BFS250 to LEO"),
                    //flightProfileManager.BuildFullPath("BFR Direct GTO"),
                    //flightProfileManager.BuildFullPath("BFS Earth EDL"),
                    //flightProfileManager.BuildFullPath("ITS Crew Launch"),
                    //flightProfileManager.BuildFullPath("ITS Tanker SSTO"),
                    //flightProfileManager.BuildFullPath("ITS Earth Aerocapture"),
                    //flightProfileManager.BuildFullPath("ITS Earth EDL"),
                    //flightProfileManager.BuildFullPath("ITS Earth Direct"),
                    //flightProfileManager.BuildFullPath("ITS Mars Aerocapture"),
                    //flightProfileManager.BuildFullPath("ITS Mars EDL"),
                    //flightProfileManager.BuildFullPath("ITS Mars Direct"),
                    //flightProfileManager.BuildFullPath("AutoLanding Test"),
                    //flightProfileManager.BuildFullPath("RedDragon Launch"),
                    //flightProfileManager.BuildFullPath("Dragon Abort"),
                    //flightProfileManager.BuildFullPath("Dragon Entry"),
                    flightProfileManager.BuildFullPath("DH-ParkerSolar"),
                    //flightProfileManager.BuildFullPath("F9 SSTO"),
                    //flightProfileManager.BuildFullPath("F9-B5-ASDS"),
                    //flightProfileManager.BuildFullPath("F9-B5-Expendable"),
                    //flightProfileManager.BuildFullPath("FH-ASDS"),
                    //flightProfileManager.BuildFullPath("FH-DEMO"),
                    //flightProfileManager.BuildFullPath("FH-RTLS"),
                    //flightProfileManager.BuildFullPath("FH-Expendable"),
                    flightProfileManager.BuildFullPath("FH-Expendable-ParkerSolar"),
                    //flightProfileManager.BuildFullPath("F9S2 Earth LEO EDL"),
                    //flightProfileManager.BuildFullPath("F9S2 Earth EDL"),
                    //flightProfileManager.BuildFullPath("F9S2 Earth EDL2"),
                    //flightProfileManager.BuildFullPath("Orbcomm-OG2"),
                    //flightProfileManager.BuildFullPath("OTV-5"),
                    //flightProfileManager.BuildFullPath("SES9"),
                    //flightProfileManager.BuildFullPath("Telstar-19"),
                    //flightProfileManager.BuildFullPath("Thaicom-8"),
                    //flightProfileManager.BuildFullPath("Grey Dragon Flyby"),
                    //flightProfileManager.BuildFullPath("Scaled BFR Launch"),
                    //flightProfileManager.BuildFullPath("Scaled BFR GTO"),
                    //flightProfileManager.BuildFullPath("Scaled BFS TLI"),
                    //flightProfileManager.BuildFullPath("Scaled BFS LL"),
                    //flightProfileManager.BuildFullPath("Scaled BFS TEI"),
                    //flightProfileManager.BuildFullPath("Scaled BFS EDL"),
                    //flightProfileManager.BuildFullPath("SLS Satellite Launch"),
                };
            }

            if (SpaceSim.MainWindow.ProfilePaths == null ||
                SpaceSim.MainWindow.ProfilePaths.Count == 0)
            {
                throw new Exception("Must specify at least one mission profile!");
            }
        }
    }
}
