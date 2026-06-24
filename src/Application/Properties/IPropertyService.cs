namespace DT_ASPNET.Application.Properties;

public interface IPropertyService
{
    Task<List<PropertyDto>> SearchAsync(string? city, DateOnly? checkIn, DateOnly? checkOut);
    Task<PropertyDto?> GetByIdAsync(Guid id);
    Task<List<PropertyDto>> GetByOwnerAsync(Guid ownerId);
    Task<PropertyDto> CreateAsync(Guid ownerId, CreatePropertyRequest req);
    Task UpdateAsync(Guid ownerId, Guid propertyId, CreatePropertyRequest req);
}