namespace DT_ASPNET.Application.Users;

public interface IUserService
{
    Task<UserProfileDto?> GetProfileAsync(Guid userId);
    Task UpdateProfileAsync(Guid userId, UpdateProfileRequest req);
    Task BecomeOwnerAsync(Guid userId);
}