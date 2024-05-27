using System.ComponentModel.DataAnnotations;

namespace TRS_backend.DBModel
{
    public class TblTables
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string TableName { get; set; } = "";

        [Required]
        public int Seats { get; set; }
    }
}
