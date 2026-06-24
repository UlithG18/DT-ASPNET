using DT_ASPNET.Domain.Users;

namespace DT_ASPNET.Application.Users;

public record UserProfileDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool IsOwner,
    string KycStatus,
    string? KycRejectionReason);

public record UpdateProfileRequest(
    string FirstName,
    string LastName,
    string? PhoneNumber);

public class UserService(IUserRepository users) : IUserService
{
    public async Task<UserProfileDto?> GetProfileAsync(Guid userId)
    {
        var user = await users.GetByIdAsync(userId);

        return user is null
            ? null
            : ToDto(user);
    }

    public async Task UpdateProfileAsync(Guid userId, UpdateProfileRequest req)
    {
        var user = await users.GetByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found.");

        user.FirstName = req.FirstName;
        user.LastName = req.LastName;
        user.PhoneNumber = req.PhoneNumber;

        await users.SaveChangesAsync();
    }

    public async Task BecomeOwnerAsync(Guid userId)
    {
        var user = await users.GetByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found.");

        user.IsOwner = true;

        await users.SaveChangesAsync();
    }

    private static UserProfileDto ToDto(User user) => new(
        user.Id,
        user.Email,
        user.FirstName,
        user.LastName,
        user.PhoneNumber,
        user.IsOwner,
        user.KycStatus.ToString(),
        user.KycRejectionReason
    );
}