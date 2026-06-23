using DT_ASPNET.Application.User;
using DT_ASPNET.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DT_ASPNET.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<Domain.User.User?> GetByIdAsync(Guid id) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<Domain.User.User?> GetByEmailAsync(string email) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(Domain.User.User user) =>
        await db.Users.AddAsync(user);

    public Task SaveChangesAsync() =>
        db.SaveChangesAsync();
}
