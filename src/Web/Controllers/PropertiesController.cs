using System.Security.Claims;
using DT_ASPNET.Application.Properties;
using DT_ASPNET.Application.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Web.Controllers;

[Route("properties")]
public class PropertiesController(
    IPropertyService properties,
    IReservationService reservations) : Controller
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Detail(Guid id)
    {
        var prop = await properties.GetByIdAsync(id);
        return prop is null ? NotFound() : View(prop);
    }

    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> Mine()
    {
        var list = await properties.GetByOwnerAsync(CurrentUserId);
        return View(list);
    }

    [HttpGet("create")]
    [Authorize]
    public IActionResult Create() => View(new CreatePropertyRequest(
        "", "", "", "", 0, 1, 1, 1, []));

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create(
        string title, string description, string city, string address,
        decimal pricePerNight, int maxGuests, int bedrooms, int bathrooms,
        string? photoUrlsRaw)
    {
        var photos = (photoUrlsRaw ?? "")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var req = new CreatePropertyRequest(
            title, description, city, address,
            pricePerNight, maxGuests, bedrooms, bathrooms, photos);

        await properties.CreateAsync(CurrentUserId, req);
        TempData["Success"] = "Inmueble publicado correctamente.";
        return RedirectToAction("Mine");
    }

    [HttpPost("{id:guid}/reserve")]
    [Authorize]
    public async Task<IActionResult> Reserve(Guid id, DateOnly checkIn, DateOnly checkOut)
    {
        try
        {
            await reservations.CreateAsync(CurrentUserId,
                new CreateReservationRequest(id, checkIn, checkOut));
            TempData["Success"] = "¡Reserva confirmada! Revisa tu correo.";
            return RedirectToAction("MyReservations", "Reservations");
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Detail", new { id });
        }
    }
}
