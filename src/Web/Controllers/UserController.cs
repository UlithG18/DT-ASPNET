using System.Security.Claims;
using DT_ASPNET.Application.Notifications;
using DT_ASPNET.Application.Users;
using DT_ASPNET.Application.Wishlists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Web.Controllers;

[Authorize]
public class UserController(
    IUserService users,
    IKycService kyc,
    IWishlistService wishlist,
    INotificationService notifications) : Controller
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ── Perfil ──────────────────────────────────────
    [HttpGet("/profile")]
    public async Task<IActionResult> Profile()
    {
        var profile = await users.GetProfileAsync(CurrentUserId);
        return profile is null ? NotFound() : View(profile);
    }

    [HttpPost("/profile")]
    public async Task<IActionResult> Profile(string firstName, string lastName, string? phoneNumber)
    {
        await users.UpdateProfileAsync(CurrentUserId,
            new UpdateProfileRequest(firstName, lastName, phoneNumber));
        TempData["Success"] = "Perfil actualizado.";
        return RedirectToAction("Profile");
    }

    [HttpPost("/profile/become-owner")]
    public async Task<IActionResult> BecomeOwner()
    {
        await users.BecomeOwnerAsync(CurrentUserId);
        TempData["Success"] = "¡Ahora eres propietario! Vuelve a iniciar sesión para ver el dashboard.";
        return RedirectToAction("Profile");
    }

    // ── KYC ─────────────────────────────────────────
    [HttpGet("/kyc")]
    public async Task<IActionResult> Kyc()
    {
        var profile = await users.GetProfileAsync(CurrentUserId);
        return View(profile);
    }

    [HttpPost("/kyc")]
    public async Task<IActionResult> Kyc(IFormFile document)
    {
        if (document is null || document.Length == 0)
        {
            TempData["Error"] = "Debes subir un archivo.";
            return RedirectToAction("Kyc");
        }

        using var stream = document.OpenReadStream();
        var result = await kyc.ProcessAsync(
            new KycSubmitRequest(CurrentUserId, stream, document.FileName));

        TempData[result.Approved ? "Success" : "Error"] = result.Approved
            ? "¡Identidad verificada! Ya puedes hacer reservas."
            : $"Verificación rechazada: {result.RejectionReason}";

        return RedirectToAction("Kyc");
    }

    // ── Wishlist ─────────────────────────────────────
    [HttpGet("/wishlist")]
    public async Task<IActionResult> Wishlist()
    {
        var items = await wishlist.GetAsync(CurrentUserId);
        return View(items);
    }

    [HttpPost("/wishlist/toggle/{propertyId:guid}")]
    public async Task<IActionResult> Toggle(Guid propertyId, string? returnUrl)
    {
        await wishlist.ToggleAsync(CurrentUserId, propertyId);
        return Redirect(returnUrl ?? "/wishlist");
    }

    // ── Notificaciones ───────────────────────────────
    [HttpGet("/notifications")]
    public async Task<IActionResult> Notifications()
    {
        var list = await notifications.GetMyNotificationsAsync(CurrentUserId);
        return View(list);
    }

    [HttpPost("/notifications/{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        await notifications.MarkAsReadAsync(id);
        return RedirectToAction("Notifications");
    }
}