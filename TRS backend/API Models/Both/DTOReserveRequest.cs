namespace TRS_backend.Controllers
{
    public class DTOReserveRequest
    {
        // Table and time slot details
        public int TableId { get; set; }
        public int TimeSlotId { get; set; }

        // Reservation details
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public bool SendReminders { get; set; }
        public string? Comment { get; set; }
    }
}