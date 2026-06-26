using DT_ASPNET.Application.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Api.Controllers;

[Route("api/dashboard")]
[Authorize]
public class DashboardController(IDashboardService dashboard) : BaseController
{
    /// <summary>
    /// Retorna métricas de rendimiento del portafolio del propietario.
    /// Parámetros opcionales: from / to (formato ISO 8601).
    /// Por defecto devuelve los últimos 12 meses.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var result = await dashboard.GetDashboardAsync(CurrentUserId, from, to);
        return Ok(result);
    }
}
