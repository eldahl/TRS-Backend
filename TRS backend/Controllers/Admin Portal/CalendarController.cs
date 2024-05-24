using Microsoft.AspNetCore.Mvc;
using TRS_backend.API_Models;
using TRS_backend.DBModel;

namespace TRS_backend.Controllers
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

        [HttpPost("GetOpenDaysByMonthAndYear")]
        public ActionResult<DTOOpenDaysByMonthAndYearResponse> GetOpenDaysByMonthAndYear([FromBody] DTOOpenDaysByMonthAndYearRequest requestBody) { 
            
            var openDays = _dbContext.OpenDays.Select(od => od).Where(od => od.Day.Month == requestBody.Month && od.Day.Year == requestBody.Year).ToList();

            return new DTOOpenDaysByMonthAndYearResponse() {
                OpenDays = openDays
            };
        }
    }
}
