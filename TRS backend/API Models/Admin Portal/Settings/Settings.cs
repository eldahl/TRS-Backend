using Microsoft.EntityFrameworkCore.Metadata;
using TRS_backend.API_Models.Admin_Portal.Settings;
using TRS_backend.DBModel;

namespace TRS_backend.API_Models
{
    // Note: Table settings are stored in the database
    public class Settings
    {
        // Reservation settings
        // ASP.NET does not like converting between JSON and TimeSpan
        public string TimeSlotDuration {
            get {
                return timeSlotDuration.ToString();
            }
            set {
                timeSlotDuration = TimeSpan.Parse(value);
            }
        }
        private TimeSpan timeSlotDuration;
        
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }

        public TimeOnly ServingInterval { get; set; }
        public int ReservationsPerTimeSlot { get; set; }

        // Reservation staff notification settings
        public ReservationNotificationEmail[] ReservationNotificationEmails { get; set; } = [];
        public ReservationNotificationPhoneNumber[] ReservationNotificationPhoneNumbers { get; set; } = [];

        // Customer reminder notification templates
        public string CustomerReminderSMSTemplate { get; set; } = "";
        public string CustomerReminderEmailTemplate { get; set; } = "";
    }
}
