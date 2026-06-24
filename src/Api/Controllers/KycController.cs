using DT_ASPNET.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Api.Controllers;

[Route("api/kyc")]
[Authorize]
public class KycController(IKycService kyc) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Submit(IFormFile document)
    {
        if (document is null || document.Length == 0)
            return BadRequest(new { error = "Document file is required." });

        var req = new KycSubmitRequest(
            CurrentUserId,
            document.OpenReadStream(),
            document.FileName);

        var result = await kyc.ProcessAsync(req);

        return result.Approved
            ? Ok(new { message = "Identity verified successfully." })
            : UnprocessableEntity(new { error = result.RejectionReason });
    }
}
