using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using TRS_backend.API_Models;

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

        // Settings object that writes to file when set
        public Settings Settings
        {
            get { 
                return _settings; 
            }
            set {
                _settings = value;
                WriteSettingsToFile();
            }
        }
        private Settings _settings = new Settings();

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
                var stream = File.Create(_settingsFileName);
                stream.Close();
            }
            else {
                // Otherwise load settings from file
                LoadSettingsFromFile();
            }
        }

        /// <summary>
        /// Reads the settings from file and deserializes them into the settings object
        /// </summary>
        private void LoadSettingsFromFile()
        {
            try {
                // Read contents of settings file
                string settingsFileString = File.ReadAllText(_settingsFilePath + _settingsFileName);
                
                // Deserialize settings file into SetSettingsModel object
                if (settingsFileString is not null) {
                    _settings = JsonSerializer.Deserialize<Settings>(settingsFileString)!;
                }
                else {
                    Debug.WriteLine($"Failed to read {_settingsFilePath}{_settingsFileName}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        
        /// <summary>
        /// Writes the settings to the settings file
        /// </summary>
        /// <returns>true: success | false: failure</returns>
        private void WriteSettingsToFile()
        {
            // Aggregate all settings entries
            string settingsFileString = JsonSerializer.Serialize(_settings);

            // Write settings to file
            try {
                File.WriteAllText(_settingsFilePath + _settingsFileName, settingsFileString);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
