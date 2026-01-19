using GoldenCrown.DTOs;

namespace GoldenCrown.Services
{
    public interface IUserService
    {
        Task<(bool IsSuccess, string ErrorMessage)> RegisterAsync(RegisterRequest request);

        Task<string?> LoginAsync(LoginRequest request);
    }
}
