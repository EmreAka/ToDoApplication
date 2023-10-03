using WebAPI.ModelViews;
using WebAPI.Services.Abstract;

namespace WebAPI.Endpoints;

public static class AuthEndpoints
{
    public static void RegisterAuthEndpoints(this WebApplication app)
    {
        var user = app.MapGroup("auth")
            .WithTags("Auth");


        user.MapPost("login",
            async (IAuthService authService, LoginRequest loginRequest) =>
            {
                var result = await authService.Login(loginRequest);
                return Results.Ok(result);
            });

        user.MapPost("register",
            async (IAuthService authService, RegisterRequest registerRequest) =>
            {
                await authService.Register(registerRequest);
                return Results.Ok();
            });
    }
}
