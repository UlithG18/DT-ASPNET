using DT_ASPNET.Domain.Properties;
using DT_ASPNET.Domain.Reservations;

namespace DT_ASPNET.Application.Dashboard;

public class DashboardService(
    IPropertyRepository properties,
    IReservationRepository reservations) : IDashboardService
{
    public async Task<DashboardDto> GetDashboardAsync(Guid ownerId, DateTime? from, DateTime? to)
    {
        var fromDate = (from ?? DateTime.UtcNow.AddMonths(-12)).ToUniversalTime();
        var toDate   = (to   ?? DateTime.UtcNow).ToUniversalTime();

        var ownerProperties = await properties.GetByOwnerAsync(ownerId);
        var allReservations = await reservations.GetByOwnerAsync(ownerId, null, fromDate, toDate);

        var totalDays = Math.Max((toDate - fromDate).TotalDays, 1);

        var breakdown = ownerProperties.Select(p =>
        {
            var propRes      = allReservations.Where(r => r.PropertyId == p.Id).ToList();
            var occupiedDays = propRes.Sum(r => r.TotalNights);
            var occupancy    = Math.Round(occupiedDays / totalDays * 100, 1);

            return new PropertyStats(
                p.Id, p.Title, p.City,
                propRes.Count,
                propRes.Sum(r => r.TotalAmount),
                (decimal)Math.Min(occupancy, 100.0)
            );
        }).ToList();

        var revenueByMonth = allReservations
            .GroupBy(r => new { r.CheckIn.Year, r.CheckIn.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyRevenue(
                new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                g.Sum(r => r.TotalAmount),
                g.Count()
            ))
            .ToList();

        return new DashboardDto(
            ownerProperties.Count,
            allReservations.Count,
            allReservations.Sum(r => r.TotalAmount),
            breakdown.Count > 0 ? Math.Round(breakdown.Average(p => p.OccupancyRate), 1) : 0,
            breakdown,
            revenueByMonth
        );
    }
}
