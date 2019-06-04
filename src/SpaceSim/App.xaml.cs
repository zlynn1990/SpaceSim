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
                    //flightProfileManager.BuildFullPath("Bangabandhu-1"),
                    //flightProfileManager.BuildFullPath("BulgariaSat-1"),
                    //flightProfileManager.BuildFullPath("BulgariaSat-1b"),
                    //flightProfileManager.BuildFullPath("CRS-8"),
                    //flightProfileManager.BuildFullPath("CRS-9"),
                    //flightProfileManager.BuildFullPath("CRS-11"),
                    //flightProfileManager.BuildFullPath("CRS-12"),
                    //flightProfileManager.BuildFullPath("CRS-14"),
                    //flightProfileManager.BuildFullPath("CRS-16"),
                    //flightProfileManager.BuildFullPath("CRS-17"),
                    //flightProfileManager.BuildFullPath("DM-1"),
                    //flightProfileManager.BuildFullPath("Electron"),
                    //flightProfileManager.BuildFullPath("Formosat-5"),
                    //flightProfileManager.BuildFullPath("GPS-III"),
                    //flightProfileManager.BuildFullPath("Hwasong-14"),
                    //flightProfileManager.BuildFullPath("IFA"),
                    //flightProfileManager.BuildFullPath("Iridium NEXT"),
                    //flightProfileManager.BuildFullPath("Iridium-8"),
                    //flightProfileManager.BuildFullPath("Iridium-7"),
                    //flightProfileManager.BuildFullPath("Iridium-GRACE-FO"),
                    //flightProfileManager.BuildFullPath("Inmarsat-5"),
                    //flightProfileManager.BuildFullPath("Intelsat-35e"),
                    //flightProfileManager.BuildFullPath("NROL-76"),
                    //flightProfileManager.BuildFullPath("Nusantara-Satu"),
                    //flightProfileManager.BuildFullPath("Saocom-1A"),
                    //flightProfileManager.BuildFullPath("SES-10"),
                    //flightProfileManager.BuildFullPath("Starlink-0.9"),
                    //flightProfileManager.BuildFullPath("BFR Crew Launch"),
                    //flightProfileManager.BuildFullPath("BFR100 Crew Launch"),
                    //flightProfileManager.BuildFullPath("BFR150 Crew Launch"),
                    //flightProfileManager.BuildFullPath("BFR300 Crew Launch"),
                    //flightProfileManager.BuildFullPath("BFR19 Crew Launch"),
                    //flightProfileManager.BuildFullPath("BFR P2P Launch"),
                    //flightProfileManager.BuildFullPath("BFS to GEO"),
                    //flightProfileManager.BuildFullPath("BFS300 to LEO"),
                    //flightProfileManager.BuildFullPath("BFS250 to LEO"),
                    //flightProfileManager.BuildFullPath("BFR Direct GTO"),
                    flightProfileManager.BuildFullPath("BFS Earth EDL"),
                    //flightProfileManager.BuildFullPath("BFS Mars Return EDL"),
                    //flightProfileManager.BuildFullPath("BFS Mars Return Skip Entry"),
                    //flightProfileManager.BuildFullPath("BFS Mars TEI"),
                    //flightProfileManager.BuildFullPath("StarshipASDS"),
                    //flightProfileManager.BuildFullPath("StarshipP2P"),
                    //flightProfileManager.BuildFullPath("StarHopper"),
                    //flightProfileManager.BuildFullPath("StarHopper2"),
                    //flightProfileManager.BuildFullPath("StarHopper3"),
                    //flightProfileManager.BuildFullPath("StarKicker"),
                    //flightProfileManager.BuildFullPath("MiniBFS"),
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
                    //flightProfileManager.BuildFullPath("DH-ParkerSolar"),
                    //flightProfileManager.BuildFullPath("F9 SSTO"),
                    //flightProfileManager.BuildFullPath("F9-B5-ASDS"),
                    //flightProfileManager.BuildFullPath("F9-B5-Expendable"),
                    //flightProfileManager.BuildFullPath("FH-ASDS"),
                    //flightProfileManager.BuildFullPath("FH-DEMO-23"),
                    //flightProfileManager.BuildFullPath("FH-DEMO"),
                    //flightProfileManager.BuildFullPath("FH-Arabsat-6A"),
                    //flightProfileManager.BuildFullPath("FH-RTLS"),
                    //flightProfileManager.BuildFullPath("FH-Expendable"),
                    //flightProfileManager.BuildFullPath("FH-Expendable-Europa"),
                    //flightProfileManager.BuildFullPath("FH-Expendable-Orion"),
                    //flightProfileManager.BuildFullPath("FH-Reusable-Orion"),
                    //flightProfileManager.BuildFullPath("FH-Booster-ICPS"),
                    //flightProfileManager.BuildFullPath("FH-Reusable-ICPS"),
                    //flightProfileManager.BuildFullPath("FH-Expendable-ParkerSolar"),
                    //flightProfileManager.BuildFullPath("F9S2 Earth LEO EDL"),
                    //flightProfileManager.BuildFullPath("F9S2 Earth EDL"),
                    //flightProfileManager.BuildFullPath("F9S2 Earth EDL2"),
                    //flightProfileManager.BuildFullPath("IXPE"),
                    //flightProfileManager.BuildFullPath("Orbcomm-OG2"),
                    //flightProfileManager.BuildFullPath("OrionTLI"),
                    //flightProfileManager.BuildFullPath("OTV-5"),
                    //flightProfileManager.BuildFullPath("SES9"),
                    //flightProfileManager.BuildFullPath("Telstar-19"),
                    //flightProfileManager.BuildFullPath("Thaicom-8"),
                    //flightProfileManager.BuildFullPath("Grey Dragon Flyby"),
                    //flightProfileManager.BuildFullPath("New Glenn"),
                    //flightProfileManager.BuildFullPath("Scaled BFR Launch"),
                    //flightProfileManager.BuildFullPath("Scaled BFR GTO"),
                    //flightProfileManager.BuildFullPath("Scaled BFS TLI"),
                    //flightProfileManager.BuildFullPath("Scaled BFS LL"),
                    //flightProfileManager.BuildFullPath("Scaled BFS TEI"),
                    //flightProfileManager.BuildFullPath("Scaled BFS EDL"),
                    //flightProfileManager.BuildFullPath("SLS Satellite Launch"),
                    //flightProfileManager.BuildFullPath("SLS Orion"),
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
