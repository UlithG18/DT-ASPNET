using DT_ASPNET.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Api.Controllers;

[Route("api/notifications")]
[Authorize]
public class NotificationsController(INotificationService notifications) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var result = await notifications.GetMyNotificationsAsync(CurrentUserId);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await notifications.MarkAsReadAsync(id);
        return NoContent();
    }
}
