namespace DT_ASPNET.Application.Dashboard;

public record PropertyStats(
    Guid PropertyId,
    string Title,
    string City,
    int TotalReservations,
    decimal TotalRevenue,
    decimal OccupancyRate  // porcentaje de días ocupados en el período
);

public record DashboardDto(
    int TotalProperties,
    int TotalReservations,
    decimal TotalRevenue,
    decimal AverageOccupancy,
    List<PropertyStats> PropertiesBreakdown,
    List<MonthlyRevenue> RevenueByMonth
);

public record MonthlyRevenue(string Month, decimal Revenue, int Reservations);

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(Guid ownerId, DateTime? from, DateTime? to);
}
