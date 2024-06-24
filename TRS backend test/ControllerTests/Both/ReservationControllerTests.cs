using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRS_backend.API_Models;
using TRS_backend.Controllers;
using TRS_backend.Controllers.Both;
using TRS_backend.DBModel;
using TRS_backend.Operational;
using TRS_backend.Services;

namespace TRS_backend_test.ControllerTests.Both
{
    public class ReservationControllerTests
    {
        private readonly Mock<TRSDbContext> _mockDbContext;
        private readonly Mock<SettingsFileContext> _mockSettingsContext;
        private TimeSlotService _timeSlotService;

        private readonly string _tempFilePath = Path.GetTempPath();
        private readonly string _tempFileName = "tempsettings.json";

        private readonly Settings _settingsData = new Settings()
        {
            TimeSlotDuration = "02:00:00",
            OpenTime = new TimeOnly(16, 0),
            CloseTime = new TimeOnly(22, 0),
            ServingInterval = new TimeOnly(0, 30),
            ReservationsPerTimeSlot = 2,
            ReservationNotificationEmails = [],
            ReservationNotificationPhoneNumbers = [],
            CustomerReminderSMSTemplate = "",
            CustomerReminderEmailTemplate = ""
        };

        public ReservationControllerTests()
        {
            _mockDbContext = new Mock<TRSDbContext>();
            _timeSlotService = new TimeSlotService(_mockDbContext.Object);

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["SettingsFileName"]).Returns(_tempFileName);
            mockConfig.Setup(x => x["SettingsFilePath"]).Returns(_tempFilePath);
            var config = mockConfig.Object;
            _mockSettingsContext = new Mock<SettingsFileContext>(config);

            _mockSettingsContext.Setup(m => m.Settings).Returns(_settingsData);
        }

