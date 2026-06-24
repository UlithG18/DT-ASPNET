using DT_ASPNET.Domain.Reservations;
using DT_ASPNET.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DT_ASPNET.Infrastructure.Repositories;

public class ReservationRepository(AppDbContext db) : IReservationRepository
{
    public Task<Reservation?> GetByIdAsync(Guid id) =>
        db.Reservations.FirstOrDefaultAsync(r => r.Id == id);

    public Task<List<Reservation>> GetByGuestAsync(Guid guestId) =>
        db.Reservations.Where(r => r.GuestId == guestId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public Task<List<Reservation>> GetByPropertyAsync(Guid propertyId) =>
        db.Reservations.Where(r => r.PropertyId == propertyId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    // Verifica solapamiento: hay conflicto si alguna reserva activa del inmueble
    // empieza antes de que la nueva termine, y termina después de que la nueva empieza.
    public Task<bool> HasOverlapAsync(Guid propertyId, DateTime checkIn, DateTime checkOut) =>
        db.Reservations.AnyAsync(r =>
            r.PropertyId == propertyId &&
            r.Status != ReservationStatus.Cancelled &&
            r.CheckIn < checkOut &&
            r.CheckOut > checkIn);

    public Task<List<Reservation>> GetByOwnerAsync(Guid ownerId, Guid? propertyId, DateTime? from, DateTime? to) =>
    db.Reservations
        .Include(r => r.Property)
        .Include(r => r.Guest)
        .Where(r =>
            r.Property.OwnerId == ownerId &&
            (propertyId == null || r.PropertyId == propertyId) &&
            (from == null || r.CheckIn >= from) &&
            (to == null || r.CheckOut <= to) &&
            r.Status != ReservationStatus.Cancelled)
        .OrderByDescending(r => r.CreatedAt)
        .ToListAsync();

    public async Task AddAsync(Reservation reservation) =>
        await db.Reservations.AddAsync(reservation);

    public Task SaveChangesAsync() =>
        db.SaveChangesAsync();
}
