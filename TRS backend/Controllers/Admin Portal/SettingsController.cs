using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using TRS_backend.DBModel;
using TRS_backend.Operational;

namespace TRS_backend.Controllers
{
    public class SettingsController : Controller
    {
        private readonly SettingsFileContext _settingsContext;

        public SettingsController(SettingsFileContext settingsContext)
        {
            _settingsContext = settingsContext;
        }

        [HttpPost("GetSettings")]
        public ActionResult<DTOGetSettingsResponse> GetSettings()
        {
            // Get settings from settings file
            var settings = _settingsContext.GetAllSettings();

            // Return settings as list of key-value pairs
            return new DTOGetSettingsResponse()
            {
                Settings = settings.ToList()
            };
        }

        [HttpPost("SetSettings")]
        public ActionResult<DTOSetSettingsResponse> SetSettings([FromBody] DTOSetSettingsRequest requestBody)
        {
            // Validate the request and sanitize the input

            // Get settings from database
            var settings = _settingsContext.GetAllSettings();

            // Apply the new settings to database
            /*
            foreach (var setting in requestBody.Settings)
            {
                settings[setting.Key] = setting.Value;
            }
            */

            // Save new settings

            // Return the new settings
            return new DTOSetSettingsResponse()
            {
                Settings = null//settings
            };
        }
    }
}
