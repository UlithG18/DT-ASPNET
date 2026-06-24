using DT_ASPNET.Application.Notifications;
using DT_ASPNET.Domain.Notifications;
using DT_ASPNET.Domain.Reservations;
using DT_ASPNET.Domain.Users;

namespace DT_ASPNET.Application.Reservations;

public record CreateReservationRequest(Guid PropertyId, DateOnly CheckIn, DateOnly CheckOut);

public record ReservationDto(
    Guid Id, Guid PropertyId, Guid GuestId,
    DateTime CheckIn, DateTime CheckOut, int TotalNights,
    decimal PricePerNight, decimal TotalAmount, string Status);

public class ReservationService(
    IReservationRepository reservations,
    Domain.Properties.IPropertyRepository properties,
    Domain.Users.IUserRepository users,
    INotificationRepository notifications,
    IEmailService email) : IReservationService
{
    public async Task<ReservationDto> CreateAsync(Guid guestId, CreateReservationRequest req)
    {
        // 1. Validar KYC
        var guest = await users.GetByIdAsync(guestId)
            ?? throw new InvalidOperationException("User not found.");

        if (guest.KycStatus != KycStatus.Approved)
            throw new InvalidOperationException("Identity verification (KYC) required before booking.");

        // 2. Validar que el inmueble existe
        var property = await properties.GetByIdAsync(req.PropertyId)
            ?? throw new InvalidOperationException("Property not found.");

        // 3. Crear la reserva (normaliza check-in 14:00 / check-out 12:00)
        var reservation = Reservation.Create(
            guestId, req.PropertyId,
            req.CheckIn, req.CheckOut,
            property.PricePerNight);

        // 4. Validar disponibilidad — anti double-booking
        var hasOverlap = await reservations.HasOverlapAsync(
            req.PropertyId, reservation.CheckIn, reservation.CheckOut);

        if (hasOverlap)
            throw new InvalidOperationException("Property is not available for the selected dates.");

        reservation.Status = ReservationStatus.Confirmed;

        await reservations.AddAsync(reservation);

        // 5. Notificación in-app
        await notifications.AddAsync(new Domain.Notifications.Notification
        {
            UserId = guestId,
            Type = NotificationType.ReservationConfirmed,
            Title = "Reservation confirmed",
            Body = $"Your stay at {property.Title} from {req.CheckIn} to {req.CheckOut} is confirmed."
        });

        await email.SendAsync(
            guest.Email,
            $"{guest.FirstName} {guest.LastName}",
            "Reserva confirmada ✅",
            $"""
            <h2>¡Tu reserva está confirmada!</h2>
            <p>Propiedad: <strong>{property.Title}</strong></p>
            <p>Check-in: <strong>{req.CheckIn:dd/MM/yyyy} a las 2:00 PM</strong></p>
            <p>Check-out: <strong>{req.CheckOut:dd/MM/yyyy} a las 12:00 PM</strong></p>
            <p>Total: <strong>${reservation.TotalAmount:N2}</strong></p>
            """);

        await reservations.SaveChangesAsync();

        return ToDto(reservation);
    }

    public async Task CancelAsync(Guid guestId, Guid reservationId, string? reason)
    {
        var reservation = await reservations.GetByIdAsync(reservationId)
            ?? throw new InvalidOperationException("Reservation not found.");
    
        if (reservation.GuestId != guestId)
            throw new UnauthorizedAccessException("Not your reservation.");
    
        if (reservation.Status == ReservationStatus.Cancelled)
            throw new InvalidOperationException("Reservation is already cancelled.");
    
        var guest = await users.GetByIdAsync(guestId)
            ?? throw new InvalidOperationException("User not found.");
    
        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancellationReason = reason;
    
        await notifications.AddAsync(new Domain.Notifications.Notification
        {
            UserId = guestId,
            Type = NotificationType.ReservationCancelled,
            Title = "Reservation cancelled",
            Body = "Your reservation has been cancelled."
        });
    
        await email.SendAsync(
            guest.Email,
            $"{guest.FirstName} {guest.LastName}",
            "Reserva cancelada",
            $"""
            <h2>Tu reserva ha sido cancelada</h2>
            {(reason is not null ? $"<p>Motivo: {reason}</p>" : "")}
            """);
    
        await reservations.SaveChangesAsync();
    }

    public async Task<List<ReservationDto>> GetMyReservationsAsync(Guid guestId)
    {
        var list = await reservations.GetByGuestAsync(guestId);
        return list.Select(ToDto).ToList();
    }

    // Para el dashboard del propietario
    public async Task<List<ReservationDto>> GetByPropertyAsync(Guid ownerId, Guid propertyId)
    {
        var property = await properties.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedAccessException("Not your property.");

        var list = await reservations.GetByPropertyAsync(propertyId);
        return list.Select(ToDto).ToList();
    }

    private static ReservationDto ToDto(Domain.Reservations.Reservation r) => new(
        r.Id, r.PropertyId, r.GuestId,
        r.CheckIn, r.CheckOut, r.TotalNights,
        r.PricePerNight, r.TotalAmount, r.Status.ToString());
}
