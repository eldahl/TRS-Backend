using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TRS_backend.Operational
{
    /// <summary>
    /// Creates an in-memory key value store for settings entries. Additionally, redudancy is provided to store settings between restarts of the application, by saving to file.
    /// </summary>
    public class SettingsFileContext
    {
        // File path and name of the settings file
        private string _settingsFilePath = "";
        private string _settingsFileName = "";

        // In-memory dictionary to store settings
        private Dictionary<string, string> _settings = new Dictionary<string, string>();

        private readonly IConfiguration _configuration;

        public SettingsFileContext(IConfiguration configuration)
        {
            // Access to configuration through dependency injection
            _configuration = configuration;

            // Get file path and name from configuration
            _settingsFileName = _configuration["SettingsFileName"]!;
            _settingsFilePath = _configuration["SettingsFilePath"]!;

            // Create settings file if it does not exist
            if (!File.Exists(_settingsFileName)) {
                File.Create(_settingsFileName);
            }
            else {
                // Otherwise load settings from file
                LoadSettingsFromFile();
            }
        }

        /// <summary>
        /// Reads the settings from file and loads them into the in-memory settings dictionary
        /// </summary>
        private void LoadSettingsFromFile()
        {
            // Read contents of settings file
            string settingsFileString = File.ReadAllText(_settingsFilePath + _settingsFileName);
            string[] settingEntries = settingsFileString.Split("\n");
            
            // Create a temporary dictionary to store the settings
            Dictionary<string, string> temp = new Dictionary<string, string>();
            foreach (string settingsEntry in settingEntries) {
                // Get key and value from settings entry
                string key = settingsEntry.Split("=")[0];
                string value = settingsEntry.Split("=")[1];

                // Check if key or value is null and if so throw an exception
                if (key == null || value == null) {
                    throw new ArgumentNullException("Settings file contains invalid entries.");
                }
                temp.Add(key, value);
            }

            _settings = temp;
        }

        /// <summary>
        /// Gets all settings from the in-memory settings dictionary using enumeration
        /// </summary>
        /// <returns>All settings in the in-memory dictionary</returns>
        public IEnumerable<KeyValuePair<string, string>> GetAllSettings()
        {
            return _settings;
        }

        /// <summary>
        /// Gets a settings entry from the in-memory settings dictionary
        /// </summary>
        /// <param name="key">Key of the settings entry</param>
        /// <returns>Value of the given key, or null</returns>
        public string GetSettingsEntry(string key)
        {
            string value = _settings[key];
            return value != null ? value : null!;
        }

        /// <summary>
        /// Adds a settings entry to the in-memory settings dictionary
        /// </summary>
        /// <param name="key">Key of the settings entry</param>
        /// <param name="value">Value of the settings entry</param>
        public void AddSettingsEntry(string key, string value) 
        {
            // Sanitization by only allowing lower- and uppercase letters, numbers, and hyphens
            Regex regex = new Regex("[^a-zA-Z0-9-]");
            key = regex.Replace(key, "");
            value = regex.Replace(value, "");

            _settings.Add(key, value);
            WriteSettingsToFile();
        }

        /// <summary>
        /// Remove a settings entry from the in-memory settings dictionary
        /// </summary>
        /// <param name="key">The key to remove</param>
        public void RemoveSettingsEntry(string key)
        {
            _settings.Remove(key);
            WriteSettingsToFile();
        }

        /// <summary>
        /// Writes the settings to the settings file
        /// </summary>
        /// <returns>true: success | false: failure</returns>
        private void WriteSettingsToFile()
        {
            // Aggregate all settings entries
            string settingsFileString = "";
            foreach (var setting in _settings)
            {
                settingsFileString += FormatSettingsEntry(setting.Key, setting.Value);
            }

            // Write settings to file
            try {
                File.WriteAllText(_settingsFilePath + _settingsFileName, settingsFileString);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        /// <summary>
        /// Formats a settings entry to a single string
        /// </summary>
        /// <param name="key">The key of the settings entry</param>
        /// <param name="value">The value of the settings entry</param>
        /// <returns>The formatted string containing the key and value of the settings entry</returns>
        private string FormatSettingsEntry(string key, string value)
        {
            return $"{key}={value}";
        }
    }
}
