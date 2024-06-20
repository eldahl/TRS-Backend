using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRS_backend.API_Models;
using TRS_backend.API_Models.Admin_Portal.Settings;
using TRS_backend.Controllers;
using TRS_backend.DBModel;
using TRS_backend.Operational;

namespace TRS_backend_test.ControllerTests.AdminPortal
{
    public class SettingsControllerTests
    {
        Mock<TRSDbContext> _mockDbContext;
        Mock<SettingsFileContext> _mockSettingsContext;
        Mock<IConfiguration> _mockConfig;
        

        public SettingsControllerTests()
        {
            _mockDbContext = new Mock<TRSDbContext>();
            NewSettingsMock();
        }

        private void NewSettingsMock() {
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Reset();

            string _tempFilePath = Path.GetTempPath();
            string _tempFileName = "tempsettings.json";

            _mockConfig.Setup(x => x["SettingsFileName"]).Returns(_tempFileName);
            _mockConfig.Setup(x => x["SettingsFilePath"]).Returns(_tempFilePath);

            _mockSettingsContext = new Mock<SettingsFileContext>(_mockConfig.Object);
        }

        [Fact]
        public void GetSettings_ReturnsSettings()
        {
            // Arrange
            NewSettingsMock();

            var settings = new Settings()
            {
                TimeSlotDuration = "01:00:00",
                OpenTime = TimeOnly.FromDateTime(DateTime.Now),
                CloseTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(8)),
                ServingInterval = TimeOnly.FromDateTime(DateTime.MinValue.AddMinutes(30)),
                ReservationsPerTimeSlot = 10,
                ReservationNotificationEmails = [
                    new ReservationNotificationEmail() {
                        Email = "test@test.dk"
                    }
                ],
                ReservationNotificationPhoneNumbers = [
                    new ReservationNotificationPhoneNumber() {
                        CountryCode = 45,
                        PhoneNumber = "12345678"
                    }
                ],
                CustomerReminderSMSTemplate = "Test template",
                CustomerReminderEmailTemplate = "Test template"
            };

            var tables = new List<TblTables>() {
                new TblTables() {
                    Id = 1,
                    Seats = 4,
                    TableName = "Table 1"
                },
                new TblTables() {
                    Id = 2,
                    Seats = 6,
                    TableName = "Table 2"
                }
            };
            _mockSettingsContext.Setup(m => m.Settings).Returns(settings);
            _mockDbContext.Setup(m => m.Tables).ReturnsDbSet(tables);

            var controller = new SettingsController(_mockDbContext.Object, _mockSettingsContext.Object);

            // Act
            var result = controller.GetSettings();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(settings, result.Value.Settings);
            Assert.Equal(tables, result.Value.Tables);

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async void SetSettings_SetsSettings()
        {
            // Arrange
            NewSettingsMock();

            var settings = new Settings()
            {
                TimeSlotDuration = "01:00:00",
                OpenTime = TimeOnly.FromDateTime(DateTime.Now),
                CloseTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(8)),
                ServingInterval = TimeOnly.FromDateTime(DateTime.MinValue.AddMinutes(30)),
                ReservationsPerTimeSlot = 10,
                ReservationNotificationEmails = [
                    new ReservationNotificationEmail() {
                        Email = "test@test.dk"
                    }
                ],
                ReservationNotificationPhoneNumbers = [
                    new ReservationNotificationPhoneNumber() {
                        CountryCode = 45,
                        PhoneNumber = "12345678"
                    }
                ],
                CustomerReminderSMSTemplate = "Test template",
                CustomerReminderEmailTemplate = "Test template"
            };
            var tables = new List<TblTables>() {
                new TblTables() {
                    Id = 1,
                    Seats = 4,
                    TableName = "Table 1"
                },
                new TblTables() {
                    Id = 2,
                    Seats = 6,
                    TableName = "Table 2"
                }
            };
            var requestBody = new DTOSettingsRequest()
            {
                Settings = settings,
                Tables = tables.ToArray()
            };

            _mockDbContext.Setup(m => m.Tables).ReturnsDbSet(tables);
            _mockSettingsContext.Setup(m => m.Settings).Returns(settings);

            // Act
            var controller = new SettingsController(_mockDbContext.Object, _mockSettingsContext.Object);
            var result = await controller.SetSettings(requestBody);

            // Assert
            _mockDbContext.Verify(m => m.Tables.UpdateRange(requestBody.Tables), Times.Once);
            _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);

            _mockSettingsContext.VerifySet(m => m.Settings = requestBody.Settings, Times.Once);

            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(settings, _mockSettingsContext.Object.Settings);
            Assert.Equal(tables, result.Value.Tables);

            // Clean up
            _mockDbContext.Reset();
        }
    }
}
