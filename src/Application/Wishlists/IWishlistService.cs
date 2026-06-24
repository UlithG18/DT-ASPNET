namespace DT_ASPNET.Application.Wishlists;

public interface IWishlistService
{
    Task ToggleAsync(Guid userId, Guid propertyId);
    Task<List<WishlistItemDto>> GetAsync(Guid userId);
}