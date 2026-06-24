using DT_ASPNET.Domain.Wishlists;

namespace DT_ASPNET.Application.Wishlists;

public record WishlistItemDto(Guid PropertyId, DateTime SavedAt);

public class WishlistService(IWishlistRepository wishlists) : IWishlistService
{
    public async Task ToggleAsync(Guid userId, Guid propertyId)
    {
        if (await wishlists.ExistsAsync(userId, propertyId))
        {
            await wishlists.DeleteAsync(userId, propertyId);
        }
        else
        {
            await wishlists.AddAsync(new Wishlist
            {
                UserId = userId,
                PropertyId = propertyId
            });
        }

        await wishlists.SaveChangesAsync();
    }

    public async Task<List<WishlistItemDto>> GetAsync(Guid userId)
    {
        var list = await wishlists.GetByUserAsync(userId);
        return list.Select(w => new WishlistItemDto(w.PropertyId, w.CreatedAt)).ToList();
    }
}
