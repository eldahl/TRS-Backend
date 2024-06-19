using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TRS_backend.DBModel;
using TRS_backend.Operational;
using TRS_backend.Services;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Text.RegularExpressions;

namespace TRS_backend.Controllers.Both
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

        /// <summary>
        /// Gets all time slots for a given date.
        /// </summary>
        /// <param name="requestBody">Request parameters</param>
        /// <returns>List of available reservation time slots</returns>
        [AllowAnonymous]
        [HttpPost("GetTimeSlotsForDate")]
        public async Task<ActionResult<List<TblTimeSlots>>> GetTimeSlotsForDate([FromBody] DTOGetTimeSlotsForDateRequest requestBody)
        {
            // Get timeslots from database
            var query = _dbContext.TimeSlots.Select(ts => ts).Where(ts => ts.Date == requestBody.Date);

            // If timeslots are found, return them
            if (await query.AnyAsync())
                return await query.ToListAsync();

            // Load settings from settings file
            var settings = _settingsContext.Settings;

            // Generate time slots for the given date
            List<TblTimeSlots> timeSlots = new List<TblTimeSlots>();
            timeSlots = await _timeSlotService.GenerateTimeSlots(
                requestBody.Date,
                settings.OpenTime,
                settings.CloseTime,
                TimeSpan.Parse(settings.TimeSlotDuration),
                settings.ReservationsPerTimeSlot,
                settings.ServingInterval
            );

            return timeSlots;
        }

        /// <summary>
        /// Get all tables in the reservation system
        /// </summary>
        /// <returns>List of tables</returns>
        [AllowAnonymous]
        [HttpGet("GetTables")]
        public async Task<ActionResult<List<TblTables>>> GetTables()
        {
            return await _dbContext.Tables.ToListAsync();
        }

        /// <summary>
        /// Makes a reservation for a given time slot.
        /// </summary>
        /// <param name="requestBody">Request parameters</param>
        /// <returns>The table reservation table</returns>
        [AllowAnonymous]
        [HttpPost("Reserve")]
        public async Task<ActionResult<TblTableReservations>> Reserve([FromBody] DTOReserveRequest requestBody)
        {
            // Validate requestBody parameters
            if (requestBody.TableId == 0 || requestBody.TableId < 0 || requestBody.TimeSlotId == 0 || requestBody.TimeSlotId < 0)
                return BadRequest("TableId and TimeSlotId must be provided");

            // Regex for FullName: Allow all unicode letters, spaces, and punctuation
            Regex fullNameRegex = new Regex(@"\\pL+[\\pL\\pZ\\pP]{0,}$?");
            if (requestBody.FullName is not null && fullNameRegex.IsMatch(requestBody.FullName))
            {
                return BadRequest("Full Name field is not recognized as a name");
            }
            // Email validator
            if (requestBody.Email is not null) {
                try {
                    var email = new System.Net.Mail.MailAddress(requestBody.Email);
                }
                catch (Exception) {
                    return BadRequest("Email field is not recognized as an email");
                }
            }
            // Phone number validator: Only allow 8 digit phone numbers for now
            Regex phoneRegex = new Regex(@"\\d{8,}$");
            if (requestBody.PhoneNumber is not null && phoneRegex.IsMatch(requestBody.PhoneNumber))
            {
                return BadRequest("Phone number field is not recognized as an 8 digit phone number");
            }


            // Check if table is already reserved
            var tableAvailableQuery = _dbContext.TableReservations
                .Include(tr => tr.Table)
                .Include(tr => tr.TimeSlot)
                .Include(tr => tr.OpenDay)
                .Select(tr => tr)
                .Where(tr =>
                    tr.Table.Id == requestBody.TableId &&
                    tr.TimeSlot.Id == requestBody.TimeSlotId
                );
            if (tableAvailableQuery.Count() >= 2)
                return BadRequest("Table is already reserved");

            // Reserve time slot and return a TblTableReservation
            return await _timeSlotService.ReserveTimeSlot(requestBody);
        }
    }
}
