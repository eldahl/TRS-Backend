using TRS_backend.DBModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TRS_backend.Controllers;

namespace TRS_backend.Services
{
    /// <summary>
    /// This class is responsible for managing time slots available at any given time.
    /// </summary>
    public class TimeSlotService
    {
        private readonly TRSDbContext _dbContext;

        public TimeSlotService(TRSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Generates time slots for a given date. If time slots already exist for the given date, they are returned.
        /// </summary>
        /// <param name="date">Date of reservation time slots</param>
        /// <param name="startTime">The start of opening hours</param>
        /// <param name="endTime">The end of opening hours</param>
        /// <param name="diningDuration">The duration of a dining reservation</param>
        /// <param name="servingsPerTimeSlot">Amount of servings per time slot</param>
        /// <param name="servingInterval">Interval between possible reservation times</param>
        /// <returns>List of TblTimeSlots that was generated, or preexisting in the database</returns>
        public async Task<List<TblTimeSlots>> GenerateTimeSlots(DateOnly date, TimeOnly startTime, TimeOnly endTime, TimeSpan diningDuration, int servingsPerTimeSlot, TimeOnly servingInterval)
        {
            // Duration of opening hours
            TimeSpan openingHoursDuration = endTime - startTime;
            
            int numOfServingintervals = ((int)openingHoursDuration.TotalMinutes / (int)servingInterval.ToTimeSpan().TotalMinutes);

            // If the closing time falls upon a multiple of the serving interval, we add one so we take orders all the way up until closing
            if ((int)openingHoursDuration.TotalMinutes % (int)servingInterval.ToTimeSpan().TotalMinutes == 0) {
                numOfServingintervals += 1;
            }

            //Debug.WriteLine("Intervals: " + numOfServingintervals);

            // Calculate the number of reservation time slots.
            List<TblTimeSlots> timeSlots = new();
            for (int i = 0; i < numOfServingintervals; i++) {
                var timeslot = startTime.AddMinutes(i * servingInterval.ToTimeSpan().TotalMinutes);
                //Debug.WriteLine("Time slot: " + timeslot);

                // Create a new time slot
                timeSlots.Add(new()
                {
                    Date = date,
                    StartTime = timeslot,   
                    Duration = diningDuration
                });
            }

            // Save into database
            await _dbContext.TimeSlots.AddRangeAsync(timeSlots);
            await _dbContext.SaveChangesAsync();

            return timeSlots;
        }

        /// <summary>
        /// Reserves a time slot given a request body. Does not do any validation.
        /// </summary>
        /// <param name="requestBody">Request parameters from HTTP request</param>
        /// <returns>The newly created TblTableReservations object</returns>
        public async Task<ActionResult<TblTableReservations>> ReserveTimeSlot(DTOReserveRequest requestBody) 
        {
            // Fetch forgein key tables
            var table = await _dbContext.Tables.Select(t => t).Where(t => t.Id == requestBody.TableId).FirstAsync();
            var timeSlot = await _dbContext.TimeSlots.Select(ts => ts).Where(ts => ts.Id == requestBody.TimeSlotId).FirstAsync();
            var openDay = await _dbContext.OpenDays.Select(od => od).Where(od => od.Date == timeSlot.Date).FirstAsync();            

            // Create a new reservation
            var reservation = new TblTableReservations()
            {
                Table = table,
                TimeSlot = timeSlot,
                OpenDay = openDay,
                FullName = requestBody.FullName,
                Email = requestBody.Email,
                PhoneNumber = requestBody.PhoneNumber,
                SendReminders = requestBody.SendReminders,
                Comment = requestBody.Comment
            };

            // Save reservation to database
            await _dbContext.TableReservations.AddAsync(reservation);
            await _dbContext.SaveChangesAsync();
            
            return new TblTableReservations();
        }
    }
}
