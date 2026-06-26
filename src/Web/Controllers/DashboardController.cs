using System.Security.Claims;
using DT_ASPNET.Application.Dashboard;
using DT_ASPNET.Application.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Web.Controllers;

[Route("dashboard")]
[Authorize]
public class DashboardController(
    IDashboardService dashboard,
    IReportService reports) : Controller
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("")]
    public async Task<IActionResult> Index(DateTime? from, DateTime? to)
    {
        var data = await dashboard.GetDashboardAsync(CurrentUserId, from, to);
        ViewBag.From = (from ?? DateTime.UtcNow.AddMonths(-12)).ToString("yyyy-MM-dd");
        ViewBag.To   = (to   ?? DateTime.UtcNow).ToString("yyyy-MM-dd");
        return View(data);
    }

    [HttpGet("excel")]
    public async Task<IActionResult> Excel(Guid? propertyId, DateTime? from, DateTime? to)
    {
        var bytes    = await reports.GenerateExcelAsync(CurrentUserId, propertyId, from, to);
        var fileName = $"reporte_{DateTime.UtcNow:yyyyMMdd}.xlsx";
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
