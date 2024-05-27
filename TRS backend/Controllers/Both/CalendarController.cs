using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TRS_backend.API_Models;
using TRS_backend.API_Models.Admin_Portal;
using TRS_backend.DBModel;
using TRS_backend.Operational;

namespace TRS_backend.Controllers.Both
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarController : Controller
    {
        private readonly TRSDbContext _dbContext;
        private readonly SettingsFileContext _settingsContext;

        public CalendarController(TRSDbContext dbContext, SettingsFileContext settingsContext)
        {
            _dbContext = dbContext;
            _settingsContext = settingsContext;
        }

        /// <summary>
        /// Get open days by month and year
        /// </summary>
        /// <param name="requestBody">Request parameters</param>
        /// <returns>Open days in the given month & year</returns>
        [Authorize]
        [HttpPost("GetOpenDaysByMonthAndYear")]
        public ActionResult<DTOOpenDaysByMonthAndYearResponse> GetOpenDaysByMonthAndYear([FromBody] DTOOpenDaysByMonthAndYearRequest requestBody)
        {

            var openDays = _dbContext.OpenDays.Select(od => od).Where(od => od.Date.Month == requestBody.Month && od.Date.Year == requestBody.Year).ToList();

            return new DTOOpenDaysByMonthAndYearResponse()
            {
                OpenDays = openDays
            };
        }

        /// <summary>
        ///  Set open day for a specific date
        /// </summary>
        /// <param name="requestBody">Request parameters</param>
        /// <returns>Response message</returns>
        [Authorize]
        [HttpPost("SetOpenDayForDate")]
        public async Task<ActionResult<string>> SetOpenDayForDate([FromBody] DTOSetOpenDayForDateRequest requestBody) 
        {
            // Check if OpenDay entry already exists
            if (await _dbContext.OpenDays.Select(od => od).Where(od => od.Date == requestBody.Date).AnyAsync()) {
                // Remove entry if it should be closed
                if (requestBody.IsOpen == false)
                {
                    _dbContext.OpenDays.Remove(await _dbContext.OpenDays.Select(od => od).Where(od => od.Date == requestBody.Date).FirstAsync());
                    return Ok();
                }
                else {
                    return BadRequest("OpenDay date is already open");
                }
            }

            // Insert new open day
            _dbContext.OpenDays.Add(new TblOpenDays() { 
                Date = requestBody.Date, 
                OpenTime = _settingsContext.Settings.OpenTime, 
                CloseTime = _settingsContext.Settings.CloseTime 
            });

            return Ok();
        }
    }
}