        [Fact]
        public async Task GetTimeSlotsForDate_ReturnsTimeSlots()
        {
            // Arrange
            var requestBody = new DTOGetTimeSlotsForDateRequest
            {
                Date = DateOnly.FromDateTime(DateTime.Now)
            };

            var timeSlots = new List<TblTimeSlots>
            {
                new TblTimeSlots
                {
                    Id = 1,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    StartTime = TimeOnly.FromDateTime(DateTime.Now),
                    Duration = new TimeSpan(2, 0, 0),               
                }
            };
            _mockSettingsContext.Setup(m => m.Settings).Returns(_settingsData);
            _mockDbContext.Setup(m => m.TimeSlots).ReturnsDbSet(timeSlots);
            

            var controller = new ReservationController(_mockDbContext.Object, _timeSlotService, _mockSettingsContext.Object);

            // Act
            var result = await controller.GetTimeSlotsForDate(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(timeSlots, result.Value);

            _mockDbContext.Verify(m => m.TimeSlots, Times.Once);

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async Task GetTimeSlotsForDate_ReturnsGeneratedTimeSlots()
        {
            // Arrange
            var requestBody = new DTOGetTimeSlotsForDateRequest {
                Date = DateOnly.FromDateTime(DateTime.Now)
            };

            _mockSettingsContext.Setup(m => m.Settings).Returns(_settingsData);
            _mockDbContext.Setup(m => m.TimeSlots).ReturnsDbSet(new List<TblTimeSlots>());

            var controller = new ReservationController(_mockDbContext.Object, _timeSlotService, _mockSettingsContext.Object);

            // Act
            var result = await controller.GetTimeSlotsForDate(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<TblTimeSlots>>(result.Value);

            _mockDbContext.Verify(m => m.TimeSlots, Times.Exactly(2));
            

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async Task GetTables_ReturnsTables()
        {
            // Arrange
            var tables = new List<TblTables>
            {
                new TblTables
                {
                    Id = 1,
                    TableName = "Table 1",
                    Seats = 4
                }
            };
            _mockDbContext.Setup(m => m.Tables).ReturnsDbSet(tables);

            var controller = new ReservationController(_mockDbContext.Object, _timeSlotService, _mockSettingsContext.Object);

            // Act
            var result = await controller.GetTables();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tables, result.Value);

            _mockDbContext.Verify(m => m.Tables, Times.Once);

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async Task ReserveTimeSlot_CreatesReservationSuccessfully()
        {
            // Arrange
            int timeSlotId = 1;
            int tableId = 1;

            _mockDbContext.Setup(m => m.Tables).ReturnsDbSet(new List<TblTables>() {
                new TblTables {
                    Id = tableId,
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
                    Id = timeSlotId,
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    StartTime = TimeOnly.FromDateTime(DateTime.Today),
                    Duration = new TimeSpan(2, 0, 0)
                }
            });
            _mockDbContext.Setup(mockDb => mockDb.TableReservations).ReturnsDbSet(new List<TblTableReservations>());

            var requestBody = new DTOReserveRequest
            {
                TableId = tableId,
                TimeSlotId = timeSlotId,
                FullName = "John Doe",
                Email = "",
                PhoneNumber = "1234567890",
                SendReminders = true,
                Comment = "Test comment"
            };

            var controller = new ReservationController(_mockDbContext.Object, _timeSlotService, _mockSettingsContext.Object);

            // Act
            var result = await controller.Reserve(requestBody);
            
            Assert.NotNull(result);
            Assert.IsType<ActionResult<TblTableReservations>>(result);

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async void ReserveTimeSlot_TableAlreadyReserved_ReturnsBadRequest()
        {
            // Arrange
            int timeSlotId = 1;
            int tableId1 = 1;
            int tableId2 = 2;

            var table1 = new TblTables
            {
                Id = tableId1,
                TableName = "Table 1",
                Seats = 4
            };

            _mockDbContext.Setup(m => m.Tables).ReturnsDbSet(new List<TblTables>() {
                table1
            });
            _mockDbContext.Setup(m => m.OpenDays).ReturnsDbSet(new List<TblOpenDays>() {
                new TblOpenDays {
                    Id = 1,
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    OpenTime = TimeOnly.FromDateTime(DateTime.Today),
                    CloseTime = TimeOnly.FromDateTime(DateTime.Today.AddHours(8))
                }
            });
            var timeslot = new TblTimeSlots
            {
                Id = timeSlotId,
                Date = DateOnly.FromDateTime(DateTime.Today),
                StartTime = TimeOnly.FromDateTime(DateTime.Today),
                Duration = new TimeSpan(2, 0, 0)
            };
            _mockDbContext.Setup(m => m.TimeSlots).ReturnsDbSet(new List<TblTimeSlots>() {
                timeslot
            });
            _mockDbContext.Setup(m => m.TableReservations).ReturnsDbSet(new List<TblTableReservations>() {
                new TblTableReservations {
                    TableId = tableId1,
                    TimeSlotId = timeSlotId,
                    OpenDayId = 1,
                    Table = table1,
                    TimeSlot = timeslot
                },
                new TblTableReservations {
                    TableId = tableId2,
                    TimeSlotId = timeSlotId,
                    OpenDayId = 1,
                    Table = table1,
                    TimeSlot = timeslot
                }
            });

            var requestBody = new DTOReserveRequest
            {
                TableId = tableId1,
                TimeSlotId = timeSlotId,
                FullName = "John Doe",
                Email = "john@doe.com",
                PhoneNumber = "1234567890",
                SendReminders = true,
                Comment = "Test comment"
            };

            var controller = new ReservationController(_mockDbContext.Object, _timeSlotService, _mockSettingsContext.Object);

            // Act
            var result = await controller.Reserve(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Table is already reserved", (result.Result as BadRequestObjectResult).Value);

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async void ReserveTimeSlot_InvalidInput_TableIdIsLessThanZero_ReturnsBadRequest()
        {
            // Arrange
            var controller = new ReservationController(_mockDbContext.Object, _timeSlotService, _mockSettingsContext.Object);

            var requestBody = new DTOReserveRequest
            {
                TableId = -1,
                TimeSlotId = 1,
                FullName = "John Doe",
                Email = "",
                PhoneNumber = "1234567890",
                SendReminders = true,
                Comment = "Test comment"
            };

            // Act
            var result = await controller.Reserve(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void ReserveTimeSlot_InvalidInput_TimeSlotIdIsLessThanZero_ReturnsBadRequest()
        {
            // Arrange
            var controller = new ReservationController(_mockDbContext.Object, _timeSlotService, _mockSettingsContext.Object);

            var requestBody = new DTOReserveRequest
            {
                TableId = 1,
                TimeSlotId = -1,
                FullName = "John Doe",
                Email = "",
                PhoneNumber = "1234567890",
                SendReminders = true,
                Comment = "Test comment"
            };

            // Act
            var result = await controller.Reserve(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void ReserveTimeSlot_InvalidInput_FullNameIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            var controller = new ReservationController(_mockDbContext.Object, _timeSlotService, _mockSettingsContext.Object);

            var requestBody = new DTOReserveRequest
            {
                TableId = 1,
                TimeSlotId = 1,
                FullName = "John Doe123",
                Email = "",
                PhoneNumber = "1234567890",
                SendReminders = true,
                Comment = "Test comment"
            };

            // Act
            var result = await controller.Reserve(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void ReserveTimeSlot_InvalidInput_EmailIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            var controller = new ReservationController(_mockDbContext.Object, _timeSlotService, _mockSettingsContext.Object);

            var requestBody = new DTOReserveRequest
            {
                TableId = 1,
                TimeSlotId = 1,
                FullName = "John Doe",
                Email = "johnexample.com",
                PhoneNumber = "1234567890",
                SendReminders = true,
                Comment = "Test comment"
            };

            // Act
            var result = await controller.Reserve(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void ReserveTimeSlot_InvalidInput_PhoneNumberIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            var controller = new ReservationController(_mockDbContext.Object, _timeSlotService, _mockSettingsContext.Object);

            var requestBody = new DTOReserveRequest
            {
                TableId = 1,
                TimeSlotId = 1,
                FullName = "John Doe",
                Email = null!,
                PhoneNumber = "1234567890a",
                SendReminders = true,
                Comment = "Test comment"
            };

            // Act
            var result = await controller.Reserve(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
