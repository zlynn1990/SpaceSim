using System;
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

            SpaceSim.MainWindow.FullScreen = true;

            // TODO this is bad hack because im too lazy to use env variables right now
            SpaceSim.MainWindow.ProfileDirectory = Path.Combine(profileDirectory, "CRS-8");
            //SpaceSim.MainWindow.ProfileDirectory = Path.Combine(profileDirectory, "SES9");

            // Parse arguments
            for (int i = 0; i < e.Args.Length; i++)
            {
                if (e.Args[i].Equals("-w", StringComparison.InvariantCultureIgnoreCase))
                {
                    SpaceSim.MainWindow.FullScreen = false;
                }
                else if (Directory.Exists(Path.Combine(profileDirectory, e.Args[i])))
                {
                    SpaceSim.MainWindow.ProfileDirectory = Path.Combine(profileDirectory, e.Args[i]);
                }
            }
            
            // If no profile directory was specified find a default locally or further up the folder structure
            if (string.IsNullOrEmpty(SpaceSim.MainWindow.ProfileDirectory))
            {
                string[] profileDirectories = Directory.GetDirectories(profileDirectory);

                if (profileDirectories.Length == 0)
                {
                    throw new Exception("No flight profiles cound be found!");
                }

                // Default
                SpaceSim.MainWindow.ProfileDirectory = profileDirectories[0];
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
