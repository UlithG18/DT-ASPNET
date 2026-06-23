using DT_ASPNET.Domain.Wishlist;
using DT_ASPNET.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DT_ASPNET.Infrastructure.Repositories;

public class WishlistRepository(AppDbContext db) : IWishlistRepository
{
    public Task<List<Domain.Wishlist.Wishlist>> GetByUserAsync(Guid userId) =>
        db.Wishlists.Where(w => w.UserId == userId).ToListAsync();

    public Task<bool> ExistsAsync(Guid userId, Guid propertyId) =>
        db.Wishlists.AnyAsync(w => w.UserId == userId && w.PropertyId == propertyId);

    public async Task AddAsync(Domain.Wishlist.Wishlist wishlist) =>
        await db.Wishlists.AddAsync(wishlist);

    public async Task DeleteAsync(Guid userId, Guid propertyId)
    {
        var item = await db.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.PropertyId == propertyId);
        if (item is not null)
            db.Wishlists.Remove(item);
    }

    public Task SaveChangesAsync() =>
        db.SaveChangesAsync();
}
