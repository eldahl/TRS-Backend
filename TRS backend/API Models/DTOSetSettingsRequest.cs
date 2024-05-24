using TRS_backend.API_Models;

namespace TRS_backend.Controllers
{
    public class DTOSetSettingsRequest
    {
        public SetSettingsModel Settings { get; set; } = new SetSettingsModel();
    }
}