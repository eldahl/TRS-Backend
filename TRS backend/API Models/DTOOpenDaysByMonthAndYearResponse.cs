using TRS_backend.DBModel;

namespace TRS_backend.API_Models
{
    public class DTOOpenDaysByMonthAndYearResponse
    {
        public List<TblOpenDays> OpenDays { get; set; } = new List<TblOpenDays>();
    }
}
