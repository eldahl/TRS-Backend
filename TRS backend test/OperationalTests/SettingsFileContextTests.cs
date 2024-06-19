using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using TRS_backend.Operational;
using System.Text.Json;
using TRS_backend.API_Models;
using TRS_backend.API_Models.Admin_Portal.Settings;

namespace TRS_backend_test.OperationalTests
{
    public class SettingsFileContextTests
    {
        private readonly string _tempFilePath = Path.GetTempPath();
        private readonly string _tempFileName = "tempsettings.json";

        [Fact]
        public void Constructor_SettingsFileIsCreated()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["SettingsFileName"]).Returns(_tempFileName);
            mockConfig.Setup(x => x["SettingsFilePath"]).Returns(_tempFilePath);
            var config = mockConfig.Object;

            // Act
            var context = new SettingsFileContext(config);

            // Assert
            Assert.NotNull(context);
            Assert.True(File.Exists(Path.Combine(_tempFilePath, _tempFileName)));

            // Clean up
            File.Delete(Path.Combine(_tempFilePath, _tempFileName));
        }

        [Fact]
        public void SaveSettings_WritesToFile()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["SettingsFileName"]).Returns(_tempFileName);
            mockConfig.Setup(x => x["SettingsFilePath"]).Returns(_tempFilePath);
            var config = mockConfig.Object;

            var context = new SettingsFileContext(config);

            var settingsToWrite = new Settings
            {
                OpenTime = TimeOnly.FromDateTime(new DateTime(2024, 1, 1, 0, 0, 0)),
                CloseTime = TimeOnly.FromDateTime(new DateTime(2024, 1, 1, 0, 0, 0).AddHours(2)),
                CustomerReminderEmailTemplate = "Test",
                CustomerReminderSMSTemplate = "Test",
                ReservationNotificationEmails = new ReservationNotificationEmail[] {
                    new ReservationNotificationEmail { Email = "Test" },
                    new ReservationNotificationEmail { Email = "Test" }
                },
                ReservationNotificationPhoneNumbers = new ReservationNotificationPhoneNumber[] {
                    new ReservationNotificationPhoneNumber { PhoneNumber = "Test", CountryCode = 45 },
                    new ReservationNotificationPhoneNumber { PhoneNumber = "Test", CountryCode = 45 }
                },
                ReservationsPerTimeSlot = 1,
                ServingInterval = TimeOnly.Parse("00:15:00"),
                TimeSlotDuration = "00:02:00"
            };

            // Act
            context.Settings = settingsToWrite;

            // Assert
            string fullPath = Path.Combine(_tempFilePath, _tempFileName);
            Assert.True(File.Exists(fullPath));
            string content = File.ReadAllText(fullPath);

            var settings = JsonSerializer.Deserialize<Settings>(content);

            Assert.NotNull(settings);

            Assert.Equal(settingsToWrite.TimeSlotDuration, settings.TimeSlotDuration);
            Assert.Equal(settingsToWrite.ReservationsPerTimeSlot, settings.ReservationsPerTimeSlot);
            Assert.Equal(settingsToWrite.ServingInterval, settings.ServingInterval);
            Assert.Equal(settingsToWrite.CustomerReminderEmailTemplate, settings.CustomerReminderEmailTemplate);
            Assert.Equal(settingsToWrite.CustomerReminderSMSTemplate, settings.CustomerReminderSMSTemplate);

            foreach (var element in settingsToWrite.ReservationNotificationEmails.Zip(settings.ReservationNotificationEmails))
            {
                Assert.Equal(element.First.Email, element.Second.Email);
            }
            foreach (var element in settingsToWrite.ReservationNotificationPhoneNumbers.Zip(settings.ReservationNotificationPhoneNumbers))
            {
                Assert.Equal(element.First.PhoneNumber, element.Second.PhoneNumber);
                Assert.Equal(element.First.CountryCode, element.Second.CountryCode);
            }

            // Clean up
            File.Delete(fullPath);
        }

        [Fact]
        public void LoadSettings_LoadsFromFile()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["SettingsFileName"]).Returns(_tempFileName);
            mockConfig.Setup(x => x["SettingsFilePath"]).Returns(_tempFilePath);
            var config = mockConfig.Object;

            string fullPath = Path.Combine(_tempFilePath, _tempFileName);
            var settingsToLoad = new Settings
            {
                OpenTime = TimeOnly.FromDateTime(new DateTime(2024, 1, 1, 0, 0, 0)),
                CloseTime = TimeOnly.FromDateTime(new DateTime(2024, 1, 1, 0, 0, 0).AddHours(2)),
                CustomerReminderEmailTemplate = "Test",
                CustomerReminderSMSTemplate = "Test",
                ReservationNotificationEmails = new ReservationNotificationEmail[] {
                    new ReservationNotificationEmail { Email = "Test" },
                    new ReservationNotificationEmail { Email = "Test" }
                },
                ReservationNotificationPhoneNumbers = new ReservationNotificationPhoneNumber[] {
                    new ReservationNotificationPhoneNumber { PhoneNumber = "Test", CountryCode = 45 },
                    new ReservationNotificationPhoneNumber { PhoneNumber = "Test", CountryCode = 45}
                },
                ReservationsPerTimeSlot = 1,
                ServingInterval = TimeOnly.Parse("00:15:00"),
                TimeSlotDuration = "02:00:00"
            };
            string settingsJson = JsonSerializer.Serialize(settingsToLoad);
            File.WriteAllText(fullPath, settingsJson);

            var context = new SettingsFileContext(config);

            // Act
            Settings loadedSettings = context.Settings;

            // Assert
            Assert.Equal(settingsToLoad.OpenTime, loadedSettings.OpenTime);
            Assert.Equal(settingsToLoad.CloseTime, loadedSettings.CloseTime);
            Assert.Equal(settingsToLoad.CustomerReminderEmailTemplate, loadedSettings.CustomerReminderEmailTemplate);
            Assert.Equal(settingsToLoad.CustomerReminderSMSTemplate, loadedSettings.CustomerReminderSMSTemplate);
            foreach (var element in settingsToLoad.ReservationNotificationEmails.Zip(loadedSettings.ReservationNotificationEmails))
            {
                Assert.Equal(element.First.Email, element.Second.Email);
            }
            foreach (var element in settingsToLoad.ReservationNotificationPhoneNumbers.Zip(loadedSettings.ReservationNotificationPhoneNumbers))
            {
                Assert.Equal(element.First.PhoneNumber, element.Second.PhoneNumber);
                Assert.Equal(element.First.CountryCode, element.Second.CountryCode);
            }
            Assert.Equal(settingsToLoad.ReservationsPerTimeSlot, loadedSettings.ReservationsPerTimeSlot);
            Assert.Equal(settingsToLoad.ServingInterval, loadedSettings.ServingInterval);
            Assert.Equal(settingsToLoad.TimeSlotDuration, loadedSettings.TimeSlotDuration);

            // Clean up
            File.Delete(fullPath);
        }
    }
}