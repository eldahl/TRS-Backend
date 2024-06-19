using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TRS_backend;
using TRS_backend.API_Models;
using TRS_backend.Controllers;
using TRS_backend.DBModel;
using TRS_backend.Models;

namespace TRS_backend_test.ControllerTests.AdminPortal
{
    public class AdminPortalControllerTests
    {
        Mock<TRSDbContext> _mockDbContext;
        Mock<IConfiguration> _mockConfiguration;

        private string _jwtSigningKey = "MySuperSecureTestJWTSigningKey123";
        private string _jwtIssuer = "TestIssuer";
        private string _jwtAudience = "TestAudience";       
        private string _registrationCode = "999-REGISTERNOW";

        private string _testUserPassword = "MySecretPassword123";
        private TblUsers _testUser = new TblUsers
        {
            Id = 1,
            Email = "testAdmin@test.dk",
            Username = "testAdmin",
            Role = UserRole.Admin,
            CreatedAt = DateTime.Now,
            // Password is MySecretPassword123
            PasswordHash = new byte[] {
                        0x84, 0x7F, 0x92, 0xCB, 0xE8, 0x6D, 0xA8, 0x86, 0x5F, 0x43, 0x4C,
                        0xC4, 0x50, 0x48, 0xF9, 0xF2, 0xF8, 0x03, 0x02, 0x43, 0xAD, 0xB4,
                        0x0D, 0x4A, 0x5F, 0x1F, 0x75, 0xAC, 0xC4, 0xC8, 0x56, 0x02
                    },
            // Salt is random bytes
            Salt = new byte[] {
                        0xE3, 0x09, 0x10, 0x01, 0x8F, 0x3B, 0x2E, 0x5A, 0x7F, 0xE2, 0x48,
                        0x3D, 0xB2, 0xDB, 0xF1, 0xB3, 0x04, 0x2F, 0xC6, 0x84, 0xD9, 0x84,
                        0x12, 0x61, 0xFB, 0xC3, 0x60, 0xCF, 0x1B, 0x96, 0x3A, 0xCA
                    }
        };

        public AdminPortalControllerTests()
        {
            _mockDbContext = new Mock<TRSDbContext>();

            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(x => x["JWT:JWTSigningKey"]).Returns(_jwtSigningKey);
            _mockConfiguration.Setup(x => x["JWT:Issuer"]).Returns(_jwtIssuer);
            _mockConfiguration.Setup(x => x["JWT:Audience"]).Returns(_jwtAudience);
            _mockConfiguration.Setup(x => x["RegistrationCode"]).Returns(_registrationCode);
        }

