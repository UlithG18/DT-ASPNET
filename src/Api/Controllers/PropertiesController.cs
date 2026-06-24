using DT_ASPNET.Application.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Api.Controllers;

[Route("api/properties")]
public class PropertiesController(PropertyService properties) : BaseController
{
    // Público — no requiere login
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? city,
        [FromQuery] DateOnly? checkIn,
        [FromQuery] DateOnly? checkOut)
    {
        var result = await properties.SearchAsync(city, checkIn, checkOut);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await properties.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    // Solo propietarios
    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> GetMine()
    {
        var result = await properties.GetByOwnerAsync(CurrentUserId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreatePropertyRequest req)
    {
        var result = await properties.CreateAsync(CurrentUserId, req);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, CreatePropertyRequest req)
    {
        try
        {
            await properties.UpdateAsync(CurrentUserId, id, req);
            return NoContent();
        }
        catch (InvalidOperationException ex) { return NotFound(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(); }
    }
}
