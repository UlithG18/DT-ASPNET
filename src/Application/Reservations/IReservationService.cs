namespace DT_ASPNET.Application.Reservations;

public interface IReservationService
{
    Task<ReservationDto> CreateAsync(Guid guestId, CreateReservationRequest req);
    Task CancelAsync(Guid guestId, Guid reservationId, string? reason);
    Task<List<ReservationDto>> GetMyReservationsAsync(Guid guestId);
    Task<List<ReservationDto>> GetByPropertyAsync(Guid ownerId, Guid propertyId);
}
