using System;
using System.IO;
using System.Xml.Serialization;

namespace CityscapeShuffle
{
    /// <summary>
    /// Persists mod configuration settings to XML file in the game's settings directory.
    /// Currently stores only the debug logging toggle, but can be extended with additional settings.
    /// Settings are saved to CityscapeShuffle.xml and automatically loaded when the mod initializes.
    /// </summary>
    public class ModSettings
    {
        // Constructor sets default values for all settings
        public ModSettings()
        {
            EnableDebugLogging = false;
        }

        // Saves settings to XML file
        public static bool Serialize(string filename, ModSettings config)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filename))
                {
                    xmlSerializer.Serialize(streamWriter, config);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        // Loads settings from XML file (returns null if file doesn't exist)
        public static ModSettings Deserialize(string filename)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
            try
            {
                using (StreamReader streamReader = new StreamReader(filename))
                {
                    return (ModSettings)xmlSerializer.Deserialize(streamReader);
                }
            }
            catch
            {
            }
            return null;
        }

        // User-configurable setting: enables detailed logging to output_log.txt
        public bool EnableDebugLogging;
    }
}