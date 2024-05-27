using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Configuration;
using TRS_backend.API_Models;
using TRS_backend.DBModel;
using TRS_backend.Operational;

namespace TRS_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SettingsController : Controller
    {
        private readonly TRSDbContext _dbContext;
        private readonly SettingsFileContext _settingsContext;

        public SettingsController(TRSDbContext dbContext, SettingsFileContext settingsContext)
        {
            _dbContext = dbContext;
            _settingsContext = settingsContext;
        }

        /// <summary>
        /// Get all settings
        /// </summary>
        /// <returns>Settings object encapsulated in Response object</returns>
        [Authorize]
        [HttpPost("GetSettings")]
        public ActionResult<DTOSettingsResponse> GetSettings()
        {
            return new DTOSettingsResponse()
            {
                Settings = _settingsContext.Settings,
                Tables = _dbContext.Tables.ToArray()
            };
        }

        /// <summary>
        /// Set settings
        /// </summary>
        /// <param name="requestBody">Settings to set</param>
        /// <returns>Newly set settings</returns>
        [Authorize]
        [HttpPost("SetSettings")]
        public async Task<ActionResult<DTOSettingsResponse>> SetSettings([FromBody] DTOSettingsRequest requestBody)
        {
            // Set tables settings in the database
            _dbContext.Tables.UpdateRange(requestBody.Tables);
            await _dbContext.SaveChangesAsync();

            // Set the settings in the settings file
            _settingsContext.Settings = requestBody.Settings;
            return new DTOSettingsResponse()
            {
                Settings = _settingsContext.Settings,
                Tables = _dbContext.Tables.ToArray()
            };
        }
    }
}
