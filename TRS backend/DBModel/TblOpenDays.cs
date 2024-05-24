using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace TRS_backend.DBModel
{
    [Index(nameof(Day), IsUnique = true)]
    public class TblOpenDays
    {
        [Key]
        [Required]
        public int Id { get; set; }

        // Date is unique as to not have multiple entries for the same day, see index class attribute
        [Required]
        public DateOnly Day { get; set; }
        
        [Required]
        public TimeOnly OpenTime { get; set; }

        [Required] 
        public TimeOnly CloseTime { get; set; }
    }
}
