using DT_ASPNET.Domain.Reservation;
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

    public async Task AddAsync(Reservation reservation) =>
        await db.Reservations.AddAsync(reservation);

    public Task SaveChangesAsync() =>
        db.SaveChangesAsync();
}
