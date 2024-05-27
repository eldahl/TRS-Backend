using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TRS_backend.API_Models;
using TRS_backend.API_Models.Admin_Portal;
using TRS_backend.DBModel;
using TRS_backend.Operational;

namespace TRS_backend.Controllers.Both
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarController : Controller
    {
        private readonly TRSDbContext _dbContext;

        public CalendarController(TRSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get open days by month and year
        /// </summary>
        /// <param name="requestBody">Request parameters</param>
        /// <returns>Open days in the given month & year</returns>
        [HttpPost("GetOpenDaysByMonthAndYear")]
        public ActionResult<DTOOpenDaysByMonthAndYearResponse> GetOpenDaysByMonthAndYear([FromBody] DTOOpenDaysByMonthAndYearRequest requestBody)
        {
            var openDays = _dbContext.OpenDays.Select(od => od).Where(od => od.Date.Month == requestBody.Month && od.Date.Year == requestBody.Year).ToList();

            return new DTOOpenDaysByMonthAndYearResponse()
            {
                OpenDays = openDays
            };
        }
    }
}
