using DT_ASPNET.Application.Properties;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Web.Controllers;

public class HomeController(IPropertyService properties) : Controller
{
    public async Task<IActionResult> Index(string? city, DateOnly? checkIn, DateOnly? checkOut)
    {
        var list = await properties.SearchAsync(city, checkIn, checkOut);
        ViewBag.City    = city;
        ViewBag.CheckIn = checkIn?.ToString("yyyy-MM-dd");
        ViewBag.CheckOut = checkOut?.ToString("yyyy-MM-dd");
        return View(list);
    }
}
