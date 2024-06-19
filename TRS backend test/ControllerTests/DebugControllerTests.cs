using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TRS_backend.Controllers;

namespace TRS_backend_test.ControllerTests
{
    public class DebugControllerTests
    {
        [Fact]
        public void Role_ReturnsNullOnUnauthorized()
        {
            // Arrange
            var controller = new DebugController();
            var httpContextMock = new Mock<HttpContext>();
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            httpContextMock.Setup(x => x.User.Claims).Returns([]);

            // Act
            var result = controller.Role();

            // Assert
            Assert.Null(result.Value);
        }

        [Fact]
        public void Role_ReturnsCorrectRole()
        {
            // Arrange
            var controller = new DebugController();
            var httpContextMock = new Mock<HttpContext>();
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            string role = "Admin";

            httpContextMock.Setup(x => x.User.Claims).Returns([
                new Claim(ClaimTypes.Role, role)
            ]);

            // Act
            var result = controller.Role();

            // Assert
            Assert.Equal(role, result.Value);
        }
    }
}
