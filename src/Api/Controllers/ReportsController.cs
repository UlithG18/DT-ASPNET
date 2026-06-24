using DT_ASPNET.Application.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Api.Controllers;

[Route("api/reports")]
[Authorize]
public class ReportsController(IReportService reports) : BaseController
{
    [HttpGet("excel")]
    public async Task<IActionResult> DownloadExcel(
        [FromQuery] Guid? propertyId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var bytes = await reports.GenerateExcelAsync(CurrentUserId, propertyId, from, to);
        var fileName = $"reporte_{DateTime.UtcNow:yyyyMMdd}.xlsx";
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}