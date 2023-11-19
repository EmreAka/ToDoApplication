using WebAPI.Filters;
using WebAPI.ModelViews;
using WebAPI.Services.Abstract;

namespace WebAPI.Endpoints;

public static class AuthEndpoints
{
    public static void RegisterAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("auth")
            .WithTags("Auth");


        auth.MapPost("login",
            async (IAuthService authService, LoginRequest loginRequest) =>
            {
                var result = await authService.Login(loginRequest);
                return Results.Ok(result);
            });

        auth.MapPost("register",
            async (IAuthService authService, RegisterRequest registerRequest) =>
            {
                await authService.Register(registerRequest);
                return Results.Ok();
            }).AddEndpointFilter<ValidatorFilter<RegisterRequest>>();

        auth.MapPost("refresh-token",
            async (IAuthService authService, RefreshTokenRequest refreshTokenRequest) =>
            {
                var result = await authService.RefreshToken(refreshTokenRequest);
                return Results.Ok(result);
            }).AddEndpointFilter<ValidatorFilter<RefreshTokenRequest>>();
    }
}
