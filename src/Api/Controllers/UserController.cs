using DT_ASPNET.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Api.Controllers;

[Route("api/users")]
[Authorize]
public class UsersController(IUserService users) : BaseController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var profile = await users.GetProfileAsync(CurrentUserId);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest req)
    {
        try
        {
            await users.UpdateProfileAsync(CurrentUserId, req);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("me/become-owner")]
    public async Task<IActionResult> BecomeOwner()
    {
        await users.BecomeOwnerAsync(CurrentUserId);
        return NoContent();
    }
}