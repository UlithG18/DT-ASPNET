namespace DT_ASPNET.Domain.Wishlists;

public interface IWishlistRepository
{
    Task<List<Wishlist>> GetByUserAsync(Guid userId);
    Task<bool> ExistsAsync(Guid userId, Guid propertyId);
    Task AddAsync(Wishlist wishlist);
    Task DeleteAsync(Guid userId, Guid propertyId);
    Task SaveChangesAsync();
}
