using System.ComponentModel.DataAnnotations;

namespace TRS_backend.DBModel
{
    public class TblTimeSlots
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }
        
        [Required]
        public TimeSpan Duration { get; set; }



    }
}
