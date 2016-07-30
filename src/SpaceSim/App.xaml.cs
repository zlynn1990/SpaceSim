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

            SpaceSim.MainWindow.FullScreen = false;

            SpaceSim.MainWindow.ProfileDirectories = new List<string>
            {
                Path.Combine(profileDirectory, "CRS-9"),
            };
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
