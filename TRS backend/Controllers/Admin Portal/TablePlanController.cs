using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TRS_backend.API_Models;
using TRS_backend.DBModel;

namespace TRS_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TablePlanController : Controller
    {
        private readonly TRSDbContext _dbContext;

        public TablePlanController(TRSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get table plan for a specific date, so it can be displayed in the admin portal
        /// </summary>
        /// <param name="requestBody">Request parameters</param>
        /// <returns>List of table reservations</returns>
        [Authorize]
        [HttpPost("GetTablePlanForDate")]
        public ActionResult<DTOTablePlanForDateResponse> GetTablePlanForDate([FromBody] DTOTablePlanForDateRequest requestBody)
        {
            var reservationsList = _dbContext.TableReservations
                .Select(tr => tr)
                .Include(tr => tr.Table)
                .Include(tr => tr.OpenDay)
                .Include(tr => tr.TimeSlot)
                .Where(tr => tr.OpenDay.Date == requestBody.Date).ToList();

            return new DTOTablePlanForDateResponse() {
                Reservations = reservationsList
            };
        }
    }
}
