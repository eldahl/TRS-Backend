using TRS_backend.DBModel;

namespace TRS_backend.API_Models
{
    public class SetSettingsModel
    {
        // Reservation settings
        public TimeSpan TimeSlotDuration { get; set; }
        
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }

        public TimeOnly ServingInterval { get; set; }

        // Table settings
        public TableSeats[] TableSeats { get; set; }



    }
}
