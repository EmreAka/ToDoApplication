using WebAPI.ModelViews;

namespace WebAPI.Services.Abstract;

public interface IAuthService
{
    Task Register(RegisterRequest registerRequest);
    Task<LoginResponse> Login(LoginRequest loginRequest);
}
