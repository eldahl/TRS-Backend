using System.Security.Cryptography;
using System.Text;

namespace TRS_backend
{
    public class Crypto
    {
        public static byte[] GenerateRandomBytes(int numOfBytes)
        {
            byte[] salt = new byte[numOfBytes];
            RandomNumberGenerator.Create().GetBytes(salt);
            return salt;
        }

        public static byte[] HashPassword(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] passwordWithSalt = new byte[passwordBytes.Length + salt.Length];
                passwordBytes.CopyTo(passwordWithSalt, 0);
                salt.CopyTo(passwordWithSalt, passwordBytes.Length);
                return sha256.ComputeHash(passwordWithSalt);
            }
        }
    }
}
