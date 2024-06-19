using Castle.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore;
using Moq.EntityFrameworkCore.DbAsyncQueryProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRS_backend.Controllers;
using TRS_backend.DBModel;
using TRS_backend.Services;

namespace TRS_backend_test.OperationalTests
{
    public class TimeSlotServiceTests
    {
        private readonly Mock<TRSDbContext> _mockDbContext;
        private TimeSlotService _timeSlotService;

        public TimeSlotServiceTests()
        {
            _mockDbContext = new Mock<TRSDbContext>();
            _timeSlotService = new TimeSlotService(_mockDbContext.Object);
        }

        [Fact]
        public async Task GenerateTimeSlots_CreatesCorrectNumberOfSlots()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.Now);
            var startTime = TimeOnly.FromDateTime(DateTime.Now);
            var endTime = startTime.AddHours(8);
            var diningDuration = new TimeSpan(2, 0, 0);
            var servingsPerTimeSlot = 10;
            var servingInterval = TimeOnly.FromDateTime(DateTime.MinValue.AddMinutes(30));

            // Setup mockDbContext data
            _mockDbContext.Setup(m => m.TimeSlots).ReturnsDbSet(new List<TblTimeSlots>());
            _mockDbContext.Setup(m => m.TableReservations).ReturnsDbSet(new List<TblTableReservations>());

            // Act
            var result = await _timeSlotService.GenerateTimeSlots(date, startTime, endTime, diningDuration, servingsPerTimeSlot, servingInterval);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(17, result.Count);  // 8 hours / 30 minutes = 16 intervals + 1 for the full coverage
            _mockDbContext.Verify(m => m.TimeSlots.AddRangeAsync(It.IsAny<IEnumerable<TblTimeSlots>>(), default), Times.Once);
            _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async Task ReserveTimeSlot_CreatesReservationSuccessfully()
        {
            // Arrange
            // Setup mockDbContext data
            var table = new TblTables { Id = 1 };
            var timeSlot = new TblTimeSlots { Id = 1, Date = DateOnly.FromDateTime(DateTime.Today) };
            var openDay = new TblOpenDays { Date = DateOnly.FromDateTime(DateTime.Today) };

            _mockDbContext.Setup(m => m.Tables).ReturnsDbSet(new List<TblTables>() {
                new TblTables {
                    Id = 1,
                    TableName = "Table 1",
                    Seats = 4
                }
            });
            _mockDbContext.Setup(mockDb => mockDb.OpenDays).ReturnsDbSet(new List<TblOpenDays>() {
                new TblOpenDays {
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    OpenTime = TimeOnly.FromDateTime(DateTime.Today),
                    CloseTime = TimeOnly.FromDateTime(DateTime.Today.AddHours(8))
                }
            });
            _mockDbContext.Setup(mockDb => mockDb.TimeSlots).ReturnsDbSet(new List<TblTimeSlots>() {
                new TblTimeSlots {
                    Id = 1,
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    StartTime = TimeOnly.FromDateTime(DateTime.Today),
                    Duration = new TimeSpan(2, 0, 0)
                }
            });
            _mockDbContext.Setup(mockDb => mockDb.TableReservations).ReturnsDbSet(new List<TblTableReservations>());

            var requestBody = new DTOReserveRequest
            {
                TableId = 1,
                TimeSlotId = 1,
                FullName = "John Doe",
                Email = "john@example.com",
                PhoneNumber = "1234567890",
                SendReminders = true,
                Comment = "Test comment"
            };

            // Act
            var result = await _timeSlotService.ReserveTimeSlot(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<TblTableReservations>>(result);
            _mockDbContext.Verify(m => m.TableReservations.AddAsync(It.IsAny<TblTableReservations>(), default), Times.Once);
            _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);

            // Clean up
            _mockDbContext.Reset();
        }
    }
}
