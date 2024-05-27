namespace TRS_backend.API_Models.Admin_Portal
{
    public class DTOSetOpenDayForDateRequest
    {
        public DateOnly Date { get; set; }
        public bool IsOpen { get; set; }
    }
}
