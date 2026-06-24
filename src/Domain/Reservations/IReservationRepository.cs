namespace DT_ASPNET.Domain.Reservations;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id);
    Task<List<Reservation>> GetByGuestAsync(Guid guestId);
    Task<List<Reservation>> GetByPropertyAsync(Guid propertyId);
    Task<bool> HasOverlapAsync(Guid propertyId, DateTime checkIn, DateTime checkOut);
    Task<List<Reservation>> GetByOwnerAsync(Guid ownerId, Guid? propertyId, DateTime? from, DateTime? to);
    Task AddAsync(Reservation reservation);
    Task SaveChangesAsync();
}
