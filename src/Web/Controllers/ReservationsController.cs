using System.Security.Claims;
using DT_ASPNET.Application.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Web.Controllers;

[Route("reservations")]
[Authorize]
public class ReservationsController(IReservationService reservations) : Controller
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("")]
    public async Task<IActionResult> MyReservations()
    {
        var list = await reservations.GetMyReservationsAsync(CurrentUserId);
        return View(list);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, string? reason)
    {
        try
        {
            await reservations.CancelAsync(CurrentUserId, id, reason);
            TempData["Success"] = "Reserva cancelada.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("MyReservations");
    }
}
