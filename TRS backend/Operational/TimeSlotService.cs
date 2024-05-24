using TRS_backend.DBModel;
using System.Diagnostics;

namespace TRS_backend.Services
{
    /// <summary>
    /// This class is responsible for managing time slots available at any given time.
    /// </summary>
    public class TimeSlotService
    {
        
        public TimeSlotService()
        {

        }

        public List<TblTimeSlots> GenerateTimeSlots(DateOnly date, TimeOnly startTime, TimeOnly endTime, TimeSpan diningDuration, int servingsPerTimeSlot, TimeOnly servingInterval)
        {
            // Duration of opening hours
            TimeSpan openingHoursDuration = endTime - startTime;
            
            int numOfServingintervals = ((int)openingHoursDuration.TotalMinutes / (int)servingInterval.ToTimeSpan().TotalMinutes);

            // If the closing time falls upon a multiple of the serving interval, we add one so we take orders all the way up until closing
            if ((int)openingHoursDuration.TotalMinutes % (int)servingInterval.ToTimeSpan().TotalMinutes == 0) {
                numOfServingintervals += 1;
            }

            Debug.WriteLine("Intervals: " + numOfServingintervals);

            // Calculate the number of reservation time slots.
            List<TblTimeSlots> timeSlots = new();
            for (int i = 0; i < numOfServingintervals; i++) {
                var timeslot = startTime.AddMinutes(i * servingInterval.ToTimeSpan().TotalMinutes);
                Debug.WriteLine("Time slot: " + timeslot);

                // Create a new time slot
                timeSlots.Add(new()
                {
                    Date = date,
                    StartTime = timeslot,   
                    Duration = diningDuration
                });
            }

            return timeSlots;
        }
    }
}
