using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
