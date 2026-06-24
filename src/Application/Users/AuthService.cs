using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DT_ASPNET.Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DT_ASPNET.Application.Users;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string AccessToken, string RefreshToken, Guid UserId, bool IsOwner);

public class AuthService(IUserRepository users, IConfiguration config)
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        if (await users.GetByEmailAsync(req.Email) is not null)
            throw new InvalidOperationException("Email already in use.");

        var user = new Domain.Users.User
        {
            Email = req.Email.ToLower(),
            FirstName = req.FirstName,
            LastName = req.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };

        await users.AddAsync(user);
        await users.SaveChangesAsync();

        return BuildTokens(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        var user = await users.GetByEmailAsync(req.Email.ToLower())
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var tokens = BuildTokens(user);

        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(30);
        await users.SaveChangesAsync();

        return tokens;
    }

    private AuthResponse BuildTokens(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:Secret"]!));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("isOwner", user.IsOwner.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return new AuthResponse(accessToken, refreshToken, user.Id, user.IsOwner);
    }
}
