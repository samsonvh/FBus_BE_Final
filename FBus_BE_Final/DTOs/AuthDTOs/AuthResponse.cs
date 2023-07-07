namespace FBus_BE.DTOs.AuthDTOs
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Code { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
    }
}
