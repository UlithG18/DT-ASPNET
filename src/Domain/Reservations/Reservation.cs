namespace DT_ASPNET.Domain.Reservations;

public class Reservation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GuestId { get; set; }
    public Guid PropertyId { get; set; }

    // Siempre: CheckIn 14:00 UTC, CheckOut 12:00 UTC
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int TotalNights { get; set; }

    public decimal PricePerNight { get; set; }
    public decimal TotalAmount { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? CancellationReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Factory method que normaliza los horarios obligatorios
    public static Reservation Create(
        Guid guestId, Guid propertyId,
        DateOnly checkInDate, DateOnly checkOutDate,
        decimal pricePerNight)
    {
        var nights = checkOutDate.DayNumber - checkInDate.DayNumber;

        return new Reservation
        {
            GuestId = guestId,
            PropertyId = propertyId,
            CheckIn = checkInDate.ToDateTime(new TimeOnly(14, 0), DateTimeKind.Utc),
            CheckOut = checkOutDate.ToDateTime(new TimeOnly(12, 0), DateTimeKind.Utc),
            TotalNights = nights,
            PricePerNight = pricePerNight,
            TotalAmount = pricePerNight * nights
        };
    }
}
