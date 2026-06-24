namespace DT_ASPNET.Domain.Property;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(Guid id);
    Task<List<Property>> SearchAsync(string? city, DateOnly? checkIn, DateOnly? checkOut);
    Task<List<Property>> GetByOwnerAsync(Guid ownerId);
    Task AddAsync(Property property);
    Task SaveChangesAsync();
}
