using DT_ASPNET.Domain.Properties;

namespace DT_ASPNET.Application.Properties;

public record PropertyDto(
    Guid Id, string Title, string Description,
    string City, string Address,
    decimal PricePerNight, int MaxGuests, int Bedrooms, int Bathrooms,
    List<string> PhotoUrls);

public record CreatePropertyRequest(
    string Title, string Description,
    string City, string Address,
    decimal PricePerNight, int MaxGuests, int Bedrooms, int Bathrooms,
    List<string> PhotoUrls);

public class PropertyService(IPropertyRepository properties) : IPropertyService
{
    public async Task<List<PropertyDto>> SearchAsync(string? city, DateOnly? checkIn, DateOnly? checkOut)
    {
        var list = await properties.SearchAsync(city, checkIn, checkOut);
        return list.Select(ToDto).ToList();
    }

    public async Task<PropertyDto?> GetByIdAsync(Guid id)
    {
        var p = await properties.GetByIdAsync(id);
        return p is null ? null : ToDto(p);
    }

    public async Task<PropertyDto> CreateAsync(Guid ownerId, CreatePropertyRequest req)
    {
        var property = new Domain.Properties.Property
        {
            OwnerId = ownerId,
            Title = req.Title,
            Description = req.Description,
            City = req.City,
            Address = req.Address,
            PricePerNight = req.PricePerNight,
            MaxGuests = req.MaxGuests,
            Bedrooms = req.Bedrooms,
            Bathrooms = req.Bathrooms,
            PhotoUrls = req.PhotoUrls
        };

        await properties.AddAsync(property);
        await properties.SaveChangesAsync();
        return ToDto(property);
    }

    public async Task<List<PropertyDto>> GetByOwnerAsync(Guid ownerId)
    {
        var list = await properties.GetByOwnerAsync(ownerId);
        return list.Select(ToDto).ToList();
    }

    public async Task UpdateAsync(Guid ownerId, Guid propertyId, CreatePropertyRequest req)
    {
        var property = await properties.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedAccessException("Not your property.");

        property.Title = req.Title;
        property.Description = req.Description;
        property.City = req.City;
        property.Address = req.Address;
        property.PricePerNight = req.PricePerNight;
        property.MaxGuests = req.MaxGuests;
        property.Bedrooms = req.Bedrooms;
        property.Bathrooms = req.Bathrooms;
        property.PhotoUrls = req.PhotoUrls;

        await properties.SaveChangesAsync();
    }

    private static PropertyDto ToDto(Domain.Properties.Property p) => new(
        p.Id, p.Title, p.Description, p.City, p.Address,
        p.PricePerNight, p.MaxGuests, p.Bedrooms, p.Bathrooms, p.PhotoUrls);
}
