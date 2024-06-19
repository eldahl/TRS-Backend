using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRS_backend.API_Models;
using TRS_backend.Controllers;
using TRS_backend.DBModel;

namespace TRS_backend_test.ControllerTests.AdminPortal
{
    public class TablePlanControllerTests
    {
        Mock<TRSDbContext> _mockDbContext;

        public TablePlanControllerTests()
        {
            _mockDbContext = new Mock<TRSDbContext>();
        }

        [Fact]
        public void GetTablePlan_ReturnsTablePlan()
        {
            // Arrange
            var tableReservations = new List<TblTableReservations>()
            {
                new TblTableReservations()
                {
                    Id = 1,
                    Table = new TblTables()
                    {
                        Id = 1,
                        Seats = 4,
                        TableName = "Table 1"
                    },
                    OpenDay = new TblOpenDays()
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now)
                    },
                    TimeSlot = new TblTimeSlots()
                    {
                        StartTime = TimeOnly.FromDateTime(DateTime.Now),
                        Duration = TimeSpan.FromMinutes(120)
                    },
                    FullName = "John Doe",
                    Email = "test@test.dk",
                    PhoneNumber = "12345678",
                    SendReminders = true,
                    Comment = "Test comment"
                }
            };
            _mockDbContext.Setup(m => m.TableReservations).ReturnsDbSet(tableReservations);
            var controller = new TablePlanController(_mockDbContext.Object);

            var requestBody = new DTOTablePlanForDateRequest()
            {
                Date = DateOnly.FromDateTime(DateTime.Now)
            };

            // Act
            var result = controller.GetTablePlanForDate(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(tableReservations, result.Value.Reservations);

            // Clean up
            _mockDbContext.Reset();
        }
    }
}
