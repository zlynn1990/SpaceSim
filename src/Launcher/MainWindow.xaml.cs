using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SpaceSim.Common;
using SpaceSim.Common.Contracts;
using SpaceSim.Common.Contracts.Commands;
using Path = System.IO.Path;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, MissionProfile> _missionProfiles;

        public MainWindow()
        {
            _missionProfiles = new Dictionary<string, MissionProfile>();

            InitializeComponent();

            var profileLoader = new FlightProfileManager();

            var profiles = profileLoader.GetAllProfiles();

            foreach (string profile in profiles)
            {
                string displayName = Path.GetFileNameWithoutExtension(profile);

                try
                {
                    var missionProfile = new MissionProfile
                    {
                        Config = MissionConfig.Load(profile),
                        Commands = new Dictionary<string, List<Command>>()
                    };

                    string[] commandFiles = Directory.GetFiles(profile);

                    // Load all the craft mission commands
                    foreach (string commandFile in commandFiles)
                    {
                        if (commandFile.IndexOf("MissionConfig.xml", StringComparison.InvariantCultureIgnoreCase) > 0 ||
                            commandFile.IndexOf("Structures.xml", StringComparison.CurrentCultureIgnoreCase) > 0)
                        {
                            continue;
                        }

                        var commmands = CommandReader.Read(commandFile);

                        missionProfile.Commands.Add(Path.GetFileNameWithoutExtension(commandFile), commmands);
                    }

                    _missionProfiles.Add(displayName, missionProfile);

                    ProfileBox.Items.Add(displayName);
                }
                // Swallow exceptions for broken profiles
                catch { }
                
            }
        }

        private void OnSelectProfile(object sender, SelectionChangedEventArgs e)
        {
            if (ProfileBox.SelectedItem == null) return;

            MissionProfile selectProfile = _missionProfiles[(string)ProfileBox.SelectedItem];

            CraftBox.Items.Clear();
            CommandBox.Items.Clear();
            PropertyBox.Items.Clear();

            foreach (string craftName in selectProfile.Commands.Keys)
            {
                CraftBox.Items.Add(craftName);
            }

            DateTime launchDate = selectProfile.Config.GetLaunchDate();

            VehicleName.Content = selectProfile.Config.VehicleType;
            PlanetName.Content = selectProfile.Config.ParentPlanet;
            PayloadName.Content = UnitDisplay.Mass(selectProfile.Config.PayloadMass);
            LocalTimeName.Content = $"{launchDate.ToShortDateString()} {launchDate.ToShortTimeString()}";
        }

        private void OnSelectCraft(object sender, SelectionChangedEventArgs e)
        {
            if (ProfileBox.SelectedItem == null) return;
            if (CraftBox.SelectedItem == null) return;

            MissionProfile selectProfile = _missionProfiles[(string)ProfileBox.SelectedItem];

            List<Command> commands = selectProfile.Commands[(string) CraftBox.SelectedItem];

            CommandBox.Items.Clear();
            PropertyBox.Items.Clear();

            foreach (Command command in commands)
            {
                CommandBox.Items.Add(command);
            }
        }

        private void OnSelectCommand(object sender, SelectionChangedEventArgs e)
        {
            if (CommandBox.SelectedItem == null) return;

            PropertyBox.Items.Clear();
            Command command = CommandBox.SelectedItem as Command;
            if (command != null)
            {
                Type type = command.GetType();
                PropertyInfo[] props = type.GetProperties();
                foreach (PropertyInfo info in props)
                {
                    string name = info.Name;
                    string format = string.Empty;
                    switch (info.PropertyType.Name)
                    {
                        case "Double":
                            format = "{0} {1:F}";
                            break;
                        default:
                            format = "{0} {1}";
                            break;
                    }

                    PropertyBox.Items.Add(string.Format(format, name, info.GetValue(command)));
                }
            }
        }

        private void OnLaunch(object sender, RoutedEventArgs e)
        {
            Close();

            var argumentBuilder = new StringBuilder("-profiles ");

            // Add all the selected profiles
            foreach (object selectedProfile in ProfileBox.SelectedItems)
            {
                argumentBuilder.Append($"\"{selectedProfile}\" ");
            }

            // Run in windowed mode if full screen is not checked
            if (FullScreenBox.IsChecked.HasValue && !FullScreenBox.IsChecked.Value)
            {
                argumentBuilder.Append("-w");
            }

            string spaceSimPath = LocateSpaceSimExecutable();

            var spaceSimProcess = new ProcessStartInfo
            {
                FileName = spaceSimPath,
                Arguments = argumentBuilder.ToString(),
                WorkingDirectory = Path.GetDirectoryName(spaceSimPath),
                UseShellExecute = false
            };

            Process.Start(spaceSimProcess);
        }

        private static string LocateSpaceSimExecutable()
        {
            // Launcher is sitting at the space sim level (real builds)
            if (File.Exists("SpaceSim.exe"))
            {
                return "SpaceSim.exe";
            }

            // Otherwise check for the release build
            if (File.Exists("../../../SpaceSim/bin/Release/SpaceSim.exe"))
            {
                return "../../../SpaceSim/bin/Release/SpaceSim.exe";
            }

            // Finally check debug (not a great option for performance)
            if (File.Exists("../../../SpaceSim/bin/Debug/SpaceSim.exe"))
            {
                return "../../../SpaceSim/bin/Debug/SpaceSim.exe";
            }

            throw new Exception("Could not locate SpaceSim.exe!");
        }
    }
}
