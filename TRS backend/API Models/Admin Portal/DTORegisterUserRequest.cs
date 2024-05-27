namespace TRS_backend.API_Models
{
    public class DTORegisterUserRequest
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? RegistrationCode { get; set; }
    }
}
