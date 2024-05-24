using System.Security.Cryptography;
using System.Text;

namespace TRS_backend.Operational
{
    /// <summary>
    /// Holds all cryptographics operations used within the systems
    /// </summary>
    public class Crypto
    {
        /// <summary>
        /// Generates a random byte array of a given length
        /// </summary>
        /// <param name="numOfBytes">Number of bytes to generate</param>
        /// <returns>Array of random data</returns>
        public static byte[] GenerateRandomBytes(int numOfBytes)
        {
            byte[] salt = new byte[numOfBytes];
            RandomNumberGenerator.Create().GetBytes(salt);
            return salt;
        }

        /// <summary>
        /// Hashes a password with a given salt using SHA256
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <param name="salt">The salt to use when hashing the password</param>
        /// <returns>SHA256 encrypted byte array of the password + salt</returns>
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
