using System.Security.Claims;
using DT_ASPNET.Application.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace DT_ASPNET.Web.Controllers;

public class AuthController(IAuthService auth) : Controller
{
    [HttpGet("/auth/login")]
    public IActionResult Login(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost("/auth/login")]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl)
    {
        try
        {
            var result = await auth.LoginAsync(new LoginRequest(email, password));
            await SignInUser(result);
            return Redirect(returnUrl ?? "/");
        }
        catch (UnauthorizedAccessException)
        {
            ViewBag.Error     = "Correo o contraseña incorrectos.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }

    [HttpGet("/auth/register")]
    public IActionResult Register() => View();

    [HttpPost("/auth/register")]
    public async Task<IActionResult> Register(
        string email, string password, string firstName, string lastName)
    {
        try
        {
            var result = await auth.RegisterAsync(
                new RegisterRequest(email, password, firstName, lastName));
            await SignInUser(result);
            TempData["Success"] = "¡Cuenta creada! Bienvenido/a.";
            return RedirectToAction("Index", "Home");
        }
        catch (InvalidOperationException ex)
        {
            ViewBag.Error = ex.Message;
            return View();
        }
    }

    [HttpGet("/auth/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUser(AuthResponse result)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.UserId.ToString()),
            new(ClaimTypes.Email, ""),
        };

        if (result.IsOwner)
            claims.Add(new Claim(ClaimTypes.Role, "Owner"));

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });
    }
}
