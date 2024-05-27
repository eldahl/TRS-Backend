using TRS_backend.API_Models;
using TRS_backend.DBModel;

namespace TRS_backend.Controllers
{
    public class DTOSettingsRequest
    {
        public Settings Settings { get; set; }
        public TblTables[] Tables { get; set; } = [];
    }
}