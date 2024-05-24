using TRS_backend.API_Models;

namespace TRS_backend.Controllers
{
    public class DTOSetSettingsResponse
    {
        public SetSettingsModel Settings { get; set; } = new SetSettingsModel();
    }
}