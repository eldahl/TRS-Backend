using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TRS_backend.DBModel;
using TRS_backend.Operational;
using TRS_backend.Services;
using System.Diagnostics;

namespace TRS_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationController : Controller
    {
        private readonly TRSDbContext _dbContext;
        private readonly TimeSlotService _timeSlotService;
        private readonly SettingsFileContext _settingsContext;

        public ReservationController(TRSDbContext dbContext, TimeSlotService timeSlotService, SettingsFileContext settingsContext)
        {
            _dbContext = dbContext;
            _timeSlotService = timeSlotService;
            _settingsContext = settingsContext;
        }

        [AllowAnonymous]
        [HttpPost("GetTimeSlotsForDate")]
        public async Task<ActionResult<List<TblTimeSlots>>> GetTimeSlotsForDate([FromBody] DTOGetTimeSlotsForDateRequest requestBody)
        {
            List<TblTimeSlots> timeSlots = new List<TblTimeSlots>();

            //timeSlots = _dbContext.TimeSlots.Select(ts => ts).Where(ts => ts.Date == requestBody.Date).ToList();

            if (timeSlots.Count() != 0) {
                return timeSlots;
            }
            else {
                // Load settings from settings file
                var settings = _settingsContext.GetAllSettings().ToDictionary();

                // Generate time slots for the given date
                timeSlots = _timeSlotService.GenerateTimeSlots(
                    requestBody.Date,
                    TimeOnly.Parse(settings["StartTime"]),
                    TimeOnly.Parse(settings["CloseTime"]),
                    TimeSpan.Parse(settings["DiningDuration"]),
                    Int32.Parse(settings["ServingsPerTimeSlot"]),
                    TimeOnly.Parse(settings["ServingInterval"])
                );

                // Save into database
                await _dbContext.TimeSlots.AddRangeAsync(timeSlots);
                await _dbContext.SaveChangesAsync();

                return timeSlots;
            }
        }

        [AllowAnonymous]
        [HttpPost("Reserve")]
        public Task<ActionResult<>> Reserve([FromBody] DTOReserveRequest requestBody) 
        {
        
        }
    }
}
