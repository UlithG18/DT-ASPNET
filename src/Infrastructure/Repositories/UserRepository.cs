using DT_ASPNET.Domain.Users;
using DT_ASPNET.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DT_ASPNET.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<Domain.Users.User?> GetByIdAsync(Guid id) =>
        db.Users.FirstOrDefaultAsync<User>(u => u.Id == id);

    public Task<Domain.Users.User?> GetByEmailAsync(string email) =>
        db.Users.FirstOrDefaultAsync<User>(u => u.Email == email);

    public Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
    db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

    public async Task AddAsync(Domain.Users.User user) =>
        await db.Users.AddAsync(user);

    public Task SaveChangesAsync() =>
        db.SaveChangesAsync();
}
