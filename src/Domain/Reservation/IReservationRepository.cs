namespace DT_ASPNET.Domain.Reservation;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id);
    Task<List<Reservation>> GetByGuestAsync(Guid guestId);
    Task<List<Reservation>> GetByPropertyAsync(Guid propertyId);
    Task<bool> HasOverlapAsync(Guid propertyId, DateTime checkIn, DateTime checkOut);
    Task AddAsync(Reservation reservation);
    Task SaveChangesAsync();
}
