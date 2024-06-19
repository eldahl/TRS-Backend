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
using TRS_backend.API_Models.Admin_Portal;
using TRS_backend.Controllers;
using TRS_backend.Controllers.Both;
using TRS_backend.DBModel;
using TRS_backend.Operational;

namespace TRS_backend_test.ControllerTests.Both
{
    public class CalendarControllerTests
    {
        Mock<TRSDbContext> _mockDbContext;
        Mock<SettingsFileContext> _mockSettingsFileContext;

        private readonly string _tempFilePath = Path.GetTempPath();
        private readonly string _tempFileName = "tempsettings.json";

        static TimeOnly _openTime = new TimeOnly(16, 0);
        static TimeOnly _closeTime = new TimeOnly(22, 0);

        List<TblOpenDays> _openDaysData = [
            new TblOpenDays()
            {
                Id = 1,
                Date = new DateOnly(2024, 01, 01),
                OpenTime = _openTime,
                CloseTime = _closeTime,
            },
            new TblOpenDays()
            {
                Id = 1,
                Date = new DateOnly(2024, 01, 02),
                OpenTime = _openTime,
                CloseTime = _closeTime,
            },
            new TblOpenDays()
            {
                Id = 1,
                Date = new DateOnly(2024, 01, 03),
                OpenTime = _openTime,
                CloseTime = _closeTime,
            }
        ];

        Settings _settingsData = new Settings()
        {
            OpenTime = _openTime,
            CloseTime = _closeTime
        };

        public CalendarControllerTests() {
            _mockDbContext = new Mock<TRSDbContext>();

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["SettingsFileName"]).Returns(_tempFileName);
            mockConfig.Setup(x => x["SettingsFilePath"]).Returns(_tempFilePath);
            var config = mockConfig.Object;

            _mockSettingsFileContext = new Mock<SettingsFileContext>(mockConfig.Object);

            _mockSettingsFileContext.Setup(x => x.Settings).Returns(_settingsData);
        }

        [Fact]
        public async void GetOpenDaysByMonthAndYear_ReturnsCorrectOpenDays()
        {
            // Arrange
            _mockDbContext.Setup(m => m.OpenDays).ReturnsDbSet(_openDaysData);
            var controller = new CalendarController(_mockDbContext.Object, _mockSettingsFileContext.Object);
            
            var requestBody = new DTOOpenDaysByMonthAndYearRequest()
            {
                Month = 1,
                Year = 2024
            };

            // Act
            var result = await controller.GetOpenDaysByMonthAndYear(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(_openDaysData.Count, result.Value.OpenDays.Count);

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async void SetOpenDayForDate_RemovesOpenDaysEntry()
        {
            // Arrange
            _mockDbContext.Setup(m => m.OpenDays).ReturnsDbSet(_openDaysData);
            var controller = new CalendarController(_mockDbContext.Object, _mockSettingsFileContext.Object);
            
            var requestBody = new DTOSetOpenDayForDateRequest()
            {
                Date = new DateOnly(2024, 01, 01),
                IsOpen = false
            };

            // Act
            var result = await controller.SetOpenDayForDate(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            _mockDbContext.Verify(m => m.OpenDays.Remove(It.IsAny<TblOpenDays>()), Times.Once);

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async void SetOpenDayForDate_ReturnsBadRequestIfOpenDayIsAlreadyOpen()
        {
            // Arrange
            _mockDbContext.Setup(m => m.OpenDays).ReturnsDbSet(_openDaysData);
            var controller = new CalendarController(_mockDbContext.Object, _mockSettingsFileContext.Object);
            
            var requestBody = new DTOSetOpenDayForDateRequest()
            {
                Date = new DateOnly(2024, 01, 01),
                IsOpen = true
            };

            // Act
            var result = await controller.SetOpenDayForDate(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);

            // Clean up
            _mockDbContext.Reset();
        }

        [Fact]
        public async void SetOpenDayForDate_AddsNewOpenDayEntry()
        {
            // Arrange
            _mockDbContext.Setup(m => m.OpenDays).ReturnsDbSet(_openDaysData);
            var controller = new CalendarController(_mockDbContext.Object, _mockSettingsFileContext.Object);
            
            var requestBody = new DTOSetOpenDayForDateRequest()
            {
                Date = new DateOnly(2024, 01, 04),
                IsOpen = true
            };

            // Act
            var result = await controller.SetOpenDayForDate(requestBody);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<string>>(result);
            _mockDbContext.Verify(m => m.OpenDays.AddAsync(It.IsAny<TblOpenDays>(), default), Times.Once);
            _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);

            // Clean up
            _mockDbContext.Reset();
        }
    }
}