        [Fact]
        public async void Login_ValidInput_ReturnsToken()
        {
            // Arrange
            var users = new List<TblUsers> { _testUser };
            _mockDbContext.Setup(m => m.Users).ReturnsDbSet(users);

            var controller = new AdminPortalController(_mockConfiguration.Object, _mockDbContext.Object);

            var requestBody = new DTOLoginRequset()
            {
                Username = "testAdmin",
                Password = _testUserPassword
            };

            // Act
            var result = await controller.Login(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.IsType<ActionResult<string>>(result);
            // Assert that token contains the correct header
            Assert.Contains("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", result.Value);

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async void Login_InvalidInput_EmailUsernamePassword_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AdminPortalController(_mockConfiguration.Object, _mockDbContext.Object);

            var requestBody = new DTOLoginRequset()
            {
                Username = "",
                Email = "",
                Password = ""
            };

            // Act
            var result = await controller.Login(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void Login_InvalidInput_InvalidUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AdminPortalController(_mockConfiguration.Object, _mockDbContext.Object);

            var requestBody = new DTOLoginRequset()
            {
                Username = "testAdmin!",
                Password = _testUserPassword
            };

            // Act
            var result = await controller.Login(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void Login_InvalidInput_InvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AdminPortalController(_mockConfiguration.Object, _mockDbContext.Object);

            var requestBody = new DTOLoginRequset()
            {
                Email = "testAdmin!",
                Password = _testUserPassword
            };

            // Act
            var result = await controller.Login(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void Login_InvalidInput_UserDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AdminPortalController(_mockConfiguration.Object, _mockDbContext.Object);

            _mockDbContext.Setup(m => m.Users).ReturnsDbSet([]);

            var requestBody = new DTOLoginRequset()
            {
                Username = "testAdmin",
                Password = _testUserPassword
            };

            // Act
            var result = await controller.Login(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async void Login_ValidInput_InvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var users = new List<TblUsers>
            {
                new TblUsers
                {
                    Id = 1,
                    Email = "testAdmin@test.dk",
                    Username = "testAdmin",
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.Now,
                    // Password is MySecretPassword123
                    PasswordHash = new byte[] {
                        0x84, 0x7F, 0x92, 0xCB, 0xE8, 0x6D, 0xA8, 0x86, 0x5F, 0x43, 0x4C,
                        0xC4, 0x50, 0x48, 0xF9, 0xF2, 0xF8, 0x03, 0x02, 0x43, 0xAD, 0xB4,
                        0x0D, 0x4A, 0x5F, 0x1F, 0x75, 0xAC, 0xC4, 0xC8, 0x56, 0x02
                    },
                    // Salt is random bytes
                    Salt = new byte[] {
                        0xE3, 0x09, 0x10, 0x01, 0x8F, 0x3B, 0x2E, 0x5A, 0x7F, 0xE2, 0x48,
                        0x3D, 0xB2, 0xDB, 0xF1, 0xB3, 0x04, 0x2F, 0xC6, 0x84, 0xD9, 0x84,
                        0x12, 0x61, 0xFB, 0xC3, 0x60, 0xCF, 0x1B, 0x96, 0x3A, 0xCA
                    }
                }
            };
            _mockDbContext.Setup(m => m.Users).ReturnsDbSet(users);
            var controller = new AdminPortalController(_mockConfiguration.Object, _mockDbContext.Object);

            var requestBody = new DTOLoginRequset()
            {
                Username = "testAdmin",
                Password = "WrongPassword123"
            };

            // Act
            var result = await controller.Login(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async void Register_ValidInput_ReturnsOk()
        {
            // Arrange
            var controller = new AdminPortalController(_mockConfiguration.Object, _mockDbContext.Object);

            _mockDbContext.Setup(m => m.Users).ReturnsDbSet([]);

            var requestBody = new DTORegisterUserRequest()
            {
                Username = _testUser.Username,
                Email = _testUser.Email,
                Password = _testUserPassword,
                RegistrationCode = _registrationCode
            };

            // Act
            var result = await controller.RegisterUser(requestBody);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            Assert.Equal("Successfully registered user.", result.Value);

            _mockDbContext.Verify(m => m.Users.AddAsync(It.IsAny<TblUsers>(), default), Times.Once);
            _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);

            // Clean up
            _mockDbContext.Reset();

        }

        [Fact]
        public async void Register_InvalidInput_UsernameNull_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AdminPortalController(_mockConfiguration.Object, _mockDbContext.Object);

            var requestBody = new DTORegisterUserRequest()
            {
                Username = null,
                Email = _testUser.Email,
                Password = _testUserPassword,
                RegistrationCode = _registrationCode
            };

            // Act
            var result = await controller.RegisterUser(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void Register_InvalidInput_EmailNull_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AdminPortalController(_mockConfiguration.Object, _mockDbContext.Object);

            var requestBody = new DTORegisterUserRequest()
            {
                Username = _testUser.Username,
                Email = null,
                Password = _testUserPassword,
                RegistrationCode = _registrationCode
            };

            // Act
            var result = await controller.RegisterUser(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void Register_InvalidInput_PasswordNull_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AdminPortalController(_mockConfiguration.Object, _mockDbContext.Object);

            var requestBody = new DTORegisterUserRequest()
            {
                Username = _testUser.Username,
                Email = _testUser.Email,
                Password = null,
                RegistrationCode = _registrationCode

            };

            // Act
            var result = await controller.RegisterUser(requestBody);
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void Register_InvalidInput_RegistrationCodeNull_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AdminPortalController(_mockConfiguration.Object, _mockDbContext.Object);

            var requestBody = new DTORegisterUserRequest()
            {
                Username = _testUser.Username,
                Email = _testUser.Email,
                Password = _testUserPassword,
                RegistrationCode = null
            };

            // Act
            var result = await controller.RegisterUser(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void Register_ValidInput_UserExists_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AdminPortalController(_mockConfiguration.Object, _mockDbContext.Object);

            var users = new List<TblUsers>
            {
                new TblUsers
                {
                    Id = 1,
                    Email = "testAdmin@test.dk",
                    Username = "testAdmin",
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.Now,
                    // Password is MySecretPassword123
                    PasswordHash = new byte[] {
                        0x84, 0x7F, 0x92, 0xCB, 0xE8, 0x6D, 0xA8, 0x86, 0x5F, 0x43, 0x4C,
                        0xC4, 0x50, 0x48, 0xF9, 0xF2, 0xF8, 0x03, 0x02, 0x43, 0xAD, 0xB4,
                        0x0D, 0x4A, 0x5F, 0x1F, 0x75, 0xAC, 0xC4, 0xC8, 0x56, 0x02
                    },
                    // Salt is random bytes
                    Salt = new byte[] {
                        0xE3, 0x09, 0x10, 0x01, 0x8F, 0x3B, 0x2E, 0x5A, 0x7F, 0xE2, 0x48,
                        0x3D, 0xB2, 0xDB, 0xF1, 0xB3, 0x04, 0x2F, 0xC6, 0x84, 0xD9, 0x84,
                        0x12, 0x61, 0xFB, 0xC3, 0x60, 0xCF, 0x1B, 0x96, 0x3A, 0xCA
                    }
                }
            };

            _mockDbContext.Setup(m => m.Users).ReturnsDbSet(users);

            var requestBody = new DTORegisterUserRequest()
            {
                Username = _testUser.Username,
                Email = _testUser.Email,
                Password = _testUserPassword,
                RegistrationCode = _registrationCode
            };

            // Act
            var result = await controller.RegisterUser(requestBody);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);

            // Clean up
            _mockDbContext.Reset();
        }
    }
}
