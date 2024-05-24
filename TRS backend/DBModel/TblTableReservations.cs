using System.ComponentModel.DataAnnotations;

namespace TRS_backend.DBModel
{
    public class TblTableReservations
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public TblTables Table { get; set; }
        
        [Required]
        public string FullName { get; set; } = "";
        
        [Required]
        public string Email { get; set; } = "";
        
        [Required]
        public string PhoneNumber { get; set; } = "";
        
        [Required]
        public TblOpenDays Day { get; set; } = new ();
        
        [Required]
        public TblTimeSlots TimeSlot { get; set; } = new ();
        
        [Required]
        public bool SendReminders { get; set; }
        
        public string? Comment { get; set; }


    }
}
