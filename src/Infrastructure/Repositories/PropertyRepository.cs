using DT_ASPNET.Domain.Properties;
using DT_ASPNET.Domain.Reservations;
using DT_ASPNET.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DT_ASPNET.Infrastructure.Repositories;

public class PropertyRepository(AppDbContext db) : IPropertyRepository
{
    public Task<Domain.Properties.Property?> GetByIdAsync(Guid id) =>
        db.Properties.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

    public async Task<List<Domain.Properties.Property>> SearchAsync(
        string? city, DateOnly? checkIn, DateOnly? checkOut)
    {
        var query = db.Properties.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(p => p.City.ToLower().Contains(city.ToLower()));

        // Si se pasan fechas, excluir propiedades con reservas solapadas
        if (checkIn.HasValue && checkOut.HasValue)
        {
            var cin = checkIn.Value.ToDateTime(new TimeOnly(14, 0), DateTimeKind.Utc);
            var cout = checkOut.Value.ToDateTime(new TimeOnly(12, 0), DateTimeKind.Utc);

            var unavailableIds = db.Reservations
                .Where(r => r.Status != ReservationStatus.Cancelled
                         && r.CheckIn < cout
                         && r.CheckOut > cin)
                .Select(r => r.PropertyId);

            query = query.Where(p => !unavailableIds.Contains(p.Id));
        }

        return await query.ToListAsync();
    }

    public Task<List<Domain.Properties.Property>> GetByOwnerAsync(Guid ownerId) =>
        db.Properties.Where(p => p.OwnerId == ownerId).ToListAsync();

    public async Task AddAsync(Domain.Properties.Property property) =>
        await db.Properties.AddAsync(property);

    public Task SaveChangesAsync() =>
        db.SaveChangesAsync();
}
