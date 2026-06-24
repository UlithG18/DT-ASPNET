using DT_ASPNET.Application.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Api.Controllers;

[Route("api/reservations")]
[Authorize]
public class ReservationsController(ReservationService reservations) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var result = await reservations.GetMyReservationsAsync(CurrentUserId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReservationRequest req)
    {
        try
        {
            var result = await reservations.CreateAsync(CurrentUserId, req);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, [FromQuery] string? reason)
    {
        try
        {
            await reservations.CancelAsync(CurrentUserId, id, reason);
            return NoContent();
        }
        catch (InvalidOperationException ex) { return NotFound(new { error = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    // Para el dueño: ver reservas de su propiedad
    [HttpGet("property/{propertyId:guid}")]
    public async Task<IActionResult> GetByProperty(Guid propertyId)
    {
        try
        {
            var result = await reservations.GetByPropertyAsync(CurrentUserId, propertyId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }
}
