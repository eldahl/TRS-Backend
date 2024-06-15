using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRS_backend.Controllers;
using TRS_backend.DBModel;
using TRS_backend.Services;

namespace TRS_backend_test
{
    public class TimeSlotServiceTests
    {
        private readonly Mock<TRSDbContext> _mockDbContext;
        private readonly TimeSlotService _timeSlotService;
        private readonly Mock<DbSet<TblTimeSlots>> _mockTimeSlotsSet;
        private readonly Mock<DbSet<TblTableReservations>> _mockTableReservationsSet;

        private readonly Mock<DbSet<TblTables>> _mockDbSetTables = new Mock<DbSet<TblTables>>();
        private readonly Mock<DbSet<TblTimeSlots>> _mockDbSetTimeSlots = new Mock<DbSet<TblTimeSlots>>();
        private readonly Mock<DbSet<TblOpenDays>> _mockDbSetOpenDays = new Mock<DbSet<TblOpenDays>>();

        public TimeSlotServiceTests()
        {
            _mockDbContext = new Mock<TRSDbContext>();
            _timeSlotService = new TimeSlotService(_mockDbContext.Object);
            _mockTimeSlotsSet = new Mock<DbSet<TblTimeSlots>>();
            _mockTableReservationsSet = new Mock<DbSet<TblTableReservations>>();

            _mockDbContext.Setup(m => m.TimeSlots).Returns(_mockTimeSlotsSet.Object);
            _mockDbContext.Setup(m => m.TableReservations).Returns(_mockTableReservationsSet.Object);
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
            var servingInterval = TimeOnly.FromDateTime(DateTime.Now.AddMinutes(30));

            // Act
            var result = await _timeSlotService.GenerateTimeSlots(date, startTime, endTime, diningDuration, servingsPerTimeSlot, servingInterval);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(17, result.Count);  // 8 hours / 30 minutes = 16 intervals + 1 for the full coverage
            _mockTimeSlotsSet.Verify(m => m.AddRangeAsync(It.IsAny<IEnumerable<TblTimeSlots>>(), default), Times.Once);
            _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task ReserveTimeSlot_CreatesReservationSuccessfully()
        {
            // Arrange
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

            var table = new TblTables { Id = 1 };
            var timeSlot = new TblTimeSlots { Id = 1, Date = DateOnly.FromDateTime(DateTime.Today) };
            var openDay = new TblOpenDays { Date = DateOnly.FromDateTime(DateTime.Today) };

            _mockDbSetTables.ReturnsDbSet(new List<TblTables> { table });
            _mockDbSetTimeSlots.ReturnsDbSet(new List<TblTimeSlots> { timeSlot });
            _mockDbSetOpenDays.ReturnsDbSet(new List<TblOpenDays> { openDay });

            // Act
            var result = await _timeSlotService.ReserveTimeSlot(requestBody);

            // Assert
            Assert.IsType<ActionResult<TblTableReservations>>(result);
            _mockTableReservationsSet.Verify(m => m.AddAsync(It.IsAny<TblTableReservations>(), default), Times.Once);
            _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
    }
}
