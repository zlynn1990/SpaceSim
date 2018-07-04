using System;
using System.Collections.Generic;
using System.IO;

namespace SpaceSim.Common
{
    public class FlightProfileManager
    {
        private readonly string _profileDirectory;

        public FlightProfileManager()
        {
            _profileDirectory = LocateProfileDirectory();
        }

        public string BuildFullPath(string profileName)
        {
            return Path.Combine(_profileDirectory, profileName);
        }

        public IEnumerable<string> GetAllProfiles()
        {
            return Directory.GetDirectories(_profileDirectory);
        }

        private static string LocateProfileDirectory()
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
