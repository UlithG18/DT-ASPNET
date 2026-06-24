namespace DT_ASPNET.Domain.Wishlists;

public class Wishlist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid PropertyId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
