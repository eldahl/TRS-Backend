using System.ComponentModel.DataAnnotations;
using TRS_backend.Models;

namespace TRS_backend.DBModel
{
    public class TblUsers
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = "";

        [Required]
        [MaxLength(255)]
        public string Username { get; set; } = "";

        [Required]
        public UserRole Role { get; set; }

        [Required]
        [MaxLength(1024)]
        public byte[] PasswordHash { get; set; } = new byte[0];

        [Required]
        [MaxLength(1024)]
        public byte[] Salt { get; set; } = new byte[0];

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
