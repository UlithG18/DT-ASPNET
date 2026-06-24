using DT_ASPNET.Application.Wishlists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Api.Controllers;

[Route("api/wishlist")]
[Authorize]
public class WishlistController(IWishlistService wishlist) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var result = await wishlist.GetAsync(CurrentUserId);
        return Ok(result);
    }

    // Toggle: si no está lo agrega, si está lo quita
    [HttpPost("{propertyId:guid}")]
    public async Task<IActionResult> Toggle(Guid propertyId)
    {
        await wishlist.ToggleAsync(CurrentUserId, propertyId);
        return NoContent();
    }
}
