using TRS_backend.API_Models;
using TRS_backend.DBModel;

namespace TRS_backend.Controllers
{
    public class DTOTablePlanForDateResponse {
        public List<TblTableReservations> Reservations { get; set; } = new List<TblTableReservations>();
    }

    
}