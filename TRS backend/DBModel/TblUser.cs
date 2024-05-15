using System.ComponentModel.DataAnnotations;

namespace TRS_backend.DBModel
{
    public enum UserRole
    {
        Admin,
        User
    }

    public class TblUser
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Username { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        [MaxLength(1024)]
        public byte[] PasswordHash { get; set; }

        [Required]
        [MaxLength(1024)]
        public byte[] Salt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
