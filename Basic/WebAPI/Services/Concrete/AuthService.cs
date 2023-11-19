using FootballAPI.Security;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.ModelViews;
using WebAPI.Security;
using WebAPI.Services.Abstract;

namespace WebAPI.Services.Concrete;

public class AuthService : IAuthService
{
    private readonly ApplicationDBContext _applicationDBContext;
    private readonly ITokenHelper _tokenHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        ApplicationDBContext applicationDBContext,
        ITokenHelper tokenHelper,
        IHttpContextAccessor httpContextAccessor)
    {
        _applicationDBContext = applicationDBContext;
        _tokenHelper = tokenHelper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoginResponse> Login(LoginRequest loginRequest)
    {
        var user = await _applicationDBContext.Users
            .FirstOrDefaultAsync(user => user.Email == loginRequest.Email);

        var result = HashingHelper
            .VerifyPasswordHash(loginRequest.Password, user.PasswordHash, user.PasswordSalt);

        if (!result) throw new BadHttpRequestException("Wrong credentials");

        AccessToken accessToken = await CreateAccessToken(user);

        //CREATE AND SAVE REFRESH TOKEN
        var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        var refreshToken = _tokenHelper.CreateRefreshToken(user, ipAddress);
        await _applicationDBContext.RefreshTokens.AddAsync(refreshToken);
        await _applicationDBContext.SaveChangesAsync();

        LoginResponse loginResponse = new()
        {
            AccessToken = accessToken.Token,
            RefreshToken = refreshToken.Token,
            Expiration = accessToken.Expiration
        };

        return loginResponse;
    }

    public async Task Register(RegisterRequest registerRequest)
    {
        var user = await _applicationDBContext.Users
            .FirstOrDefaultAsync(user => user.Username.ToLower() == registerRequest.Username.ToLower()
                || user.Email.ToLower() == registerRequest.Email.ToLower());

        if (user is not null) throw new BadHttpRequestException("Username or email is already in use");

        HashingHelper.CreatePasswordHash(registerRequest.Password,
            out byte[] passwordHash, out byte[] passwordSalt);

        var userToRegister = new User()
        {
            Email = registerRequest.Email,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            Username = registerRequest.Username,
            PasswordSalt = passwordSalt,
            PasswordHash = passwordHash,
        };
        await _applicationDBContext.Users.AddAsync(userToRegister);
        await _applicationDBContext.SaveChangesAsync();
    }

    public async Task<LoginResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        var refreshToken = await _applicationDBContext.RefreshTokens
            .FirstOrDefaultAsync(refreshToken => refreshToken.Token == refreshTokenRequest.Token)
            ?? throw new BadHttpRequestException("Refresh Token does not exist");

        var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        if (refreshToken.Revoked is not null)
        {
            await RevokeDescendantRefreshTokens(refreshToken, ipAddress, $"Attempted reuse of revoked ancestor token: {refreshToken.Token}");
            throw new BadHttpRequestException("This refresh token is revoked because attempted reuse of revoked ancestor");
        }

        if (DateTime.UtcNow > refreshToken.Expires)
            throw new BadHttpRequestException("Refresh token is not active");

        var user = await _applicationDBContext.Users
            .FirstOrDefaultAsync(user => user.Id == refreshToken.UserId);
        var newRefreshToken = await RotateRefreshToken(user, refreshToken, ipAddress);
        await _applicationDBContext.RefreshTokens.AddAsync(newRefreshToken);
        await _applicationDBContext.SaveChangesAsync();

        var accessToken = await CreateAccessToken(user);

        LoginResponse loginResponse = new()
        {
            AccessToken = accessToken.Token,
            RefreshToken = newRefreshToken.Token,
            Expiration = accessToken.Expiration
        };

        return loginResponse;
    }

    private async Task<AccessToken> CreateAccessToken(User user)
    {
        var operationClaims = await _applicationDBContext.UserOperationClaims
                    .Where(userOperationClaim => userOperationClaim.UserId == user.Id)
                    .Include(userOperationClaim => userOperationClaim.OperationClaim)
                    .Select(userOperationClaim => userOperationClaim.OperationClaim)
                    .ToListAsync();

        var trimmedOperationClaims = operationClaims.Select(op =>
        {
            op.Name = op.Name.Trim();
            return op;
        }).ToList();

        var accessToken = _tokenHelper.CreateToken(user, trimmedOperationClaims);
        return accessToken;
    }

    private async Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason)
    {
        if (refreshToken.ReplacedByToken is null) return;

        var childToken = await _applicationDBContext.RefreshTokens
            .FirstOrDefaultAsync(refreshTokenQuery => refreshTokenQuery.Token == refreshToken.ReplacedByToken);

        if (childToken is not null && childToken.Revoked is null)
            await RevokeRefreshToken(childToken, ipAddress, reason, null);
        else
            await RevokeDescendantRefreshTokens(childToken, ipAddress, reason);
    }

    private async Task RevokeRefreshToken(RefreshToken refreshToken, string ipAddress, string? reason, string? replacedByToken)
    {
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoked = reason;
        refreshToken.ReplacedByToken = replacedByToken;
        await _applicationDBContext.SaveChangesAsync();
    }

    private async Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = _tokenHelper.CreateRefreshToken(user, ipAddress);
        newRefreshToken.Created = DateTime.UtcNow;

        await RevokeRefreshToken(refreshToken,
            ipAddress,
            "New refresh token requested",
            newRefreshToken.Token
        );

        return newRefreshToken;
    }
}
