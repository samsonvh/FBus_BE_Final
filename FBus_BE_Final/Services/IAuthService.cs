using FBus_BE.DTOs.AuthDTOs;

namespace FBus_BE.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> Authenticate(string idToken);
    }
}
