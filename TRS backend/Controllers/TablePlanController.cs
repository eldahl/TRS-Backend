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

        [Authorize]
        [HttpPost("GetTablePlanForDate")]
        public ActionResult<DTOTablePlanForDateResponse> GetTablePlanForDate([FromBody] DTOTablePlanForDateRequest requestBody)
        {
            var reservationsList = _dbContext.TableReservations.Select(tr => tr).Where(tr => tr.Day.Day == requestBody.Date).ToList();

            return new DTOTablePlanForDateResponse() {
                Reservations = reservationsList
            };
        }
    }
}
