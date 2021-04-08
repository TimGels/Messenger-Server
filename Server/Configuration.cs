using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Messenger_Server
{
    /// <summary>
    /// This class is used to read and get settings from the settings file.
    /// In this way it's possible to configure the server by some settings in the file.
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// The settings file is always stored in the same folder as the executale with the name: "settings".
        /// </summary>
        private static readonly string settingsFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings");

        /// <summary>
        /// Read all settings from the file and save them in this dictionary.
        /// </summary>
        private static readonly Dictionary<string, string> Settings = ReadSettings();

        /// <summary>
        /// Get specific setting from the dictionary
        /// </summary>
        /// <param name="key">The setting to get.</param>
        /// <returns>The value of the setting or null when the setting wasn't found.</returns>
        public static string GetSetting(string key)
        {
            if(key is null)
            {
                return null;
            }
            Settings.TryGetValue(key, out string value);
            return value;
        }

        /// <summary>
        /// This method is called on initialization of this class.
        /// It will read the file and add the settings read to the Dictionary.
        /// </summary>
        /// <returns>The dictionary with the read settings.</returns>
        private static Dictionary<string, string> ReadSettings()
        {
            try
            {
                // Initialize the dictionary.
                Dictionary<string, string> settings = new Dictionary<string, string>();

                using (StreamReader fileReader = new StreamReader(settingsFilePath))
                {
                    string line = "";
                    // Read every line of the file.
                    while ((line = fileReader.ReadLine()) != null)
                    {
                        // If the line starts with // it's supposed to be a comment.
                        if (line.StartsWith("//")){
                            continue;
                        }
                        // Convention: everything before ":" is the key of the setting.
                        string[] setting = line.Split(":");
                        // If there are at least 2 strings, add them to the dictionary.
                        if (setting.Length > 1)
                        {
                            settings.Add(setting[0], setting[1]);
                        }
                    }
                }

                return settings;
            }
            // If the file wasn't found, create it and add all the possible settings as comments.
            catch (FileNotFoundException)
            {
                Console.WriteLine("Settings file not found. Using default fallbacks!");
                using (StreamWriter writer = new StreamWriter(File.Create(settingsFilePath)))
                {
                    writer.WriteLine("//databaseCacheShared:true");
                    writer.WriteLine("//walEnabled:true");
                    writer.WriteLine("//port:5000");
                    writer.WriteLine("//plinq:true");

                    writer.Flush();
                }
                return new Dictionary<string, string>();
            }
        }
    }
}
