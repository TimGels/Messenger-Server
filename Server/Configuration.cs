using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Messenger_Server
{
    public static class Configuration
    {
        private static readonly string settingsFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings");
        private static readonly Dictionary<string, string> Settings = ReadSettings();

        public static string GetSetting(string key)
        {
            string value;
            if (!Settings.TryGetValue(key, out value))
            {
                return null;
            }
            else
            {
                return value;
            }
        }

        private static Dictionary<string, string> ReadSettings()
        {
            try
            {
                Dictionary<string, string> settings = new Dictionary<string, string>();
                using (StreamReader fileReader = new StreamReader(settingsFilePath))
                {
                    string line = "";
                    while ((line = fileReader.ReadLine()) != null)
                    {
                        if (line.StartsWith("//")){
                            continue;
                        }
                        string[] setting = line.Split(":");
                        if (setting.Length > 1)
                        {
                            settings.Add(setting[0], setting[1]);
                        }
                    }
                }

                return settings;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Settings file not found. Using default fallbacks!");
                using (StreamWriter writer = new StreamWriter(File.Create(settingsFilePath)))
                {
                    writer.WriteLine("//databaseCacheShared:true");
                    writer.WriteLine("//walEnabled:true");
                    writer.WriteLine("//port:5000");

                    writer.Flush();
                }
                return new Dictionary<string, string>();
            }
        }
    }
}
