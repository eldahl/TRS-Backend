using TRS_backend.Operational;

namespace TRS_backend_test.OperationalTests
{
    public class CryptoTests
    {
        [Fact]
        public void GenerateRandomBytes_GeneratesCorrectLength()
        {
            // Arrange
            var length = 16;

            // Act
            var bytes = Crypto.GenerateRandomBytes(length);

            // Assert
            Assert.NotNull(bytes);
            Assert.Equal(length, bytes.Length);
        }

        [Fact]
        public void GenerateRandomBytes_GeneratesDifferentBytes()
        {
            // Arrange
            var length = 16;

            // Act
            var bytes1 = Crypto.GenerateRandomBytes(length);
            var bytes2 = Crypto.GenerateRandomBytes(length);

            // Assert
            Assert.NotEqual(bytes1, bytes2);
        }

        [Fact]
        public void HashPassword_GeneratesCorrectHashLength()
        {
            // Arrange
            var password = "password";
            var salt = Crypto.GenerateRandomBytes(16);

            // Act
            var hash = Crypto.HashPassword(password, salt);

            // Assert
            Assert.NotNull(hash);
            Assert.Equal(32, hash.Length);
        }

        [Fact]
        public void HashPassword_GeneratesDifferentHashesForDifferentSalts()
        {
            // Arrange
            var password = "password";
            var salt1 = Crypto.GenerateRandomBytes(16);
            var salt2 = Crypto.GenerateRandomBytes(16);

            // Act
            var hash1 = Crypto.HashPassword(password, salt1);
            var hash2 = Crypto.HashPassword(password, salt2);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void HashPassword_GeneratesDifferentHashesForDifferentPasswords()
        {
            // Arrange
            var password1 = "password1";
            var password2 = "password2";
            var salt = Crypto.GenerateRandomBytes(16);

            // Act
            var hash1 = Crypto.HashPassword(password1, salt);
            var hash2 = Crypto.HashPassword(password2, salt);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void HashPassword_GeneratesSameHashForSamePasswordAndSalt()
        {
            // Arrange
            var password = "password";
            var salt = Crypto.GenerateRandomBytes(16);

            // Act
            var hash1 = Crypto.HashPassword(password, salt);
            var hash2 = Crypto.HashPassword(password, salt);

            // Assert
            Assert.Equal(hash1, hash2);
        }
    }
}
